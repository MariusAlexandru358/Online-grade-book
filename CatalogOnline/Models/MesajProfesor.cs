namespace CatalogOnline.Models
{
    public class MesajProfesor
    {
        public int Id { get; set; }
        public int ProfesorId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Profesor Profesor { get; set; }
    }
}
