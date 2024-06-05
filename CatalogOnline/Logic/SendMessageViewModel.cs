using CatalogOnline.Models;
using System.ComponentModel.DataAnnotations;

namespace CatalogOnline.Logic
{
    public class SendMessageViewModel
    {
        [Required]
        [StringLength(500, ErrorMessage = "Mesajul nu poate avea mai mult de 500 de caractere.")]
        public string Mesaj { get; set; }

        public List<string>? CatedraList { get; set; }

        public List<Profesor>? ProfesorList { get; set; }

        public List<int>? ProfesorIdList { get; set; }

        public int? SecretarId { get; set; } 
    }

}
