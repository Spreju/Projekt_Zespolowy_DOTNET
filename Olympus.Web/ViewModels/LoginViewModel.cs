using System.ComponentModel.DataAnnotations;
namespace Olympus.Web.ViewModels;
public class LoginViewModel
{
    [Display(Name = "Adres e-mail")]
    [Required(ErrorMessage = "Email jest wymagany")]
    [EmailAddress(ErrorMessage ="Nieprawidłowy adres e-mail")]
    public string Email {get;set;} = string.Empty;
    [Display(Name = "Hasło")]
    [Required(ErrorMessage = "Hasło jest wymagane")]
    [DataType(DataType.Password)]
    public string Password {get;set;} = string.Empty;
    [Display(Name ="Zapamiętaj mnie")]
    public bool RememberMe {get;set;}
}