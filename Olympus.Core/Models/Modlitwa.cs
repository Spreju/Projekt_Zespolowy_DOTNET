using System;
using System.Collections.Generic;
using System.Text;

namespace Olympus.Core.Entities
{
    public class Modlitwa
    {
        public int Id { get; set; }
        public string Tresc { get; set; }
        public string Ofiara { get; set; }
        public string Status { get; set; } // np. "Oczekująca", "Zaakceptowana", "Odrzucona"

        // Klucz obcy i właściwość nawigacyjna do Śmiertelnika
        public int SmiertelnikId { get; set; }
        public Smiertelnik Smiertelnik { get; set; }

        // Klucz obcy i właściwość nawigacyjna do Boga
        public int BogId { get; set; }
        public Bog Bog { get; set; }
    }
}