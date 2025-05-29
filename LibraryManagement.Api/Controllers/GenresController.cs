using LibraryManagement.Application.DTOs.GenreDtos;
using LibraryManagement.Application.Services.Interfaces;
using LibraryManagement.Infrastructure.Identity;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

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
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
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
            return Ok(genre);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = Roles.Admin)]
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

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = Roles.Admin)] 
        [ResponseType(typeof(void))]     
        public async Task<IHttpActionResult> UpdateGenre(int id, [FromBody] GenreUpdateDto genreUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != genreUpdateDto.Id)
            {
                return BadRequest("ID mismatch in route and body.");
            }

            await _genreService.UpdateGenreAsync(genreUpdateDto);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = Roles.Admin)] 
        [ResponseType(typeof(void))]    
        public async Task<IHttpActionResult> DeleteGenre(int id)
        {
            await _genreService.DeleteGenreAsync(id);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}