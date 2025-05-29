using LibraryManagement.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace LibraryManagement.Infrastructure.Persistence.Configurations
{
    public class AuthorConfiguration : EntityTypeConfiguration<Author>
    {
        public AuthorConfiguration()
        {
            ToTable("Authors"); 

            HasKey(a => a.Id);

            Property(a => a.FullName)
                .IsRequired()
                .HasMaxLength(150);

            Property(a => a.BirthDate)
                .IsRequired();

            HasMany(a => a.Books)
                .WithRequired(b => b.Author) 
                .HasForeignKey(b => b.AuthorId)
                .WillCascadeOnDelete(false); 
        }
    }
}