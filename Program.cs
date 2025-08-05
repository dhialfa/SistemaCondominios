using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SistemaCondominios.Data;
using SistemaCondominios.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 🔐 Autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
    });

// 🧠 Sesiones
builder.Services.AddSession();

// DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ✅ Seeders para roles y zonas comunes
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Seed Roles
    if (!context.Roles.Any())
    {
        context.Roles.AddRange(
            new Rol { Nombre = "SuperAdministrador" },
            new Rol { Nombre = "AdministradorCondominal" },
            new Rol { Nombre = "Guarda" },
            new Rol { Nombre = "Condómino" }
        );
        context.SaveChanges();
    }

    // Seed Zonas Comunes
    if (!context.ZonasComunes.Any())
    {
        context.ZonasComunes.AddRange(
            new ZonaComun { Nombre = "Piscina", Descripcion = "Zona recreativa con piscina", Ubicacion = "Bloque A - Planta Baja" },
            new ZonaComun { Nombre = "Gimnasio", Descripcion = "Área equipada para entrenamiento físico", Ubicacion = "Bloque B - Segundo Piso" },
            new ZonaComun { Nombre = "Sala de Eventos", Descripcion = "Espacio para reuniones y fiestas", Ubicacion = "Centro Comunal" },
            new ZonaComun { Nombre = "Cancha Multiusos", Descripcion = "Para fútbol, baloncesto, voleibol...", Ubicacion = "Zona Este del Condominio" },
            new ZonaComun { Nombre = "Rancho BBQ", Descripcion = "Rancho techado con parrilla y mesas", Ubicacion = "Área Verde junto a la piscina" }
        );
        context.SaveChanges();
    }

}

app.Run();
