using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CurriculumVitaeApp.Models
{
    public partial class DatosBasicos
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioID { get; set; }
        [Display(Name = "Dato")]
        public string Nombre { get; set; }
        [Display(Name = "Valor")]
        public string Descripcion { get; set; }

        //Clave foranea
        [ForeignKey(nameof(UsuarioID))]
        public virtual Usuario? Usuarios { get; set; }
    }
}
