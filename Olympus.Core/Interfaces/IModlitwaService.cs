using Olympus.Core.Entities;

namespace Olympus.Core.Interfaces;

public class ModlitwaDane
{
    public int BogId { get; set; }
    public string Tresc { get; set; } = string.Empty;
    public string? Ofiara { get; set; }
}

/// <summary>
/// Logika biznesowa modułu modlitw.
/// KeyNotFoundException  = brak zasobu lub brak dostępu (kontroler zwraca 404),
/// InvalidOperationException = naruszenie reguły biznesowej (komunikat dla użytkownika).
/// </summary>
public interface IModlitwaService
{
    Task<IReadOnlyList<Bog>> PobierzBogowAsync();

    Task<IReadOnlyList<Modlitwa>> PobierzModlitwySmiertelnikaAsync(string userId);
    Task<IReadOnlyList<Modlitwa>> PobierzModlitwyDlaBogaAsync(string userId, bool wszystkie);
    Task<Modlitwa?> PobierzAsync(int id);

    Task<Modlitwa> ZlozAsync(string userId, string imie, ModlitwaDane dane, ZapisanyPlik? zalacznik);

    /// <param name="nowyZalacznik">Nowy plik (zastępuje stary) albo null.</param>
    /// <param name="usunZalacznik">True = usuń obecny załącznik bez zastępowania.</param>
    Task AktualizujAsync(int id, string userId, ModlitwaDane dane, ZapisanyPlik? nowyZalacznik, bool usunZalacznik);

    Task UsunAsync(int id, string userId);

    Task RozpatrzAsync(int id, string userId, bool jestZeusem, StatusModlitwy decyzja, string? odpowiedz);
}
