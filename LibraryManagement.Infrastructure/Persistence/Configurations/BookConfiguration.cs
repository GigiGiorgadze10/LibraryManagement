// src/LibraryManagementSystem.Infrastructure/Persistence/Configurations/BookConfiguration.cs
using LibraryManagement.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LibraryManagement.Infrastructure.Persistence.Configurations
{
    public class BookConfiguration : EntityTypeConfiguration<Book>
    {
        public BookConfiguration()
        {
            ToTable("Books");

            HasKey(b => b.Id);

            Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);

            Property(b => b.Pages)
                .IsRequired();

            Property(b => b.PublicationYear)
                .IsRequired();

            // Relationship: Book belongs to one Author (already configured in AuthorConfiguration implicitly via Author.Books)
            // Relationship: Book belongs to one Genre
            HasRequired(b => b.Genre)
                .WithMany(g => g.Books)
                .HasForeignKey(b => b.GenreId)
                .WillCascadeOnDelete(false);
        }
    }
}