namespace CatalogOnline.Models
{
    public class Nota
    {
        public int Id { get; set; }
        public int ProfesorMaterieStudentId { get; set; }
        public double? Valoare { get; set; }
        public virtual ProfesorMaterieStudent ProfesorMaterieStudent { get; set; }

    }
}
