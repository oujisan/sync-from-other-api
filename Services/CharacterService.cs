using syncdata.Models;
using System.Text.Json;
using syncdata.Repositories;

namespace syncdata.Services
{
    public class CharacterService
    {
        private readonly CharacterRepository _repo;
        private readonly HttpClient _http;

        public CharacterService(CharacterRepository repo)
        {
            _repo = repo;
            _http = new HttpClient();
        }

        public async Task<List<Character>> GetCharactersFromDb()
        {
            return await _repo.GetAllCharacters();
        }

        public async Task<Character?> GetCharacterById(int id)
        {
            return await _repo.GetCharacterById(id);
        }


        public async Task SyncCharactersFromApi()
        {
            var response = await _http.GetAsync("https://hsr-api.vercel.app/api/v1/characters");
            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();
            var rawCharacters = JsonSerializer.Deserialize<List<JsonElement>>(json);

            var characters = rawCharacters.Select(item => new Character
            {
                CharId = item.GetProperty("id").GetInt32(),
                Name = item.GetProperty("name").GetString() ?? "",
                Rarity = item.GetProperty("rarity").GetInt32(),
                Path = item.GetProperty("path").GetString() ?? "",
                Element = item.GetProperty("element").GetString() ?? "",
                Release = item.GetProperty("release").GetDateTime(),
                Introduction = item.GetProperty("introduction").GetString() ?? "",
                ImgUrl = item.GetProperty("img").GetString() ?? ""
            }).ToList();

            await _repo.InsertCharacters(characters);
        }
    }
}
