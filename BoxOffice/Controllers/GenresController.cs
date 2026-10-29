using BoxOffice.Business.Interfaces;
using BoxOffice.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoxOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenreModel>>> Get()
        {
            return Ok(await _genreService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GenreModel>> Get(int id)
        {
            var genre = await _genreService.GetByIdAsync(id);

            if (genre is null)
            {
                return NotFound();
            }

            return Ok(genre);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<GenreModel>>> Post(GenreModel model)
        {
            var output = await _genreService.AddAsync(model);

            return CreatedAtAction(nameof(this.Get), new { id = output.Id }, output);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, GenreModel model)
        {
            if (model.Id != id)
            {
                return BadRequest();
            }

            await _genreService.UpdateAsync(model);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _genreService.DeleteAsync(id);

            return NoContent();
        }
    }
}
