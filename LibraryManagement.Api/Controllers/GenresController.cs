using LibraryManagement.Application.DTOs.BookDtos;
using LibraryManagement.Application.DTOs.GenreDtos;
using LibraryManagement.Application.Services;
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
        [Route("")]
        [Authorize(Roles = Roles.Admin)] 
        [ResponseType(typeof(GenreUpdateDto))]
        public async Task<IHttpActionResult> UpdateGenre([FromBody] GenreUpdateDto GenreUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _genreService.UpdateGenreAsync(GenreUpdateDto);
            return Ok(GenreUpdateDto);
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = Roles.Admin)] 
        [ResponseType(typeof(object))]    
        public async Task<IHttpActionResult> DeleteGenre(int id)
        {
            await _genreService.DeleteGenreAsync(id);
            return Ok(new { message = $"Genre with ID {id} was successfully deleted." });
        }
    }
}