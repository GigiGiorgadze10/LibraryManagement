using System;
using System.Collections.Generic;

namespace LibraryManagement.Domain.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public virtual ICollection<Book> Books { get; set; }
        public Author() { Books = new HashSet<Book>(); }
    }
}