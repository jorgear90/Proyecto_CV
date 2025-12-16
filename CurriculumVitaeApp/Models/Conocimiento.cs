using CurriculumVitaeApp.Helpers;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CurriculumVitaeApp.Models
{
    public partial class Conocimiento
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioID { get; set; }
        [Required(ErrorMessage = MensajesValidacion.CampoObligatorio)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        //Clave foranea
        [ForeignKey(nameof(UsuarioID))]
        public virtual Usuario? Usuarios { get; set; }
    }
}
