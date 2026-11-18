using Olympus.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Olympus.Data.Identity;
using Olympus.DATA.Identity;
using Olympus.Web.ViewModels;

namespace Olympus.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IPasswordResetEmailSender _emailSender;
    public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser>signInManager,IPasswordResetEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }
    [HttpGet]
    
    public IActionResult Login(string? returnUrl = null)
    {
        if(User.Identity ?.IsAuthenticated == true)
        {
            return RedirectToAction("Index","Home");
        }
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model,string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var result = await _signInManager.PasswordSignInAsync(model.Email,model.Password,model.RememberMe,lockoutOnFailure:false);
        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
            return RedirectToAction("Index","Home");
        }
        ModelState.AddModelError(string.Empty,"Nieprawidłowy adres e-mail lub hasło");

        return View(model);
    }
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string? email, string? token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
        {
            return BadRequest("Link do resetowania hasła jest nieprawidłowy.");
        }
        var model  = new ResetPasswordViewModel
        {
            Email = email,
            Token = token
        };
        return View(model);
    }
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if(!ModelState.IsValid)
        {
            return View(model);
        }
        var user = await _userManager.FindByEmailAsync(model.Email);
        if(user == null)
        {
            ModelState.AddModelError(string.Empty,"Nie udało się zresetować hasła. Link może być nieprawidłowy lub wygasły.");
            return View(model);
        }
        var result = await _userManager.ResetPasswordAsync(user,model.Token, model.Password);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }
        ModelState.AddModelError(string.Empty, "Nie udało się zresetować hasła. Sprawdź poprawność linku i wymagania dotyczące hasła.");
        return View(model);
    }
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if(!ModelState.IsValid)
        {
            return View(model);
        }
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        } 
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = Url.Action(action: "ResetPassword",controller: "Account",values: new { email = user.Email,token = token}, protocol: Request.Scheme);
        if (string.IsNullOrEmpty(resetLink))
        {
            throw new InvalidOperationException("Nie udało się wygenerować linku resetowania hasła.");
        }
        await _emailSender.SendPasswordResetLinkAsync(user.Email!,resetLink);
        return RedirectToAction(nameof(ForgotPasswordConfirmation));
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]

    
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            DisplayName = model.DisplayName
        };
        var result = await _userManager.CreateAsync(user,model.Password);
        if (result.Succeeded)
        {
            var RoleResult = await _userManager.AddToRoleAsync(user,UserRoles.Mortal);
            if(!RoleResult.Succeeded)
            {
                var deleteResult = await _userManager.DeleteAsync(user);
                if(!deleteResult.Succeeded)
                {
                    throw new InvalidOperationException("Nie udało się usunąć konta po błędzie przypisywania roli");
                }
                ModelState.AddModelError(string.Empty,"Konto zostało utworzone ale nie udało się przypisać roli");
                return View(model);
            }
            TempData["SuccessMessage"] = "Konto zostało utworzone. Możesz się zalogować.";
            return RedirectToAction(nameof(Login));
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(
                string.Empty,
                error.Code switch
                {
                    "DuplicateEmail" or "DuplicateUserName" =>
                        "Konto z podanym adresem e-mail już istnieje.",

                    "PasswordTooShort" =>
                        "Hasło jest za krótkie.",

                    "PasswordRequiresDigit" =>
                        "Hasło musi zawierać cyfrę.",

                    "PasswordRequiresLower" =>
                        "Hasło musi zawierać małą literę.",

                    "PasswordRequiresUpper" =>
                        "Hasło musi zawierać wielką literę.",

                    "PasswordRequiresNonAlphanumeric" =>
                        "Hasło musi zawierać znak specjalny.",

                    _ => "Nie udało się utworzyć konta. Sprawdź podane dane."
                });
        }
        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
       await _signInManager.SignOutAsync();
       return RedirectToAction(nameof(Login)); 
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return Content("Brak dostępu. Nie masz uprawnień do tej strony");
    }
}
