namespace Olympus.Core.Entities;

public class MisjaHeros
{
    public int MisjaId { get; set; }
    public Misja Misja { get; set; } = null!;

    public int HerosId { get; set; }
    public Heros Heros { get; set; } = null!;

    public StatusRealizacji Status { get; set; } = StatusRealizacji.Przyjeta;
    public DateTime DataPrzyjecia { get; set; }
    public DateTime? DataWykonania { get; set; }
    public string? Raport { get; set; }
}