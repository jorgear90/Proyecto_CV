using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CurriculumVitaeApp.Helpers;

namespace CurriculumVitaeApp.Models
{
    public partial class DatosBasicos
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioID { get; set; }
        [Required(ErrorMessage = MensajesValidacion.CampoObligatorio)]
        [Display(Name = "Dato")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = MensajesValidacion.CampoObligatorio)]
        [Display(Name = "Valor")]
        public string Valor { get; set; }

        //Clave foranea
        [ForeignKey(nameof(UsuarioID))]
        public virtual Usuario? Usuarios { get; set; }
    }
}
