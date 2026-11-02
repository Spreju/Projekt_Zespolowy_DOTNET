using System;
using System.Collections.Generic;
using System.Text;

namespace Olympus.Core.Entities
{
    public class Smiertelnik
    {
        public int Id { get; set; } // Zamiast idSmiertelnika - standard EF
        public string Imie { get; set; }
        public int PoziomPoboznosci { get; set; }

        // Relacja OneToMany: Jeden śmiertelnik może złożyć wiele modlitw
        public ICollection<Modlitwa> Modlitwy { get; set; } = new List<Modlitwa>();
    }
}
