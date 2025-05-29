using LibraryManagement.Infrastructure.Identity;
using LibraryManagement.Infrastructure.Persistence;
using LibraryManagement.Domain.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework; 
using System;
using System.Linq;

namespace LibraryManagement.Infrastructure.Persistence
{
    public class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!roleManager.RoleExists(Roles.Admin))
            {
                roleManager.Create(new IdentityRole(Roles.Admin));
            }
            if (!roleManager.RoleExists(Roles.User))
            {
                roleManager.Create(new IdentityRole(Roles.User));
            }
            context.SaveChanges(); 

            var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
            if (userManager.FindByName("admin@library.com") == null)
            {
                var adminUser = new AppUser
                {
                    UserName = "admin@library.com",
                    Email = "admin@library.com",
                    EmailConfirmed = true
                };
                var result = userManager.Create(adminUser, "Admin@123"); 
                if (result.Succeeded)
                {
                    userManager.AddToRole(adminUser.Id, Roles.Admin);
                }
            }
            context.SaveChanges();

            if (!context.Genres.Any())
            {
                var genres = new[]
                {
                    new Genre { Name = "Science Fiction" },
                    new Genre { Name = "Fantasy" },
                    new Genre { Name = "Mystery" },
                    new Genre { Name = "Thriller" },
                    new Genre { Name = "Historical Fiction" }
                };
                context.Genres.AddRange(genres);
                context.SaveChanges();
            }

            if (!context.Authors.Any())
            {
                var authors = new[]
                {
                    new Author { FullName = "Isaac Asimov", BirthDate = new DateTime(1920, 1, 2) },
                    new Author { FullName = "J.R.R. Tolkien", BirthDate = new DateTime(1892, 1, 3) },
                    new Author { FullName = "Agatha Christie", BirthDate = new DateTime(1890, 9, 15) }
                };
                context.Authors.AddRange(authors);
                context.SaveChanges();
            }

            if (!context.Books.Any() && context.Authors.Any() && context.Genres.Any())
            {
                var author1 = context.Authors.Local.FirstOrDefault(a => a.FullName.Contains("Asimov")) ?? context.Authors.FirstOrDefault(a => a.FullName.Contains("Asimov"));
                var genre1 = context.Genres.Local.FirstOrDefault(g => g.Name.Contains("Science Fiction")) ?? context.Genres.FirstOrDefault(g => g.Name.Contains("Science Fiction"));

                var author2 = context.Authors.Local.FirstOrDefault(a => a.FullName.Contains("Tolkien")) ?? context.Authors.FirstOrDefault(a => a.FullName.Contains("Tolkien"));
                var genre2 = context.Genres.Local.FirstOrDefault(g => g.Name.Contains("Fantasy")) ?? context.Genres.FirstOrDefault(g => g.Name.Contains("Fantasy"));

                if (author1 != null && genre1 != null && author2 != null && genre2 != null)
                {
                    var books = new[]
                    {
                        new Book { Title = "Foundation", AuthorId = author1.Id, GenreId = genre1.Id, Pages = 255, PublicationYear = 1951 },
                        new Book { Title = "The Hobbit", AuthorId = author2.Id, GenreId = genre2.Id, Pages = 310, PublicationYear = 1937 }
                    };
                    context.Books.AddRange(books);
                    context.SaveChanges();
                }
            }
        }
    }
}