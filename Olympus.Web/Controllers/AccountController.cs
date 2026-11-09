using Microsoft.AspNetCore.Mvc;
using Olympus.Web.ViewModels;
namespace Olympus.Web.Controllers;
public class AccountController: Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if(!ModelState.IsValid)
        {
            return View(model);
        }
        ModelState.AddModelError(string.Empty,"Logowanie nie jest jeszcze dostępne");
        return View(model);
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if(!ModelState.IsValid)
        {
            return View(model);
        }
        ModelState.AddModelError(string.Empty,"Rejstracja nie jest jeszcze dostępna");
        return View(model);
    }
}