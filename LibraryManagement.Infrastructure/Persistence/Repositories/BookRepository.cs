using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using LibraryManagement.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Infrastructure.Persistence.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(AppDbContext context) : base(context)
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
            IQueryable<Book> query = DbSet.Include(b => b.Author).Include(b => b.Genre);

            // Apply Filtering
            if (minPages.HasValue) query = query.Where(b => b.Pages >= minPages.Value);
            if (maxPages.HasValue) query = query.Where(b => b.Pages <= maxPages.Value);
            if (genreId.HasValue) query = query.Where(b => b.GenreId == genreId.Value);
            if (authorId.HasValue) query = query.Where(b => b.AuthorId == authorId.Value);

            var totalCount = await query.CountAsync();

            IOrderedQueryable<Book> orderedQuery = null;

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                string sortByLower = sortBy.ToLowerInvariant();
                switch (sortByLower)
                {
                    case "title":
                        orderedQuery = sortDirection == SortDirection.Asc
                            ? query.OrderBy(b => b.Title)
                            : query.OrderByDescending(b => b.Title);
                        break;
                    case "pages":
                        orderedQuery = sortDirection == SortDirection.Asc
                            ? query.OrderBy(b => b.Pages)
                            : query.OrderByDescending(b => b.Pages);
                        break;
                    case "publicationyear":
                        orderedQuery = sortDirection == SortDirection.Asc
                            ? query.OrderBy(b => b.PublicationYear)
                            : query.OrderByDescending(b => b.PublicationYear);
                        break;
                }
            }

            if (orderedQuery != null)
            {
                query = orderedQuery.ThenBy(b => b.Id); 
            }
            else
            {
                query = sortDirection == SortDirection.Desc
                    ? query.OrderByDescending(b => b.Id) 
                    : query.OrderBy(b => b.Id);          
            }

            if (pageNumber < 1) pageNumber = 1;
            var books = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (books, totalCount);
        }
    }
}