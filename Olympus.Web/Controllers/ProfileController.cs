using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Olympus.Data.Identity;
using Olympus.DATA.Identity;
using Olympus.Web.ViewModels;
namespace Olympus.Web.Controllers;
public class ProfileController : Controller
{
    
    private readonly UserManager<ApplicationUser> _userManager;
    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    [Authorize(Roles = UserRoles.Mortal)]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }
        var roles = await _userManager.GetRolesAsync(user);
        var model = new ProfileViewModel
        {
            DisplayName = user.DisplayName,
            Email = user.Email ?? string.Empty,
            Roles = roles
        };
        return View(model);
    }
}