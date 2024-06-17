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

// Configuraci�n de ASP.NET Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Opciones de configuraci�n de ASP.NET Identity
    options.SignIn.RequireConfirmedAccount = false; // No requiere cuenta confirmada para iniciar sesi�n
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI() 
    .AddDefaultTokenProviders() // Proveedores de tokens por defecto
    .AddRoles<IdentityRole>(); // Agregar roles

// Configuraci�n de cookies de aplicaci�n
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Ruta de inicio de sesi�n
    options.LogoutPath = "/Account/Logout"; // Ruta de cierre de sesi�n
    options.AccessDeniedPath = "/Account/AccessDenied"; // Ruta de acceso denegado
    options.SlidingExpiration = true; // Expiraci�n deslizante
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Crear roles si no existen
    CreateRoles(roleManager).Wait();
}

// Configuraci�n de middleware de aplicaci�n
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // P�gina de error detallada en desarrollo
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Ruta de manejo de errores en producci�n
    app.UseHsts(); // Strict Transport Security en producci�n
}

app.UseHttpsRedirection(); // Redirecci�n HTTP a HTTPS
app.UseStaticFiles(); // Servir archivos est�ticos desde wwwroot

app.UseRouting(); // Enrutamiento

// Configuraci�n de autenticaci�n y autorizaci�n
app.UseAuthentication(); // Middleware de autenticaci�n
app.UseAuthorization(); // Middleware de autorizaci�n

// Configuraci�n de rutas por defecto para controladores MVC
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

// M�todo para crear roles
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
