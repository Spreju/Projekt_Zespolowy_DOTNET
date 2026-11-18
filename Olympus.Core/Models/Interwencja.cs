using System;
using System.Collections.Generic;
using System.Text;

namespace Olympus.Core.Entities
{
    public class Interwencja
    {
        public int Id { get; set; }
        public string Typ { get; set; } = string.Empty;      // np. "Zjawisko naturalne", "Błogosławieństwo", "Kara"
        public string Opis { get; set; } = string.Empty;
        public string Kraina { get; set; } = string.Empty;   // region, na który działa interwencja
        public bool CzyGlobalna { get; set; }                // true = wymaga zatwierdzenia przez Zeusa
        public string Status { get; set; } = "Oczekująca";   // "Oczekująca", "Zatwierdzona", "Odrzucona"
        public DateTime DataUtworzenia { get; set; } = DateTime.Now;

        // Klucz obcy i właściwość nawigacyjna do Boga
        public int BogId { get; set; }
        public Bog? Bog { get; set; }
    }
}