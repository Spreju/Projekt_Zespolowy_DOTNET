namespace Olympus.Core.Entities
{
    public class Smiertelnik
    {
        public int Id { get; set; } // Zamiast idSmiertelnika - standard EF
        public string Imie { get; set; } = string.Empty;
        public int PoziomPoboznosci { get; set; }

        // Id konta (ApplicationUser) powiązanego ze śmiertelnikiem
        public string? UserId { get; set; }

        // Relacja OneToMany: Jeden śmiertelnik może złożyć wiele modlitw
        public ICollection<Modlitwa> Modlitwy { get; set; } = new List<Modlitwa>();
    }
}
