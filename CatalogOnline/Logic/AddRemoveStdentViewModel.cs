using CatalogOnline.Models;

namespace CatalogOnline.Logic
{
    public class AddRemoveStudentViewModel
    {
        public int An { get; set; }
        public int Semestru { get; set; }
        public int MaterieId { get; set; }
        public int ProfesorId { get; set; }
        public Materie Materie { get; set; }
        public Profesor Profesor { get; set; }
        public List<Student> StudentList { get; set; }
    }

}
