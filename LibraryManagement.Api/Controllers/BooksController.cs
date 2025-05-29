using LibraryManagement.Application.DTOs.BookDtos;
using LibraryManagement.Application.Services.Interfaces;
using LibraryManagement.Infrastructure.Identity; 
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description; 
using System.Collections.Generic; 
using Microsoft.Owin; 
using System.Net.Http; 

namespace LibraryManagement.Api.Controllers
{
    [RoutePrefix("api/books")]
    public class BooksController : ApiController
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        [ResponseType(typeof(IEnumerable<BookReadDto>))] 
        public async Task<IHttpActionResult> GetBooks([FromUri] BookFilterDto filter)
        {
            if (filter == null) filter = new BookFilterDto();
            var result = await _bookService.GetBooksAsync(filter);

            IOwinContext owinContext = Request.GetOwinContext();
            if (owinContext != null) 
            {
                owinContext.Response.Headers.Add("X-Pagination-TotalCount", new[] { result.TotalCount.ToString() });
                owinContext.Response.Headers.Add("X-Pagination-TotalPages", new[] { result.TotalPages.ToString() });
                owinContext.Response.Headers.Add("X-Pagination-CurrentPage", new[] { result.CurrentPage.ToString() });
                owinContext.Response.Headers.Add("X-Pagination-PageSize", new[] { result.PageSize.ToString() });
            }
            return Ok(result.Items); 
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetBookById")] 
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        [ResponseType(typeof(BookReadDto))]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
    
            return Ok(book);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = Roles.Admin)] 
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
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> UpdateBook(int id, [FromBody] BookUpdateDto bookUpdateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != bookUpdateDto.Id) return BadRequest("ID mismatch in route and body.");

            await _bookService.UpdateBookAsync(bookUpdateDto);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteBook(int id)
        {
            await _bookService.DeleteBookAsync(id);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}