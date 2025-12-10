using MeaxHub.Data;
using MeaxHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeaxHub.Controllers
{
    [Authorize] // Luego lo puedes restringir a admins
    public class AdminController : Controller
    {
        private readonly MeaxDbContext _db;

        public AdminController(MeaxDbContext db)
        {
            _db = db;
        }

        // Lista de usuarios con # de sistemas asignados
        public async Task<IActionResult> Users()
        {
            var users = await _db.MeaxAllUsers
                .Select(u => new
                {
                    u.Id,
                    u.PcLoginId,
                    u.Username,
                    SystemsCount = u.UserSystems.Count
                })
                .OrderBy(u => u.PcLoginId)
                .ToListAsync();

            return View(users);
        }

        // Editar sistemas asignados a un usuario
        [HttpGet]
        public async Task<IActionResult> EditUserSystems(int id)
        {
            var user = await _db.MeaxAllUsers
                .Include(u => u.UserSystems)
                .ThenInclude(us => us.System)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var allSystems = await _db.MeaxAllSystems
                .OrderBy(s => s.Name)
                .ToListAsync();

            var vm = new EditUserSystemsViewModel
            {
                UserId = user.Id,
                PcLoginId = user.PcLoginId,
                Username = user.Username
            };

            foreach (var sys in allSystems)
            {
                var link = user.UserSystems.FirstOrDefault(us => us.SystemId == sys.Id);

                vm.Systems.Add(new UserSystemItemViewModel
                {
                    SystemId = sys.Id,
                    Code = sys.Code,
                    Name = sys.Name,
                    Assigned = link != null,
                    Role = link?.Role
                });
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserSystems(EditUserSystemsViewModel model)
        {
            var user = await _db.MeaxAllUsers
                .Include(u => u.UserSystems)
                .FirstOrDefaultAsync(u => u.Id == model.UserId);

            if (user == null)
            {
                return NotFound();
            }

            // Diccionario de links actuales
            var currentLinks = user.UserSystems.ToDictionary(us => us.SystemId, us => us);

            foreach (var item in model.Systems)
            {
                bool shouldBeAssigned = item.Assigned;
                bool isAssigned = currentLinks.TryGetValue(item.SystemId, out var link);

                if (shouldBeAssigned && !isAssigned)
                {
                    // Crear nuevo registro
                    var newLink = new MeaxAllUserSystem
                    {
                        UserId = user.Id,
                        SystemId = item.SystemId,
                        Role = string.IsNullOrWhiteSpace(item.Role) ? null : item.Role
                    };
                    _db.MeaxAllUserSystems.Add(newLink);
                }
                else if (!shouldBeAssigned && isAssigned)
                {
                    // Eliminar registro
                    _db.MeaxAllUserSystems.Remove(link!);
                }
                else if (shouldBeAssigned && isAssigned)
                {
                    // Actualizar rol si cambió
                    link!.Role = string.IsNullOrWhiteSpace(item.Role) ? null : item.Role;
                    _db.MeaxAllUserSystems.Update(link);
                }
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Users));
        }
    }
}
