using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Domain.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<(IEnumerable<Book> Books, int TotalCount)> GetBooksAsync(
            int pageNumber,
            int pageSize,
            int? minPages,
            int? maxPages,
            int? genreId,
            int? authorId,
            string sortBy,
            SortDirection sortDirection);
    }
}
