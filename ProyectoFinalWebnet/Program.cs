using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProyectoFinalWebnet.Data;
using ProyectoFinalWebnet.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllersWithViews(options =>
{
  
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de ASP.NET Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Opciones de configuración de ASP.NET Identity
    options.SignIn.RequireConfirmedAccount = false; // No requiere cuenta confirmada para iniciar sesión
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI() 
    .AddDefaultTokenProviders() // Proveedores de tokens por defecto
    .AddRoles<IdentityRole>(); // Agregar roles

// Configuración de cookies de aplicación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Ruta de inicio de sesión
    options.LogoutPath = "/Account/Logout"; // Ruta de cierre de sesión
    options.AccessDeniedPath = "/Account/AccessDenied"; // Ruta de acceso denegado
    options.SlidingExpiration = true; // Expiración deslizante
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Crear roles si no existen
    CreateRoles(roleManager).Wait();
}

// Configuración de middleware de aplicación
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Página de error detallada en desarrollo
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Ruta de manejo de errores en producción
    app.UseHsts(); // Strict Transport Security en producción
}

app.UseHttpsRedirection(); // Redirección HTTP a HTTPS
app.UseStaticFiles(); // Servir archivos estáticos desde wwwroot

app.UseRouting(); // Enrutamiento

// Configuración de autenticación y autorización
app.UseAuthentication(); // Middleware de autenticación
app.UseAuthorization(); // Middleware de autorización

// Configuración de rutas por defecto para controladores MVC
app.MapControllerRoute(
    name: "miPrestamo",
    pattern: "Prestamos/miPrestamo",
    defaults: new { controller = "Prestamos", action = "MiPrestamo" });

app.MapControllerRoute(
       name: "detailuser",
       pattern: "{controller=Usuarios}/{action=Detailuser}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run(); 

// Método para crear roles
async Task CreateRoles(RoleManager<IdentityRole> roleManager)
{
    if (!await roleManager.RoleExistsAsync("User"))
    {
        var role = new IdentityRole { Name = "User" };
        await roleManager.CreateAsync(role);
    }

    if (!await roleManager.RoleExistsAsync("Manager"))
    {
        var role = new IdentityRole { Name = "Manager" };
        await roleManager.CreateAsync(role);
    }
}
