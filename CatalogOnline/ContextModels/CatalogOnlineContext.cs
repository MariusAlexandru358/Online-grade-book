using CatalogOnline.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CatalogOnline.ContextModels
{
    public class CatalogOnlineContext : DbContext
    {
        public DbSet<Student> Student { get; set; }
        public DbSet<Profesor> Profesor { get; set; }
        public DbSet<Materie> Materie { get; set; }
        public DbSet<ProfesorMaterieStudent> ProfesorMaterieStudent { get; set; }
        public DbSet<Nota> Nota { get; set; }
        public DbSet<NotificareNota> NotificareNota { get; set; }

        public CatalogOnlineContext(DbContextOptions<CatalogOnlineContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProfesorMaterieStudent>()
                .HasKey(pms => pms.Id);

            modelBuilder.Entity<Nota>()
                .HasKey(n => n.Id);

            modelBuilder.Entity<Nota>()
                .HasOne(n => n.ProfesorMaterieStudent)
                .WithMany()
                .HasForeignKey(n => n.ProfesorMaterieStudentId);
        }
    }
}
