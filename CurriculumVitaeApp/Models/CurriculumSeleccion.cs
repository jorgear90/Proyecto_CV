using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumVitaeApp.Models
{
    public partial class CurriculumSeleccion
    {
        [Key]
        public int Id { get; set; }
        public int TipoDatoCurriculumID { get; set; }
        public int TipoDatoID { get; set; }
        public int CurriculumID { get; set; }
        //public int Orden { get; set; }

        //Clave foranea
        [ForeignKey(nameof(CurriculumID))]
        public virtual Curriculum? Curriculums { get; set; }
        [ForeignKey(nameof(TipoDatoCurriculumID))]
        public virtual TipoDatoCurriculum? TiposDatosCurriculums { get; set; }
    }
}
