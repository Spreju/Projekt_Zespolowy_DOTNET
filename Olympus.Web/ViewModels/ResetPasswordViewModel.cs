using System.ComponentModel.DataAnnotations;
namespace Olympus.Web.ViewModels;
public class ResetPasswordViewModel
{
    [Required]
    public string Email {get;set;} = string.Empty;
    public string Token {get;set;} = string.Empty;
    [Required(ErrorMessage = "Nowe hasło jest wymagane")]
    [DataType(DataType.Password)]
    [Display(Name = "Nowe hasło")]
    public string Password {get;set;} = string.Empty;
    [Required(ErrorMessage = "Potwierdź nowe hasło")]
    [DataType(DataType.Password)]
    [Display(Name = "Potwierdź hasło")]
    [Compare(nameof(Password),ErrorMessage = " Podane hasła muszą być identyczne")]
    public string ConfirmPassword {get;set;} = string.Empty;
}