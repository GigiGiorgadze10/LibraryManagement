// src/LibraryManagementSystem.Infrastructure/Persistence/Repositories/BookRepository.cs
using LibraryManagement.Infrastructure.Persistence.Repositories;
using LibraryManagement.Infrastructure.Persistence;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity; // For .Include()
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LibraryManagement.Infrastructure.Persistence.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(AppDbContext context) : base(context) // Pass AppDbContext
        {
        }

        public async Task<(IEnumerable<Book> Books, int TotalCount)> GetBooksAsync(
            int pageNumber,
            int pageSize,
            int? minPages,
            int? maxPages,
            int? genreId,
            int? authorId,
            string sortBy,
            SortDirection sortDirection)
        {
            var query = DbSet.Include(b => b.Author).Include(b => b.Genre).AsQueryable(); // Eager load

            // Filtering
            if (minPages.HasValue)
                query = query.Where(b => b.Pages >= minPages.Value);
            if (maxPages.HasValue)
                query = query.Where(b => b.Pages <= maxPages.Value);
            if (genreId.HasValue)
                query = query.Where(b => b.GenreId == genreId.Value);
            if (authorId.HasValue)
                query = query.Where(b => b.AuthorId == authorId.Value);

            var totalCount = await query.CountAsync();

            // Sorting - C# 7.3 compatible
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                string sortByLower = sortBy.ToLowerInvariant();
                if (sortByLower == "title")
                {
                    query = sortDirection == SortDirection.Asc ? query.OrderBy(b => b.Title) : query.OrderByDescending(b => b.Title);
                }
                else if (sortByLower == "pages")
                {
                    query = sortDirection == SortDirection.Asc ? query.OrderBy(b => b.Pages) : query.OrderByDescending(b => b.Pages);
                }
                else if (sortByLower == "publicationyear")
                {
                    query = sortDirection == SortDirection.Asc ? query.OrderBy(b => b.PublicationYear) : query.OrderByDescending(b => b.PublicationYear);
                }
                else
                {
                    query = query.OrderBy(b => b.Id); // Default sort
                }
            }
            else
            {
                query = query.OrderBy(b => b.Id); // Default sort if sortBy is not provided
            }

            // Pagination
            var books = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (books, totalCount);
        }
    }
}