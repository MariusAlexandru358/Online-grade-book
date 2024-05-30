namespace CatalogOnline.Models
{
    public class NotificareNota
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Student Student { get; set; }

    }
}
