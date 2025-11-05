using CurriculumVitaeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CurriculumVitaeApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ExperienciaLaboral> ExperienciaLaborals { get; set; }
        public DbSet<FormacionAcademica> FormacionAcademica { get; set; }
        public DbSet<Habilidad> Habilidades { get; set; }
        public DbSet<DatosBasicos> Perfil { get; set; }
        public DbSet<TipoInstitucion> tipoInstitucion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }   
    }
}
