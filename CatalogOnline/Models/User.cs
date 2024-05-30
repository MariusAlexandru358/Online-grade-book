
using CatalogOnline.Logic;

namespace CatalogOnline.Models
{
    public abstract class User
    {
        public int Id { get; set; }
        public string? Nume { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserType Rol { get; set; }
    }
}


