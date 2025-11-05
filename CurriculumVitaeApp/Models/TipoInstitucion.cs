using System.ComponentModel.DataAnnotations;

namespace CurriculumVitaeApp.Models
{
    public partial class TipoInstitucion
    {
        [Key]
        public int ID { get; set; }
        public string Tipo {  get; set; }

        //Claves foraneas de salida
        public virtual ICollection<FormacionAcademica> FormacionAcademica { get; set; } = new List<FormacionAcademica>();
    }
}
