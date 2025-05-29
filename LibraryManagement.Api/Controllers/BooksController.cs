// src/LibraryManagementSystem.Api/Controllers/BooksController.cs
using LibraryManagement.Application.DTOs.BookDtos;
using LibraryManagement.Application.Services.Interfaces;
using LibraryManagement.Infrastructure.Identity; // For Roles
using System.Net;
//using System.Net.Http; // Not strictly needed for IHttpActionResult and Request.GetOwinContext()
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description; // For ApiResponseType
using System.Collections.Generic; // Added for IEnumerable in GetBooks response type
using Microsoft.Owin; // For Request.GetOwinContext()
using System.Net.Http;

namespace LibraryManagement.Api.Controllers
{
    [RoutePrefix("api/books")]
    public class BooksController : ApiController
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService) // Injected by Autofac
        {
            _bookService = bookService;
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)] // User or Admin
        [ResponseType(typeof(IEnumerable<BookReadDto>))] // Actual items returned
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
        [Route("{id:int}", Name = "GetBookById")] // Added route name for CreatedAtRoute
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        [ResponseType(typeof(BookReadDto))]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            // NotFoundException will be handled by ErrorHandlingMiddleware
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
            return CreatedAtRoute("GetBookById", new { id = createdBook.Id }, createdBook);
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IHttpActionResult> UpdateBook(int id, [FromBody] BookUpdateDto bookUpdateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != bookUpdateDto.Id) return BadRequest("ID mismatch in route and body.");

            await _bookService.UpdateBookAsync(bookUpdateDto);
            // NotFoundException or ValidationException handled by ErrorHandlingMiddleware
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IHttpActionResult> DeleteBook(int id)
        {
            await _bookService.DeleteBookAsync(id);
            // NotFoundException handled by ErrorHandlingMiddleware
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}