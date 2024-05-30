using System.ComponentModel.DataAnnotations;

namespace CatalogOnline.Models
{
    public class ProfesorMaterieStudent
    {
        public int Id { get; set; }
        public int ProfesorId { get; set; }
        public int MaterieId { get; set; }
        public int StudentId { get; set; }

        
        [Range(1, 4, ErrorMessage = "Anul trebuie sa fie intre 1 si 4.")]
        public int An { get; set; }

        [Range(1, 2, ErrorMessage = "Semestrul trebuie sa fie 1 sau 2")]
        public int Semestru { get; set; }


        [Range(1, 10, ErrorMessage = "Nota trebuie sa fie intre 1 si 10")]
        public int? Nota { get; set; }

        public virtual Profesor Profesor { get; set; }
        public virtual Materie Materie { get; set; }
        public virtual Student Student { get; set; }
    }
}
