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
    }
}
