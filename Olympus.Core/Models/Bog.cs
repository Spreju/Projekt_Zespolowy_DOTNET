using System;
using System.Collections.Generic;
using System.Text;

namespace Olympus.Core.Entities
{
    public class Bog
    {
        public int Id { get; set; }
        public string Imie { get; set; }
        public string Domena { get; set; }

        // Relacja OneToMany: Jeden Bóg otrzymuje wiele modlitw
        public ICollection<Modlitwa> Modlitwy { get; set; } = new List<Modlitwa>();
        // Id konta (ApplicationUser) powiązanego z tym bogiem
        public string? UserId { get; set; }

        // Relacja OneToMany: Jeden Bóg zleca wiele misji
        public ICollection<Misja> Misje { get; set; } = new List<Misja>();
    }
}
