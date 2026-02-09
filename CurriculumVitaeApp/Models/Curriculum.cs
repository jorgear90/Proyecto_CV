using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumVitaeApp.Models
{
    public partial class Curriculum
    {
        public int Id { get; set; }
        public int UsuarioID { get; set; }
        public string Nombre { get; set; }
        public string Encabezado { get; set; }

        //Clave foranea
        [ForeignKey(nameof(UsuarioID))]
        public virtual Usuario? Usuarios { get; set; }

        public virtual ICollection<CurriculumSeleccion> CurriculumsSelecciones { get; set; } = new List<CurriculumSeleccion>();
    }
}
