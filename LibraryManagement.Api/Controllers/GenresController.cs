// src/LibraryManagementSystem.Api/Controllers/GenresController.cs
using LibraryManagement.Application.DTOs.GenreDtos;
using LibraryManagement.Application.Services.Interfaces;
using LibraryManagement.Infrastructure.Identity; // For Roles
using System.Collections.Generic; // For IEnumerable
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description; // For ApiResponseType

namespace LibraryManagement.Api.Controllers
{
    [RoutePrefix("api/genres")]
    public class GenresController : ApiController
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)] // User or Admin can view
        [ResponseType(typeof(IEnumerable<GenreReadDto>))]
        public async Task<IHttpActionResult> GetAllGenres()
        {
            var genres = await _genreService.GetAllGenresAsync();
            return Ok(genres);
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetGenreById")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        [ResponseType(typeof(GenreReadDto))]
        public async Task<IHttpActionResult> GetGenre(int id)
        {
            var genre = await _genreService.GetGenreByIdAsync(id);
            // NotFoundException handled by ErrorHandlingMiddleware
            return Ok(genre);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = Roles.Admin)] // Only Admin can create
        [ResponseType(typeof(GenreReadDto))]
        public async Task<IHttpActionResult> CreateGenre([FromBody] GenreCreateDto genreCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdGenre = await _genreService.CreateGenreAsync(genreCreateDto);
            return CreatedAtRoute("GetGenreById", new { id = createdGenre.Id }, createdGenre);
        }

        // No Update or Delete for Genres as per PDF requirement (Only Read/Create)
    }
}