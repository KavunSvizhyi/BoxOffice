using BoxOffice.Business.Interfaces;
using BoxOffice.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoxOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorModel>>> Get()
        {
            return Ok(await _authorService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorModel>> Get(int id)
        {
            var author = await _authorService.GetByIdAsync(id);

            if (author is null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<AuthorModel>>> Post(AuthorModel model)
        {
            var output = await _authorService.AddAsync(model);

            return CreatedAtAction(nameof(this.Get), new { id = output.Id }, output);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, AuthorModel model)
        {
            if (model.Id != id)
            {
                return BadRequest();
            }

            await _authorService.UpdateAsync(model);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _authorService.DeleteAsync(id);

            return NoContent();
        }
    }
}
