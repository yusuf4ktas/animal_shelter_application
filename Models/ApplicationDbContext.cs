using animal_shelter_app.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace animal_shelter_app.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Map user table to the User entity
        public DbSet<User> Users { get; set; }
        public DbSet<AnimalInformation> AnimalInformations { get; set; }
        public DbSet<AnimalHealthCondition> AnimalHealthConditions { get; set; }
        public DbSet<AnimalType> AnimalTypes { get; set; }
        public DbSet<Adoption> Adoptions { get; set; }
        public DbSet<ArticleInformations> ArticleInformations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Configuring the one-to-one relationship between AnimalInformation and AnimalHealthCondition
            modelBuilder.Entity<AnimalHealthCondition>()
                .HasOne(a => a.AnimalInformation)
                .WithOne(a => a.HealthCondition)
                .HasForeignKey<AnimalHealthCondition>(a => a.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AnimalInformation>()
                .HasOne(ai => ai.AnimalType)
                .WithOne(at => at.AnimalInformation)
                .HasForeignKey<AnimalType>(at => at.AnimalId);
        }
    }
}
