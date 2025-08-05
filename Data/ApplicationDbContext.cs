using Microsoft.EntityFrameworkCore;
using SistemaCondominios.Models;

namespace SistemaCondominios.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        

        //PROYECTO
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Propietario> Propietarios { get; set; }
        public DbSet<ZonaComun> ZonasComunes { get; set; }
        public DbSet<ReservaZonaComun> ReservasZonasComunes { get; set; }
        public DbSet<Rol> Roles { get; set; }

    }
}
