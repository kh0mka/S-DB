using Beltelecom.ClassEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Dapper;

namespace Beltelecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechnologyController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TechnologyController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]  // List All Technologys
        public async Task<ActionResult<List<Technology>>> ListAllTechnologys()
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpTechnologyAll = await connection.QueryAsync<Technology>("SELECT * FROM Technology");
            return Ok(tmpTechnologyAll);
        }

        [HttpGet("id={TechnologyId}")] // Get a list of Technologies by the specified identifier (TechId)

        public async Task<ActionResult<Technology>> ListTechnologyById(int TechnologyId)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpTechnologyById = await connection.QueryFirstOrDefaultAsync<Technology>("SELECT * FROM Technology WHERE TechId = @Id",
                new { Id = TechnologyId });
            if (tmpTechnologyById is null)
            {
                return BadRequest($"Technology with ID - {TechnologyId} does not exist.");
            }
            return Ok(tmpTechnologyById);
        }

        [HttpPost] // Add New Technology
        public async Task<ActionResult<List<Technology>>> AddTechnology(Technology AddTechnology)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("INSERT INTO Technology (Name, MaxSpeed, ProvId) values (@Name, @MaxSpeed, @ProvId)", AddTechnology);
            return Ok(await SelectAllTechnologies(connection));
        }

        [HttpPut] // Update Technology
        public async Task<ActionResult<List<Technology>>> UpdateTechnology(Technology UpdateTechnology)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("UPDATE Technology SET Name = @Name, MaxSpeed = @MaxSpeed, ProvId = @ProvId where TechId = @TechId", UpdateTechnology);
            return Ok(await SelectAllTechnologies(connection));
        }

        [HttpDelete("delete={techId}")] // Delete Technology
        public async Task<ActionResult<List<Technology>>> DeleteTechnology(int techId)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE FROM Technology WHERE TechId = @TechnologyId", new { TechnologyId = techId });
            return Ok(await SelectAllTechnologies(connection));
        }
        private static async Task<IEnumerable<Technology>> SelectAllTechnologies(MySqlConnection connection)
        {
            return await connection.QueryAsync<Technology>("SELECT * FROM Technology");
        }
    }
}
