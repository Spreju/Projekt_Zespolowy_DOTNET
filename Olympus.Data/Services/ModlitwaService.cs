using Microsoft.EntityFrameworkCore;
using Olympus.Core.Entities;
using Olympus.Core.Interfaces;
using Olympus.Data.Data;

namespace Olympus.Data.Services;

public class ModlitwaService : IModlitwaService
{
    private readonly ApplicationDbContext _db;
    private readonly IPlikiService _pliki;

    public ModlitwaService(ApplicationDbContext db, IPlikiService pliki)
    {
        _db = db;
        _pliki = pliki;
    }

    public async Task<IReadOnlyList<Bog>> PobierzBogowAsync()
    {
        return await _db.Bogowie.AsNoTracking().OrderBy(b => b.Imie).ToListAsync();
    }

    public async Task<IReadOnlyList<Modlitwa>> PobierzModlitwySmiertelnikaAsync(string userId)
    {
        return await _db.Modlitwy.AsNoTracking()
            .Include(m => m.Bog)
            .Where(m => m.Smiertelnik.UserId == userId)
            .OrderByDescending(m => m.DataZlozenia)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Modlitwa>> PobierzModlitwyDlaBogaAsync(string userId, bool wszystkie)
    {
        var query = _db.Modlitwy.AsNoTracking()
            .Include(m => m.Bog)
            .Include(m => m.Smiertelnik)
            .AsQueryable();

        if (!wszystkie)
        {
            query = query.Where(m => m.Bog.UserId == userId);
        }

        // Oczekujące na górze, w ramach grupy - od najnowszych
        return await query
            .OrderBy(m => m.Status != StatusModlitwy.Oczekujaca)
            .ThenByDescending(m => m.DataZlozenia)
            .ToListAsync();
    }

    public async Task<Modlitwa?> PobierzAsync(int id)
    {
        return await _db.Modlitwy.AsNoTracking()
            .Include(m => m.Bog)
            .Include(m => m.Smiertelnik)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Modlitwa> ZlozAsync(string userId, string imie, ModlitwaDane dane, ZapisanyPlik? zalacznik)
    {
        if (!await _db.Bogowie.AnyAsync(b => b.Id == dane.BogId))
        {
            throw new InvalidOperationException("Wybrane bóstwo nie istnieje.");
        }

        // Rekord śmiertelnika tworzymy przy pierwszej modlitwie
        var smiertelnik = await _db.Smiertelnicy.FirstOrDefaultAsync(s => s.UserId == userId);
        if (smiertelnik == null)
        {
            smiertelnik = new Smiertelnik
            {
                UserId = userId,
                Imie = string.IsNullOrWhiteSpace(imie) ? "Nieznany śmiertelnik" : imie,
                PoziomPoboznosci = 0
            };
            _db.Smiertelnicy.Add(smiertelnik);
        }

        var modlitwa = new Modlitwa
        {
            Smiertelnik = smiertelnik,
            BogId = dane.BogId,
            Tresc = dane.Tresc.Trim(),
            Ofiara = string.IsNullOrWhiteSpace(dane.Ofiara) ? null : dane.Ofiara.Trim(),
            Status = StatusModlitwy.Oczekujaca,
            DataZlozenia = DateTime.UtcNow
        };
        UstawZalacznik(modlitwa, zalacznik);

        _db.Modlitwy.Add(modlitwa);
        await _db.SaveChangesAsync();
        return modlitwa;
    }

    public async Task AktualizujAsync(int id, string userId, ModlitwaDane dane, ZapisanyPlik? nowyZalacznik, bool usunZalacznik)
    {
        var modlitwa = await PobierzWlasnaAsync(id, userId);

        if (modlitwa.Status != StatusModlitwy.Oczekujaca)
        {
            throw new InvalidOperationException("Można edytować tylko modlitwy oczekujące na rozpatrzenie.");
        }
        if (!await _db.Bogowie.AnyAsync(b => b.Id == dane.BogId))
        {
            throw new InvalidOperationException("Wybrane bóstwo nie istnieje.");
        }

        var staryPlik = modlitwa.ZalacznikNazwaPliku;

        modlitwa.BogId = dane.BogId;
        modlitwa.Tresc = dane.Tresc.Trim();
        modlitwa.Ofiara = string.IsNullOrWhiteSpace(dane.Ofiara) ? null : dane.Ofiara.Trim();

        if (nowyZalacznik != null)
        {
            UstawZalacznik(modlitwa, nowyZalacznik);
        }
        else if (usunZalacznik)
        {
            UstawZalacznik(modlitwa, null);
        }

        await _db.SaveChangesAsync();

        // Stary plik kasujemy dopiero po udanym zapisie w bazie
        if (staryPlik != null && staryPlik != modlitwa.ZalacznikNazwaPliku)
        {
            _pliki.Usun(staryPlik);
        }
    }

    public async Task UsunAsync(int id, string userId)
    {
        var modlitwa = await PobierzWlasnaAsync(id, userId);

        if (modlitwa.Status != StatusModlitwy.Oczekujaca)
        {
            throw new InvalidOperationException("Można usunąć tylko modlitwy oczekujące na rozpatrzenie.");
        }

        var plik = modlitwa.ZalacznikNazwaPliku;
        _db.Modlitwy.Remove(modlitwa);
        await _db.SaveChangesAsync();

        if (plik != null)
        {
            _pliki.Usun(plik);
        }
    }

    public async Task RozpatrzAsync(int id, string userId, bool jestZeusem, StatusModlitwy decyzja, string? odpowiedz)
    {
        var modlitwa = await _db.Modlitwy
            .Include(m => m.Bog)
            .Include(m => m.Smiertelnik)
            .FirstOrDefaultAsync(m => m.Id == id);

        // Bóg widzi tylko modlitwy skierowane do siebie; Zeus (admin) - wszystkie
        if (modlitwa == null || (!jestZeusem && modlitwa.Bog.UserId != userId))
        {
            throw new KeyNotFoundException();
        }
        if (modlitwa.Status != StatusModlitwy.Oczekujaca)
        {
            throw new InvalidOperationException("Ta modlitwa została już rozpatrzona.");
        }
        if (decyzja == StatusModlitwy.Oczekujaca)
        {
            throw new InvalidOperationException("Wybierz decyzję: akceptacja, odrzucenie lub kara.");
        }

        modlitwa.Status = decyzja;
        modlitwa.OdpowiedzBoga = string.IsNullOrWhiteSpace(odpowiedz) ? null : odpowiedz.Trim();
        modlitwa.DataRozpatrzenia = DateTime.UtcNow;

        // Pobożność rośnie po przychylnym rozpatrzeniu, maleje po karze
        if (decyzja == StatusModlitwy.Zaakceptowana)
        {
            modlitwa.Smiertelnik.PoziomPoboznosci++;
        }
        else if (decyzja == StatusModlitwy.Ukarana)
        {
            modlitwa.Smiertelnik.PoziomPoboznosci = Math.Max(0, modlitwa.Smiertelnik.PoziomPoboznosci - 1);
        }

        await _db.SaveChangesAsync();
    }

    // Śmiertelnik ma dostęp wyłącznie do własnych modlitw (cudza = "nie znaleziono")
    private async Task<Modlitwa> PobierzWlasnaAsync(int id, string userId)
    {
        var modlitwa = await _db.Modlitwy
            .Include(m => m.Smiertelnik)
            .FirstOrDefaultAsync(m => m.Id == id && m.Smiertelnik.UserId == userId);

        return modlitwa ?? throw new KeyNotFoundException();
    }

    private static void UstawZalacznik(Modlitwa modlitwa, ZapisanyPlik? plik)
    {
        modlitwa.ZalacznikNazwaPliku = plik?.NazwaPliku;
        modlitwa.ZalacznikNazwaOryginalna = plik?.NazwaOryginalna;
        modlitwa.ZalacznikTyp = plik?.ContentType;
    }
}
