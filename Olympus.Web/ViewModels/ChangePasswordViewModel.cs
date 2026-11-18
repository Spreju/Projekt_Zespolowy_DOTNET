using System.ComponentModel.DataAnnotations;
namespace Olympus.Web.ViewModels;
public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Podaj aktualne hasło.")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Podaj nowe hasło.")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Potwierdź nowe hasło.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Hasła muszą być identyczne.")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}