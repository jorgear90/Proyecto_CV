using CurriculumVitaeApp.Helpers;
using System.ComponentModel.DataAnnotations;

namespace CurriculumVitaeApp.Models
{
    public partial class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = MensajesValidacion.CampoObligatorio)]
        public string Correo { get; set; }
        [Required(ErrorMessage = MensajesValidacion.CampoObligatorio)]
        public string Password { get; set; }

        //Claves foraneas de salida
        public virtual ICollection<DatosBasicos> DatosBasicos { get; set; } = new List<DatosBasicos>();
        public virtual ICollection<ExperienciaLaboral> ExperienciaLaboral { get; set; } = new List<ExperienciaLaboral>();
        public virtual ICollection<FormacionAcademica> FormacionAcademica { get; set; } = new List<FormacionAcademica>();
        public virtual ICollection<Habilidad> Habilidades { get; set; } = new List<Habilidad>();
        public virtual ICollection<Conocimiento> Conocimientos { get; set; } = new List<Conocimiento>();
        public virtual ICollection<Curriculum> Curriculums { get; set; } = new List<Curriculum>();
        public virtual ICollection<EncabezadoCurriculum> Encabezados { get; set; } = new List<EncabezadoCurriculum>();
        public virtual ICollection<Link> Links { get; set; } = new List<Link>();
    }
}
