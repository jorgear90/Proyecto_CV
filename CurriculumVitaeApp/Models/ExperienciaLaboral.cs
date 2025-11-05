using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumVitaeApp.Models
{
    public partial class ExperienciaLaboral
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioID { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly? FechaTermino { get; set; }
        public string Vigente { get; set; }
        public string Empresa { get; set; }
        public string Descripcion { get; set; }

        //Clave foranea
        [ForeignKey(nameof(UsuarioID))]
        public virtual Usuario? Usuarios { get; set; }

    }
}
