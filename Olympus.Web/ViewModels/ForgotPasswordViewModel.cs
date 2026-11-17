using System.ComponentModel.DataAnnotations;
namespace Olympus.Web.ViewModels;
public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Adres e-mail jest wymagany")]
    [EmailAddress(ErrorMessage = " Podaj poprawny adres e-mail")]
    [Display(Name = "Adres e-mail")]
    public string Email{get;set;} = string.Empty;
}