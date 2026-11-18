using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Olympus.Core.Entities;

namespace Olympus.Web.ViewModels;

public class ModlitwaFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Wybierz bóstwo, do którego się modlisz.")]
    [Range(1, int.MaxValue, ErrorMessage = "Wybierz bóstwo, do którego się modlisz.")]
    [Display(Name = "Bóstwo")]
    public int BogId { get; set; }

    [Required(ErrorMessage = "Treść modlitwy jest wymagana.")]
    [StringLength(2000, MinimumLength = 10,
        ErrorMessage = "Modlitwa musi mieć od {2} do {1} znaków.")]
    [Display(Name = "Treść modlitwy")]
    public string Tresc { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Opis ofiary może mieć maksymalnie {1} znaków.")]
    [Display(Name = "Ofiara (opis)")]
    public string? Ofiara { get; set; }

    [Display(Name = "Zdjęcie ofiary")]
    public IFormFile? Zalacznik { get; set; }

    // Tylko przy edycji
    public string? ObecnyZalacznik { get; set; }

    [Display(Name = "Usuń obecne zdjęcie")]
    public bool UsunZalacznik { get; set; }

    public IEnumerable<SelectListItem> Bogowie { get; set; } = new List<SelectListItem>();
}

public class ModlitwaDetailsViewModel
{
    public Modlitwa Modlitwa { get; set; } = null!;
    public bool JestWlascicielem { get; set; }
    public bool MozeRozpatrywac { get; set; }

    public bool MozeEdytowac => JestWlascicielem && Modlitwa.Status == StatusModlitwy.Oczekujaca;
}

public class RozpatrzViewModel : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Wybierz decyzję.")]
    [Display(Name = "Decyzja")]
    public StatusModlitwy? Decyzja { get; set; }

    [StringLength(1000, ErrorMessage = "Odpowiedź może mieć maksymalnie {1} znaków.")]
    [Display(Name = "Odpowiedź boga")]
    public string? OdpowiedzBoga { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Decyzja == StatusModlitwy.Oczekujaca)
        {
            yield return new ValidationResult("Wybierz akceptację, odrzucenie lub karę.", new[] { nameof(Decyzja) });
        }
        if (Decyzja == StatusModlitwy.Ukarana && string.IsNullOrWhiteSpace(OdpowiedzBoga))
        {
            yield return new ValidationResult("Opisz karę, którą zsyłasz.", new[] { nameof(OdpowiedzBoga) });
        }
    }
}
