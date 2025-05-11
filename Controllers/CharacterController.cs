using Microsoft.AspNetCore.Mvc;
using syncdata.Repositories;
using syncdata.Services;

namespace syncdata.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly CharacterService _service;

        public CharacterController(IConfiguration config)
        {
            var repo = new CharacterRepository(config);
            _service = new CharacterService(repo);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var characters = await _service.GetCharactersFromDb();
            return Ok(characters);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var character = await _service.GetCharacterById(id);
            if (character == null)
                return NotFound(new { message = $"Character with id {id} not found." });

            return Ok(character);
        }


        [HttpPost("sync")]
        public async Task<IActionResult> Sync()
        {
            await _service.SyncCharactersFromApi();
            return Ok(new { message = "Sinkronisasi selesai." });
        }
    }

}
