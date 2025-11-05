using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumVitaeApp.Models
{
    public partial class FormacionAcademica
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioID { get; set; }
        public int TipoInstitucionID { get; set; }
        public string Carrera { get; set; }
        public int AnhoInicio { get; set; }
        public int? AnhoTermino { get; set; }
        public string? Vigente { get; set; }

        //Clave foranea
        [ForeignKey(nameof(TipoInstitucionID))]
        public virtual TipoInstitucion? TipoInstitucion { get; set; }

        [ForeignKey(nameof(UsuarioID))]
        public virtual Usuario? Usuarios { get; set; }
    }
}
