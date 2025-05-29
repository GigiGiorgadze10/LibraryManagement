// src/LibraryManagementSystem.Infrastructure/Persistence/Configurations/AuthorConfiguration.cs
using LibraryManagement.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LibraryManagement.Infrastructure.Persistence.Configurations
{
    public class AuthorConfiguration : EntityTypeConfiguration<Author>
    {
        public AuthorConfiguration()
        {
            ToTable("Authors"); // Explicitly set table name

            HasKey(a => a.Id);

            Property(a => a.FullName)
                .IsRequired()
                .HasMaxLength(150);

            Property(a => a.BirthDate)
                .IsRequired();

            // Relationship: Author has many Books
            HasMany(a => a.Books)
                .WithRequired(b => b.Author) // Book requires an Author
                .HasForeignKey(b => b.AuthorId)
                .WillCascadeOnDelete(false); // Prevent cascade delete if an author is deleted
        }
    }
}