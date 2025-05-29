using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Domain.Interfaces
{
    public interface IGenreRepository : IRepository<Genre>
    {
        // Add Genre-specific methods if any
    }
}
