namespace CurriculumVitaeApp.Models
{
    public partial class TipoDatoCurriculum
    {
        public int Id { get; set; }
        public string NombreTipoDato { get; set; }

        public virtual ICollection<CurriculumSeleccion> CurriculumSeleccion { get; set; } = new List<CurriculumSeleccion>();
    }
}
