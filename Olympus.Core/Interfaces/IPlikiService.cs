namespace Olympus.Core.Interfaces;

public record ZapisanyPlik(string NazwaPliku, string NazwaOryginalna, string ContentType);

/// <summary>Obsługa plików wgrywanych na serwer (np. zdjęcia ofiar).</summary>
public interface IPlikiService
{
    /// <summary>Zwraca komunikat błędu albo null, jeśli plik jest poprawny.</summary>
    string? Waliduj(string nazwaPliku, long rozmiar);

    /// <summary>Zapisuje plik na dysku. Rzuca InvalidOperationException dla niepoprawnego pliku.</summary>
    Task<ZapisanyPlik> ZapiszAsync(Stream zawartosc, string nazwaOryginalna, long rozmiar);

    /// <summary>Otwiera zapisany plik do odczytu (null, jeśli nie istnieje).</summary>
    Stream? Otworz(string nazwaPliku);

    void Usun(string nazwaPliku);
}
