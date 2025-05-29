using LibraryManagement.Infrastructure.Persistence.Repositories;
using LibraryManagement.Infrastructure.Persistence;
using LibraryManagement.Domain.Interfaces;
using System.Threading.Tasks;

namespace LibraryManagement.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IAuthorRepository Authors { get; }
        public IBookRepository Books { get; }
        public IGenreRepository Genres { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Authors = new AuthorRepository(_context);
            Books = new BookRepository(_context);   
            Genres = new GenreRepository(_context);  
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}