using System.ComponentModel.DataAnnotations;
using Olympus.Web.Services;

namespace Olympus.Web.ViewModels;

public class OracleViewModel
{
    [Display(Name = "Kraina (miasto)")]
    [Required(ErrorMessage = "Podaj nazwę krainy, o którą pytasz Wyrocznię")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nazwa krainy musi mieć od 2 do 100 znaków")]
    [RegularExpression(@"^[\p{L}\s\-']+$", ErrorMessage = "Nazwa krainy może zawierać tylko litery, spacje, myślniki i apostrofy")]
    public string City { get; set; } = string.Empty;

    // Wypełniane po udanym odczycie
    public OracleReading? Reading { get; set; }
}
