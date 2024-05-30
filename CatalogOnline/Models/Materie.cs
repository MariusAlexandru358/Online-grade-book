using System.ComponentModel.DataAnnotations;

namespace CatalogOnline.Models
{
    public class Materie
    {
        public int Id { get; set; }
        public string Nume { get; set; }

        public int? Credite { get; set; } //TODO logica de medie ponderata

    }
}
