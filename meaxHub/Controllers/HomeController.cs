using MeaxHub.Data;
using MeaxHub.Models.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeaxHub.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly MeaxDbContext _db;

        public HomeController(MeaxDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // Leemos de la tabla meax_all_system (entidad MeaxAllSystem)
            var systems = await _db.MeaxAllSystems
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .Select(s => new HomeSystemItem
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    Url = s.Url
                })
                .ToListAsync();

            var vm = new HomeViewModel
            {
                Systems = systems
            };

            return View(vm);
        }
    }
}
