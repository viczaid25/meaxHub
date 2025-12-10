using MeaxHub.Data;
using MeaxHub.Models;
using MeaxHub.Models.Systems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]   // 👈 solo requiere que esté autenticado
public class SystemsController : Controller
{
    private readonly MeaxDbContext _db;

    public SystemsController(MeaxDbContext db)
    {
        _db = db;
    }

    // Helper para saber si el usuario es IT
    private async Task<bool> IsUserITAsync()
    {
        var dept = (User.FindFirst("Department")?.Value ?? "").Trim();
        var pcLoginId = User.FindFirst("PcLoginId")?.Value;

        // Si el claim viene vacío, buscamos en la BD
        if (string.IsNullOrEmpty(dept) && !string.IsNullOrEmpty(pcLoginId))
        {
            var userRow = await _db.MeaxAllUsers
                .FirstOrDefaultAsync(u => u.PcLoginId == pcLoginId);

            if (userRow != null && !string.IsNullOrWhiteSpace(userRow.Department))
            {
                dept = userRow.Department.Trim();
            }
        }

        // Misma lógica que en la sidebar
        return dept.Equals("IT", StringComparison.OrdinalIgnoreCase)
               || dept.StartsWith("IT ", StringComparison.OrdinalIgnoreCase);
    }

    // GET: /Systems/Create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!await IsUserITAsync())
            return RedirectToAction("AccessDenied", "Account");

        var vm = new SystemCreateViewModel
        {
            IsActive = true
        };
        return View(vm);
    }

    // POST: /Systems/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SystemCreateViewModel model)
    {
        if (!await IsUserITAsync())
            return RedirectToAction("AccessDenied", "Account");

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var code = model.Code.Trim().ToUpper();

        var exists = await _db.MeaxAllSystems
            .AnyAsync(s => s.Code == code);

        if (exists)
        {
            ModelState.AddModelError("Code", "Ya existe un sistema con este código.");
            return View(model);
        }

        var entity = new MeaxAllSystem
        {
            Code = code,
            Name = model.Name.Trim(),
            Url = model.Url.Trim(),
            IsActive = model.IsActive
        };

        _db.MeaxAllSystems.Add(entity);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Sistema registrado correctamente.";

        return RedirectToAction(nameof(Create));
    }
}
