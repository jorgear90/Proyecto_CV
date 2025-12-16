using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumVitaeApp.Models
{
    public partial class EncabezadoCurriculum
    {
        public int Id { get; set; }
        public int UsuarioID { get; set; }
        public string ValorEncabezado { get; set; }

        //Clave foranea
        [ForeignKey(nameof(UsuarioID))]
        public virtual Usuario? Usuarios { get; set; }
    }
}
