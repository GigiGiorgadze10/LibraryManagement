using LibraryManagement.Application.DTOs.AuthorDtos;
using LibraryManagement.Application.Services.Interfaces;
using LibraryManagement.Infrastructure.Identity; 
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http; 
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace LibraryManagement.Api.Controllers
{
    [RoutePrefix("api/authors")]
    public class AuthorsController : ApiController
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService ?? throw new ArgumentNullException(nameof(authorService));
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
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
            return Ok(author); 
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = Roles.Admin)]
        [ResponseType(typeof(AuthorReadDto))]
        public async Task<IHttpActionResult> CreateAuthor([FromBody] AuthorCreateDto authorCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdAuthor = await _authorService.CreateAuthorAsync(authorCreateDto);

            string locationUri = null;
            try
            {
                locationUri = Url.Link("GetAuthorById", new { id = createdAuthor.Id });
            }
            catch (NotImplementedException nie)
            {
                System.Diagnostics.Trace.TraceWarning($"CreateAuthor: Url.Link failed with NotImplementedException: {nie.Message}. HttpContextBase.Response might be unavailable. Falling back.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceWarning($"CreateAuthor: Error generating Location URI: {ex.Message}");
            }

            if (locationUri != null)
            {
                return Created(locationUri, createdAuthor);
            }
            else
            {
                var response = Request.CreateResponse(HttpStatusCode.Created, createdAuthor);
                System.Diagnostics.Debug.WriteLine("CreateAuthor: Fallback response used due to Url.Link failure.");
                return ResponseMessage(response);
            }
        }

        [HttpPut]
        [Route("")] 
        [Authorize(Roles = Roles.Admin)]
        [ResponseType(typeof(AuthorUpdateDto))] 
        public async Task<IHttpActionResult> UpdateAuthor([FromBody] AuthorUpdateDto authorUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _authorService.UpdateAuthorAsync(authorUpdateDto);
            return Ok(authorUpdateDto);
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> DeleteAuthor(int id)
        {
            await _authorService.DeleteAuthorAsync(id);
            return Ok(new { message = $"Author with ID {id} was successfully deleted." });
        }
    }
}