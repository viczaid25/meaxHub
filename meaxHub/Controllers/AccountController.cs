using System.Security.Claims;
using MeaxHub.Data;
using MeaxHub.Models.Account;
using MeaxHub.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeaxHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly MeaxDbContext _db;
        private readonly ILdapAuthService _ldap;

        public AccountController(MeaxDbContext db, ILdapAuthService ldap)
        {
            _db = db;
            _ldap = ldap;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Validación simple de formulario
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                ViewBag.ErrorMessage = "Usuario y contraseña son obligatorios.";
                return View(model);
            }

            // ===== 1) VALIDAR CONTRA AD =====
            bool adOk;
            try
            {
                adOk = _ldap.ValidateUser(model.Username, model.Password);
            }
            catch
            {
                // Por si AD truena, que no se muera el login
                adOk = false;
            }

            if (!adOk)
            {
                ViewBag.ErrorMessage = "Usuario o contraseña incorrectos (AD).";
                return View(model);
            }

            // ===== 2) BUSCAR USUARIO EN meax_all_user =====
            var user = await _db.MeaxAllUsers
                .Include(u => u.UserSystems)
                .ThenInclude(us => us.System)
                .FirstOrDefaultAsync(u => u.PcLoginId == model.Username);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Su usuario no está registrado en MEAX Hub.";
                return View(model);
            }

            // ===== 3) CREAR CLAIMS Y COOKIE =====
            var deptNormalized = (user.Department ?? "").Trim();
            var displayName = user.Username ?? user.PcLoginId;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.PcLoginId),
                new Claim(ClaimTypes.Name, displayName),
                new Claim("PcLoginId", user.PcLoginId),
                new Claim("DisplayName", displayName),
                new Claim("Department", deptNormalized),   // 👈 IMPORTANTE
                new Claim("Position", user.Position ?? "")
            };

            var identity = new ClaimsIdentity(claims, "MeaxAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MeaxAuth", principal, new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            });

            // ===== 4) REDIRECCIONAR =====
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MeaxAuth");
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
