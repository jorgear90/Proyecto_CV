using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CurriculumVitaeApp.Models
{
    public partial class DatosBasicos
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        //Clave foranea
        [ForeignKey(nameof(UsuarioID))]
        public virtual Usuario? Usuarios { get; set; }
    }
}
