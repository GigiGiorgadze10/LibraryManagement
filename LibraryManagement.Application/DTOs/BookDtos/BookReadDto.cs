namespace LibraryManagement.Application.DTOs.BookDtos
{
    public class BookReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; } 
        public string GenreName { get; set; }  
        public int Pages { get; set; }
        public int PublicationYear { get; set; }
        public int BookAge { get; set; }    
        public bool IsThick { get; set; }     
    }
}