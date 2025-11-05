using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumVitaeApp.Models
{
    public partial class Habilidad
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioID { get; set; }
        public string Descripcion { get; set; }

        //Clave foranea
        [ForeignKey(nameof(UsuarioID))]
        public virtual Usuario? Usuarios { get; set; }
    }
}
