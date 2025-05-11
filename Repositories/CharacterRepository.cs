using Npgsql;
using System.Text.Json;
using syncdata.Models;

namespace syncdata.Repositories
{
    public class CharacterRepository
    {
        private readonly string _connectionString;

        public CharacterRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Character>> GetAllCharacters()
        {
            var characters = new List<Character>();

            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand("SELECT * FROM characters", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                characters.Add(new Character
                {
                    CharId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Rarity = reader.GetInt32(2),
                    Path = reader.GetString(3),
                    Element = reader.GetString(4),
                    Release = reader.GetDateTime(5),
                    Introduction = reader.GetString(6),
                    ImgUrl = reader.GetString(7)
                });
            }

            return characters;
        }

        public async Task<Character?> GetCharacterById(int charId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand("SELECT * FROM characters WHERE char_id = @id", conn);
            cmd.Parameters.AddWithValue("id", charId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Character
                {
                    CharId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Rarity = reader.GetInt32(2),
                    Path = reader.GetString(3),
                    Element = reader.GetString(4),
                    Release = reader.GetDateTime(5),
                    Introduction = reader.GetString(6),
                    ImgUrl = reader.GetString(7)
                };
            }

            return null;
        }

        public async Task<bool> IsDataExists()
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM characters", conn);
            var count = (long)await cmd.ExecuteScalarAsync();
            return count > 0;
        }

        public async Task InsertCharacters(List<Character> characters)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            foreach (var c in characters)
            {
                using var cmd = new NpgsqlCommand(
                    @"INSERT INTO characters (char_id, name, rarity, path, element, release, introduction, img_url) 
                  VALUES (@char_id, @name, @rarity, @path, @element, @release, @introduction, @img_url)
                  ON CONFLICT (char_id) DO UPDATE SET
                    name = EXCLUDED.name,
                    rarity = EXCLUDED.rarity,
                    path = EXCLUDED.path,
                    element = EXCLUDED.element,
                    release = EXCLUDED.release,
                    introduction = EXCLUDED.introduction,
                    img_url = EXCLUDED.img_url;", conn);

                cmd.Parameters.AddWithValue("char_id", c.CharId);
                cmd.Parameters.AddWithValue("name", c.Name);
                cmd.Parameters.AddWithValue("rarity", c.Rarity);
                cmd.Parameters.AddWithValue("path", c.Path);
                cmd.Parameters.AddWithValue("element", c.Element);
                cmd.Parameters.AddWithValue("release", c.Release);
                cmd.Parameters.AddWithValue("introduction", c.Introduction);
                cmd.Parameters.AddWithValue("img_url", c.ImgUrl);

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}