using CurriculumVitaeApp.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumVitaeApp.Models
{
    public partial class Link
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioID { get; set; }
        [Required(ErrorMessage = MensajesValidacion.CampoObligatorio)]
        public string Titulo { get; set; }
        [Required(ErrorMessage = MensajesValidacion.CampoObligatorio)]
        [Url(ErrorMessage = "El campo Enlace debe ser una URL válida")]
        public string Enlace { get; set; }

        [ForeignKey(nameof(UsuarioID))]
        public virtual Usuario? Usuarios { get; set; }
    }
}
