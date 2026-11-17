using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Olympus.DATA.Identity;
using Olympus.Web.Services;
using Olympus.Web.ViewModels;

namespace Olympus.Web.Controllers;

// Moduł Wyroczni – dostęp dla Wyroczni, bogów i Zeusa
[Authorize(Roles = UserRoles.Oracle + "," + UserRoles.God + "," + UserRoles.Zeus)]
public class OracleController : Controller
{
    private readonly IOracleService _oracleService;

    public OracleController(IOracleService oracleService)
    {
        _oracleService = oracleService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new OracleViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(OracleViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            model.Reading = await _oracleService.ConsultAsync(model.City, cancellationToken);
            TempData["Success"] = "Wyrocznia odczytała znaki.";
        }
        catch (OracleException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        return View(model);
    }
}
