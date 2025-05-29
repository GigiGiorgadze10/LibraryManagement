// src/LibraryManagement.Api/Controllers/BooksController.cs
using LibraryManagement.Application.DTOs.BookDtos;
using LibraryManagement.Application.Services.Interfaces;
using LibraryManagement.Infrastructure.Identity; // For Roles
using System; // For ArgumentNullException
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description; // For ApiResponseType
using System.Collections.Generic; // For IEnumerable
using Microsoft.Owin; // For IOwinContext
using System.Net.Http; // For Request.GetOwinContext()

namespace LibraryManagement.Api.Controllers
{
    [RoutePrefix("api/books")]
    public class BooksController : ApiController
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService) // Injected by Autofac
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)] // User or Admin
        [ResponseType(typeof(IEnumerable<BookReadDto>))] // Actual items returned in body
        public async Task<IHttpActionResult> GetBooks([FromUri] BookFilterDto filter)
        {
            if (filter == null) filter = new BookFilterDto(); // Default filter
            var result = await _bookService.GetBooksAsync(filter);

            // Add pagination headers as per PDF (X-Pagination)
            IOwinContext owinContext = Request.GetOwinContext();
            if (owinContext != null) // Check if OWIN context is available
            {
                owinContext.Response.Headers.Add("X-Pagination-TotalCount", new[] { result.TotalCount.ToString() });
                owinContext.Response.Headers.Add("X-Pagination-TotalPages", new[] { result.TotalPages.ToString() });
                owinContext.Response.Headers.Add("X-Pagination-CurrentPage", new[] { result.CurrentPage.ToString() });
                owinContext.Response.Headers.Add("X-Pagination-PageSize", new[] { result.PageSize.ToString() });
            }
            return Ok(result.Items); // Return only the items in the body
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetBookById")] // Named route for CreatedAtRoute
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        [ResponseType(typeof(BookReadDto))]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            // Assuming NotFoundException is handled by ErrorHandlingMiddleware
            return Ok(book);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = Roles.Admin)] // Only Admin
        [ResponseType(typeof(BookReadDto))]
        public async Task<IHttpActionResult> CreateBook([FromBody] BookCreateDto bookCreateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var createdBook = await _bookService.CreateBookAsync(bookCreateDto);
            // Assuming exceptions from service (like validation or not found for author/genre)
            // are handled by ErrorHandlingMiddleware
            return CreatedAtRoute("GetBookById", new { id = createdBook.Id }, createdBook);
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> UpdateBook(int id, [FromBody] BookUpdateDto bookUpdateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != bookUpdateDto.Id) return BadRequest("ID mismatch in route and body.");

            // Assuming exceptions from service are handled by ErrorHandlingMiddleware
            await _bookService.UpdateBookAsync(bookUpdateDto);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteBook(int id)
        {
            // Assuming exceptions from service are handled by ErrorHandlingMiddleware
            await _bookService.DeleteBookAsync(id);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}