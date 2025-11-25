using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumVitaeApp.Models
{
    public partial class FormacionAcademica
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioID { get; set; }
        [Display(Name = "Tipo de institución")]
        public int TipoInstitucionID { get; set; }
        [Display(Name = "Nombre de la institución")]
        public string NombreInstitucion { get; set; }
        public string Carrera { get; set; }
        [Display(Name = "Año de inicio")]
        public int AnhoInicio { get; set; }
        [Display(Name = "Año de termino")]
        public int? AnhoTermino { get; set; }
        public bool Vigente { get; set; }
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        //Clave foranea
        [ForeignKey(nameof(TipoInstitucionID))]
        public virtual TipoInstitucion? TipoInstitucion { get; set; }

        [ForeignKey(nameof(UsuarioID))]
        public virtual Usuario? Usuarios { get; set; }
    }
}
