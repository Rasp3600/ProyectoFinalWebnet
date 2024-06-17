using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalWebnet.Models;

namespace ProyectoFinalWebnet.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categorias> Categorias { get; set; }
        public DbSet<Libros> Libros { get; set; }
        public DbSet<Prestamos> Prestamos { get; set; }

        public DbSet<Usuarios> Usuarios { get; set; }

        public DbSet<HistorialPrestamos> HistorialPrestamos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuarios>()
          .Property(u => u.ID)
          .ValueGeneratedOnAdd();

            // Configurar la relación Prestamos - Libros
            modelBuilder.Entity<Prestamos>()
                 .HasOne(p => p.Libros)
                 .WithMany()
                 .HasForeignKey(p => p.LibroID)
                 .OnDelete(DeleteBehavior.Restrict); // Opcional: Define la acción de eliminación

         

        }
    }
}
