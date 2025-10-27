using Microsoft.EntityFrameworkCore;

namespace CalculIndiceMasseCorpolle.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
              : base(options)
        {
        }

        public DbSet<CalculIMC> CalculsIMC { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalculIMC>(entity =>
            {
                entity.ToTable("CalculsIMC");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nom)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Poids)
                    .HasColumnType("decimal(5,2)");

                entity.Property(e => e.Taille)
                    .HasColumnType("decimal(3,2)");

                entity.Property(e => e.IMC)
                    .HasColumnType("decimal(5,2)");

                entity.Property(e => e.Categorie)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DateCalcul)
                    .HasColumnType("datetime2");
            });
        }
    
}
}
