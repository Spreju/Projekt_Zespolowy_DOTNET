namespace Olympus.Core.Entities;

public class Heros
{
    public int Id { get; set; }
    public string Imie { get; set; } = string.Empty;

    // Id konta (ApplicationUser) powiązanego z tym herosem
    public string? UserId { get; set; }

    // Relacja ManyToMany z Misja (przez tabelę łączącą MisjaHeros)
    public ICollection<MisjaHeros> Misje { get; set; } = new List<MisjaHeros>();
}