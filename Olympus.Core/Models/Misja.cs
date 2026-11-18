namespace Olympus.Core.Entities;

public class Misja
{
    public int Id { get; set; }
    public string Tytul { get; set; } = string.Empty;
    public string Opis { get; set; } = string.Empty;
    public int PoziomTrudnosci { get; set; } // 1-5
    public DateTime DataUtworzenia { get; set; }
    public DateOnly? Termin { get; set; }
    public StatusMisji Status { get; set; } = StatusMisji.Aktywna;

    // Relacja OneToMany: jeden Bóg zleca wiele misji
    public int BogId { get; set; }
    public Bog Bog { get; set; } = null!;

    // Relacja ManyToMany z Heros (przez tabelę łączącą MisjaHeros)
    public ICollection<MisjaHeros> Herosi { get; set; } = new List<MisjaHeros>();
}