using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Domain.Interfaces
{
    public interface IAuthorRepository : IRepository<Author>
    {
        // Add Author-specific methods if any, e.g., GetAuthorsWithBooksAsync
    }
}
