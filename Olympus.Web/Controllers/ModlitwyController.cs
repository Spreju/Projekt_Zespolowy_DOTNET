using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Olympus.Core.Entities;
using Olympus.Core.Interfaces;
using Olympus.Data.Identity;
using Olympus.DATA.Identity;
using Olympus.Web.Services;
using Olympus.Web.ViewModels;

namespace Olympus.Web.Controllers;

[Authorize]
public class ModlitwyController : Controller
{
    private const string RoleBogowie = UserRoles.God + "," + UserRoles.Zeus;

    private readonly IModlitwaService _modlitwy;
    private readonly IPlikiService _pliki;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ModlitwyController> _logger;

    public ModlitwyController(
        IModlitwaService modlitwy,
        IPlikiService pliki,
        UserManager<ApplicationUser> userManager,
        ILogger<ModlitwyController> logger)
    {
        _modlitwy = modlitwy;
        _pliki = pliki;
        _userManager = userManager;
        _logger = logger;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // ───────────── ŚMIERTELNIK: lista własnych modlitw ─────────────

    [Authorize(Roles = UserRoles.Mortal)]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var lista = await _modlitwy.PobierzModlitwySmiertelnikaAsync(UserId);
        return View(lista);
    }

    // ───────────── ŚMIERTELNIK: składanie modlitwy ─────────────

    [Authorize(Roles = UserRoles.Mortal)]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new ModlitwaFormViewModel();
        await WypelnijBogowAsync(model);
        return View(model);
    }

    [Authorize(Roles = UserRoles.Mortal)]
    [HttpPost, ValidateAntiForgeryToken]
    [RequestSizeLimit(LocalPlikiService.MaksRozmiar + 1024 * 1024)]
    public async Task<IActionResult> Create(ModlitwaFormViewModel model)
    {
        WalidujPlik(model);
        if (!ModelState.IsValid)
        {
            await WypelnijBogowAsync(model);
            return View(model);
        }

        ZapisanyPlik? plik = null;
        try
        {
            plik = await ZapiszPlikAsync(model.Zalacznik);
            var user = await _userManager.GetUserAsync(User);
            var imie = !string.IsNullOrWhiteSpace(user?.DisplayName) ? user.DisplayName : User.Identity?.Name ?? string.Empty;
            await _modlitwy.ZlozAsync(UserId, imie, DoDanych(model), plik);

            TempData["SuccessMessage"] = "Twoja modlitwa została złożona. Bogowie wkrótce ją rozpatrzą.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            UsunOsieroconyPlik(plik);
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (Exception ex)
        {
            UsunOsieroconyPlik(plik);
            _logger.LogError(ex, "Nie udało się zapisać modlitwy");
            ModelState.AddModelError(string.Empty, "Wystąpił błąd podczas zapisu modlitwy. Spróbuj ponownie później.");
        }

        await WypelnijBogowAsync(model);
        return View(model);
    }

    // ───────────── ŚMIERTELNIK: edycja ─────────────

    [Authorize(Roles = UserRoles.Mortal)]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var modlitwa = await _modlitwy.PobierzAsync(id);
        if (modlitwa == null || modlitwa.Smiertelnik.UserId != UserId)
        {
            return NotFound();
        }
        if (modlitwa.Status != StatusModlitwy.Oczekujaca)
        {
            TempData["ErrorMessage"] = "Można edytować tylko modlitwy oczekujące na rozpatrzenie.";
            return RedirectToAction(nameof(Index));
        }

        var model = new ModlitwaFormViewModel
        {
            Id = modlitwa.Id,
            BogId = modlitwa.BogId,
            Tresc = modlitwa.Tresc,
            Ofiara = modlitwa.Ofiara,
            ObecnyZalacznik = modlitwa.ZalacznikNazwaOryginalna
        };
        await WypelnijBogowAsync(model);
        return View(model);
    }

    [Authorize(Roles = UserRoles.Mortal)]
    [HttpPost, ValidateAntiForgeryToken]
    [RequestSizeLimit(LocalPlikiService.MaksRozmiar + 1024 * 1024)]
    public async Task<IActionResult> Edit(int id, ModlitwaFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        WalidujPlik(model);
        if (!ModelState.IsValid)
        {
            await WypelnijBogowAsync(model);
            return View(model);
        }

        ZapisanyPlik? plik = null;
        try
        {
            plik = await ZapiszPlikAsync(model.Zalacznik);
            await _modlitwy.AktualizujAsync(id, UserId, DoDanych(model), plik, model.UsunZalacznik);

            TempData["SuccessMessage"] = "Modlitwa została zaktualizowana.";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            UsunOsieroconyPlik(plik);
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            UsunOsieroconyPlik(plik);
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (Exception ex)
        {
            UsunOsieroconyPlik(plik);
            _logger.LogError(ex, "Nie udało się zaktualizować modlitwy {Id}", id);
            ModelState.AddModelError(string.Empty, "Wystąpił błąd podczas zapisu zmian. Spróbuj ponownie później.");
        }

        await WypelnijBogowAsync(model);
        return View(model);
    }

    // ───────────── ŚMIERTELNIK: usuwanie ─────────────

    [Authorize(Roles = UserRoles.Mortal)]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var modlitwa = await _modlitwy.PobierzAsync(id);
        if (modlitwa == null || modlitwa.Smiertelnik.UserId != UserId)
        {
            return NotFound();
        }
        if (modlitwa.Status != StatusModlitwy.Oczekujaca)
        {
            TempData["ErrorMessage"] = "Można usunąć tylko modlitwy oczekujące na rozpatrzenie.";
            return RedirectToAction(nameof(Index));
        }
        return View(modlitwa);
    }

    [Authorize(Roles = UserRoles.Mortal)]
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _modlitwy.UsunAsync(id, UserId);
            TempData["SuccessMessage"] = "Modlitwa została usunięta.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Nie udało się usunąć modlitwy {Id}", id);
            TempData["ErrorMessage"] = "Wystąpił błąd podczas usuwania modlitwy.";
        }
        return RedirectToAction(nameof(Index));
    }

    // ───────────── BÓG / ZEUS: skrzynka modlitw ─────────────

    [Authorize(Roles = RoleBogowie)]
    [HttpGet]
    public async Task<IActionResult> Skrzynka()
    {
        var lista = await _modlitwy.PobierzModlitwyDlaBogaAsync(UserId, User.IsInRole(UserRoles.Zeus));
        return View(lista);
    }

    [Authorize(Roles = RoleBogowie)]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Rozpatrz(RozpatrzViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var bledy = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            TempData["ErrorMessage"] = string.Join(" ", bledy);
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        try
        {
            await _modlitwy.RozpatrzAsync(model.Id, UserId, User.IsInRole(UserRoles.Zeus),
                model.Decyzja!.Value, model.OdpowiedzBoga);
            TempData["SuccessMessage"] = "Decyzja została zapisana.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Nie udało się rozpatrzyć modlitwy {Id}", model.Id);
            TempData["ErrorMessage"] = "Wystąpił błąd podczas zapisu decyzji.";
        }
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    // ───────────── SZCZEGÓŁY i ZAŁĄCZNIK (właściciel, adresat-bóg lub Zeus) ─────────────

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var modlitwa = await _modlitwy.PobierzAsync(id);
        if (modlitwa == null || !MaDostep(modlitwa, out var wlasciciel, out var rozpatruje))
        {
            return NotFound();
        }

        return View(new ModlitwaDetailsViewModel
        {
            Modlitwa = modlitwa,
            JestWlascicielem = wlasciciel,
            MozeRozpatrywac = rozpatruje && modlitwa.Status == StatusModlitwy.Oczekujaca
        });
    }

    [HttpGet]
    public async Task<IActionResult> Zalacznik(int id)
    {
        var modlitwa = await _modlitwy.PobierzAsync(id);
        if (modlitwa == null || modlitwa.ZalacznikNazwaPliku == null || !MaDostep(modlitwa, out _, out _))
        {
            return NotFound();
        }

        var strumien = _pliki.Otworz(modlitwa.ZalacznikNazwaPliku);
        if (strumien == null)
        {
            return NotFound();
        }

        Response.Headers["X-Content-Type-Options"] = "nosniff";
        return File(strumien, modlitwa.ZalacznikTyp ?? "application/octet-stream");
    }

    // ───────────── pomocnicze ─────────────

    private bool MaDostep(Modlitwa m, out bool wlasciciel, out bool rozpatruje)
    {
        wlasciciel = m.Smiertelnik.UserId == UserId;
        var jestZeusem = User.IsInRole(UserRoles.Zeus);
        rozpatruje = jestZeusem || (User.IsInRole(UserRoles.God) && m.Bog.UserId == UserId);
        return wlasciciel || rozpatruje;
    }

    private async Task WypelnijBogowAsync(ModlitwaFormViewModel model)
    {
        var bogowie = await _modlitwy.PobierzBogowAsync();
        model.Bogowie = bogowie.Select(b => new SelectListItem
        {
            Value = b.Id.ToString(),
            Text = string.IsNullOrWhiteSpace(b.Domena) ? b.Imie : $"{b.Imie} ({b.Domena})",
            Selected = b.Id == model.BogId
        }).ToList();
    }

    private void WalidujPlik(ModlitwaFormViewModel model)
    {
        if (model.Zalacznik is { Length: > 0 } plik)
        {
            var blad = _pliki.Waliduj(plik.FileName, plik.Length);
            if (blad != null)
            {
                ModelState.AddModelError(nameof(model.Zalacznik), blad);
            }
        }
    }

    private async Task<ZapisanyPlik?> ZapiszPlikAsync(IFormFile? plik)
    {
        if (plik == null || plik.Length == 0)
        {
            return null;
        }
        await using var strumien = plik.OpenReadStream();
        return await _pliki.ZapiszAsync(strumien, plik.FileName, plik.Length);
    }

    // Jeśli zapis do bazy się nie udał, nie zostawiamy pliku "w próżni"
    private void UsunOsieroconyPlik(ZapisanyPlik? plik)
    {
        if (plik != null)
        {
            _pliki.Usun(plik.NazwaPliku);
        }
    }

    private static ModlitwaDane DoDanych(ModlitwaFormViewModel m) => new()
    {
        BogId = m.BogId,
        Tresc = m.Tresc,
        Ofiara = m.Ofiara
    };
}
