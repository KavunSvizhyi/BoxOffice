using AutoMapper;
using Azure.Core;
using BoxOffice.Business.Interfaces;
using BoxOffice.Business.Models;
using BoxOffice.Business.Services;
using BoxOffice.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BoxOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;
        private readonly IMapper _mapper;

        public GenresController(IGenreService genreService, IMapper mapper)
        {
            _genreService = genreService;
            _mapper = mapper;
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
        public async Task<ActionResult<GenrePostResponse>> Post(AddGenreRequest request)
        {
            var model = _mapper.Map<GenreModel>(request);

            var output = await _genreService.AddAsync(model);

            var response = _mapper.Map<GenrePostResponse>(output);

            return CreatedAtAction(nameof(this.Get), new { id = response.Id }, response);
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
