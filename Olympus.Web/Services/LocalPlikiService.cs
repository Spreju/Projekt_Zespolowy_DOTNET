using Olympus.Core.Interfaces;

namespace Olympus.Web.Services;

/// <summary>
/// Zapis plików na dysku serwera, poza wwwroot (dostęp tylko przez kontroler z autoryzacją).
/// </summary>
public class LocalPlikiService : IPlikiService
{
    public const long MaksRozmiar = 5 * 1024 * 1024; // 5 MB

    private static readonly Dictionary<string, string> DozwoloneTypy = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".gif"] = "image/gif",
        [".webp"] = "image/webp"
    };

    private readonly string _katalog;

    public LocalPlikiService(IWebHostEnvironment env)
    {
        _katalog = Path.Combine(env.ContentRootPath, "App_Data", "uploads");
        Directory.CreateDirectory(_katalog);
    }

    public string? Waliduj(string nazwaPliku, long rozmiar)
    {
        if (rozmiar <= 0)
        {
            return "Plik jest pusty.";
        }
        if (rozmiar > MaksRozmiar)
        {
            return $"Plik jest za duży (maksymalnie {MaksRozmiar / 1024 / 1024} MB).";
        }
        if (!DozwoloneTypy.ContainsKey(Path.GetExtension(nazwaPliku)))
        {
            return "Dozwolone formaty zdjęć: JPG, PNG, GIF, WEBP.";
        }
        return null;
    }

    public async Task<ZapisanyPlik> ZapiszAsync(Stream zawartosc, string nazwaOryginalna, long rozmiar)
    {
        var blad = Waliduj(nazwaOryginalna, rozmiar);
        if (blad != null)
        {
            throw new InvalidOperationException(blad);
        }

        var rozszerzenie = Path.GetExtension(nazwaOryginalna).ToLowerInvariant();

        // Sprawdzamy sygnaturę pliku, a nie tylko rozszerzenie
        var naglowek = new byte[12];
        var odczytano = await zawartosc.ReadAsync(naglowek.AsMemory(0, naglowek.Length));
        if (!ZgadzaSieSygnatura(rozszerzenie, naglowek, odczytano))
        {
            throw new InvalidOperationException("Zawartość pliku nie jest poprawnym zdjęciem.");
        }

        var nazwaNaDysku = $"{Guid.NewGuid():N}{rozszerzenie}";
        var sciezka = Path.Combine(_katalog, nazwaNaDysku);

        await using (var wyjscie = File.Create(sciezka))
        {
            await wyjscie.WriteAsync(naglowek.AsMemory(0, odczytano));
            await zawartosc.CopyToAsync(wyjscie);
        }

        var nazwaDlaUzytkownika = Path.GetFileName(nazwaOryginalna);
        if (nazwaDlaUzytkownika.Length > 200)
        {
            nazwaDlaUzytkownika = nazwaDlaUzytkownika[^200..];
        }

        return new ZapisanyPlik(nazwaNaDysku, nazwaDlaUzytkownika, DozwoloneTypy[rozszerzenie]);
    }

    public Stream? Otworz(string nazwaPliku)
    {
        var sciezka = SciezkaBezpieczna(nazwaPliku);
        return sciezka != null && File.Exists(sciezka)
            ? new FileStream(sciezka, FileMode.Open, FileAccess.Read, FileShare.Read)
            : null;
    }

    public void Usun(string nazwaPliku)
    {
        var sciezka = SciezkaBezpieczna(nazwaPliku);
        if (sciezka != null && File.Exists(sciezka))
        {
            File.Delete(sciezka);
        }
    }

    // Chroni przed ścieżkami typu ..\..\plik
    private string? SciezkaBezpieczna(string nazwaPliku)
    {
        if (string.IsNullOrWhiteSpace(nazwaPliku) || nazwaPliku != Path.GetFileName(nazwaPliku))
        {
            return null;
        }
        return Path.Combine(_katalog, nazwaPliku);
    }

    private static bool ZgadzaSieSygnatura(string rozszerzenie, byte[] b, int dlugosc)
    {
        if (dlugosc < 12)
        {
            return false;
        }

        return rozszerzenie switch
        {
            ".jpg" or ".jpeg" => b[0] == 0xFF && b[1] == 0xD8 && b[2] == 0xFF,
            ".png" => b[0] == 0x89 && b[1] == 0x50 && b[2] == 0x4E && b[3] == 0x47
                      && b[4] == 0x0D && b[5] == 0x0A && b[6] == 0x1A && b[7] == 0x0A,
            ".gif" => b[0] == 0x47 && b[1] == 0x49 && b[2] == 0x46 && b[3] == 0x38,
            ".webp" => b[0] == 0x52 && b[1] == 0x49 && b[2] == 0x46 && b[3] == 0x46
                       && b[8] == 0x57 && b[9] == 0x45 && b[10] == 0x42 && b[11] == 0x50,
            _ => false
        };
    }
}
