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
        public DbSet<ExperienciaLaboral> ExperienciaLaboral { get; set; }
        public DbSet<FormacionAcademica> FormacionAcademica { get; set; }
        public DbSet<Habilidad> Habilidades { get; set; }
        public DbSet<Conocimiento> Conocimientos { get; set; }
        public DbSet<DatosBasicos> DatosBasicos { get; set; }
        public DbSet<TipoInstitucion> TipoInstitucion { get; set; }
        public DbSet<Curriculum> Curriculum { get; set; }
        public DbSet<CurriculumSeleccion> CurriculumSeleccion { get; set; }
        public DbSet<EncabezadoCurriculum> Encabezados { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }   
        
    }
}
