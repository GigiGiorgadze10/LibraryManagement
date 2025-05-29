// src/LibraryManagement.Api/Controllers/AuthorsController.cs
using LibraryManagement.Application.DTOs.AuthorDtos;
using LibraryManagement.Application.Services.Interfaces;
using LibraryManagement.Infrastructure.Identity; // For Roles
using System; // For ArgumentNullException
using System.Collections.Generic; // For IEnumerable
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description; // For ApiResponseType

namespace LibraryManagement.Api.Controllers
{
    [RoutePrefix("api/authors")]
    public class AuthorsController : ApiController
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            // It's good practice to check for null dependencies, though Autofac usually handles this.
            _authorService = authorService ?? throw new ArgumentNullException(nameof(authorService));
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)] // User or Admin can view
        [ResponseType(typeof(IEnumerable<AuthorReadDto>))]
        public async Task<IHttpActionResult> GetAllAuthors()
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            return Ok(authors);
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetAuthorById")] // Named route for CreatedAtRoute
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        [ResponseType(typeof(AuthorReadDto))]
        public async Task<IHttpActionResult> GetAuthor(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            // Assuming NotFoundException is thrown by service if not found,
            // and handled by your ErrorHandlingMiddleware.
            // If service returns null, you might want: if (author == null) return NotFound();
            return Ok(author);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = Roles.Admin)] // Only Admin can create
        [ResponseType(typeof(AuthorReadDto))]
        public async Task<IHttpActionResult> CreateAuthor([FromBody] AuthorCreateDto authorCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdAuthor = await _authorService.CreateAuthorAsync(authorCreateDto);
            // If CreateAuthorAsync can throw exceptions (e.g., ValidationException for age < 18),
            // they should be handled by ErrorHandlingMiddleware.
            return CreatedAtRoute("GetAuthorById", new { id = createdAuthor.Id }, createdAuthor);
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = Roles.Admin)] // Only Admin can update
        [ResponseType(typeof(void))] // Indicates no body content on successful update
        public async Task<IHttpActionResult> UpdateAuthor(int id, [FromBody] AuthorUpdateDto authorUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != authorUpdateDto.Id)
            {
                return BadRequest("ID mismatch in route and body.");
            }

            // Assuming UpdateAuthorAsync throws NotFoundException or ValidationException for business rule violations,
            // which are then handled by your ErrorHandlingMiddleware.
            // Alternatively, if UpdateAuthorAsync returns a boolean:
            // bool success = await _authorService.UpdateAuthorAsync(authorUpdateDto);
            // if (!success) return NotFound(); // Or another appropriate error
            await _authorService.UpdateAuthorAsync(authorUpdateDto);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = Roles.Admin)] // Only Admin can delete
        [ResponseType(typeof(void))] // Indicates no body content on successful delete
        public async Task<IHttpActionResult> DeleteAuthor(int id)
        {
            // Assuming DeleteAuthorAsync throws NotFoundException if author does not exist,
            // which is then handled by your ErrorHandlingMiddleware.
            await _authorService.DeleteAuthorAsync(id);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}