using System.IO;
using MeaxHub.Data;
using MeaxHub.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1) DataProtection compartido con otros sistemas (RSQ, etc.)
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\DataProtectionKeys\MeaxAuth"))
    .SetApplicationName("MeaxAuth");

// 2) EF Core - BD del Hub (meax_all_*)
builder.Services.AddDbContext<MeaxDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MeaxDb")));

// 3) Autenticación por cookie - ESQUEMA ÚNICO "MeaxAuth"
builder.Services
    .AddAuthentication("MeaxAuth")
    .AddCookie("MeaxAuth", options =>
    {
        options.Cookie.Name = ".MEAX.AUTH";
        options.Cookie.Path = "/";

        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Intranet HTTP
        options.Cookie.SameSite = SameSiteMode.Lax;

        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.AddAuthorization();


// Servicio LDAP (si lo estás usando)
builder.Services.AddScoped<ILdapAuthService, LdapAuthService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Puedes comentar esto si solo usas HTTP interno.
// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
