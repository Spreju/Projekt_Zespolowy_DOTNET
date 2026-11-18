namespace Olympus.Core.Entities
{
    public class Modlitwa
    {
        public int Id { get; set; }
        public string Tresc { get; set; } = string.Empty;

        // Opis składanej ofiary (tekst); zdjęcie daru trafia do pól Zalacznik*
        public string? Ofiara { get; set; }

        public StatusModlitwy Status { get; set; } = StatusModlitwy.Oczekujaca;

        public DateTime DataZlozenia { get; set; }
        public DateTime? DataRozpatrzenia { get; set; }

        // Odpowiedź boga (uzasadnienie decyzji lub opis kary)
        public string? OdpowiedzBoga { get; set; }

        // Załącznik (zdjęcie ofiary) - plik na serwerze
        public string? ZalacznikNazwaPliku { get; set; }        // nazwa zapisana na dysku (GUID)
        public string? ZalacznikNazwaOryginalna { get; set; }   // nazwa widoczna dla użytkownika
        public string? ZalacznikTyp { get; set; }               // content-type

        // Klucz obcy i właściwość nawigacyjna do Śmiertelnika
        public int SmiertelnikId { get; set; }
        public Smiertelnik Smiertelnik { get; set; } = null!;

        // Klucz obcy i właściwość nawigacyjna do Boga
        public int BogId { get; set; }
        public Bog Bog { get; set; } = null!;
    }
}
