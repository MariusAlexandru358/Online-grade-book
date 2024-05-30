namespace CatalogOnline.Models
{
    public class Student : User
    {
        public int AnInscriere { get; set; }
        public string? Seria { get; set; }
        public int Grupa { get; set; }
        public int? AnStudiu { get; set; }
        public string? ProgramStudiu { get; set; }
    }
}
