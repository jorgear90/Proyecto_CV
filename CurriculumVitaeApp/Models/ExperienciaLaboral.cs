using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumVitaeApp.Models
{
    public partial class ExperienciaLaboral
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioID { get; set; }
        [Display(Name = "Fecha de inicio")]
        public DateOnly FechaInicio { get; set; }
        [Display(Name = "Fecha de termino")]
        public DateOnly? FechaTermino { get; set; }
        public bool Vigente { get; set; }
        public string Empresa { get; set; }
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        //Clave foranea
        [ForeignKey(nameof(UsuarioID))]
        public virtual Usuario? Usuarios { get; set; }

    }
}
