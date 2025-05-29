// src/LibraryManagementSystem.Infrastructure/Persistence/Configurations/GenreConfiguration.cs
using LibraryManagement.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LibraryManagement.Infrastructure.Persistence.Configurations
{
    public class GenreConfiguration : EntityTypeConfiguration<Genre>
    {
        public GenreConfiguration()
        {
            ToTable("Genres");

            HasKey(g => g.Id);

            Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Relationship: Genre has many Books (implicitly configured via Book.Genre)
        }
    }
}