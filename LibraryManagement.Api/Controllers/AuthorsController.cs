// src/LibraryManagementSystem.Api/Controllers/AuthorsController.cs
using LibraryManagement.Application.DTOs.AuthorDtos;
using LibraryManagement.Application.Services.Interfaces;
using LibraryManagement.Infrastructure.Identity; // For Roles
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
            _authorService = authorService;
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
        [Route("{id:int}", Name = "GetAuthorById")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        [ResponseType(typeof(AuthorReadDto))]
        public async Task<IHttpActionResult> GetAuthor(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            // NotFoundException will be handled by ErrorHandlingMiddleware
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
            return CreatedAtRoute("GetAuthorById", new { id = createdAuthor.Id }, createdAuthor);
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = Roles.Admin)] // Only Admin can update
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
            await _authorService.UpdateAuthorAsync(authorUpdateDto);
            // NotFoundException or ValidationException handled by ErrorHandlingMiddleware
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = Roles.Admin)] // Only Admin can delete
        public async Task<IHttpActionResult> DeleteAuthor(int id)
        {
            await _authorService.DeleteAuthorAsync(id);
            // NotFoundException handled by ErrorHandlingMiddleware
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}