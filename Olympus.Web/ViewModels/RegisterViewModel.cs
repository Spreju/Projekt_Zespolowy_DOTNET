using System.ComponentModel.DataAnnotations;
namespace Olympus.Web.ViewModels;
public class RegisterViewModel
{
    [Display(Name = "Nazwa uytkownika")]
    [Required(ErrorMessage = "Nazwa uytkownika jest wymagana")]
    public string DisplayName {get;set;} = string.Empty;
    [Display(Name = "Adres e-mail")]
    [Required(ErrorMessage = "Adres e-mail jest wymagany")]
    [EmailAddress(ErrorMessage = "Nieprawidłowy adres e-mail")]
    public string Email {get;set;} = string.Empty;
    [Display(Name ="Hasło")]
    [Required(ErrorMessage ="Hasło jest wymagane")]
    [DataType(DataType.Password)]
    public string Password {get;set;} = string.Empty;
    [Display(Name = "Potwierdź hasło")]
    [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane")]
    [DataType(DataType.Password)]
    [Compare ("Password",ErrorMessage = "Hasła muszą być identyczne")]
    public string ConfirmPassword {get;set;} = string.Empty;
}