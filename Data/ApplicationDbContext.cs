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

    }
}
