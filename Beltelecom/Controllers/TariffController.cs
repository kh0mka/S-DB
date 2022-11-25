using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace Beltelecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TariffController : ControllerBase
    {
        private readonly IConfiguration _config;
        
        public TariffController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<Tariff>>> GetAllTariff()
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            // using var connection = new SqlConnection(_config.GetConnectionString("DbConnection"));
            var alltariff = await connection.QueryAsync<Tariff>("SELECT * FROM Tariff");
            return Ok(alltariff);
        }

        [HttpGet("id={tariffId}")]

        public async Task<ActionResult<Tariff>> GetTariff(int tariffId)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tariff = await connection.QueryFirstOrDefaultAsync<Tariff>("select * from Tariff where TariffId = @Id",
                new { Id = tariffId });
            if (tariff is null)
            {
                return BadRequest($"Id {tariffId} does not exist.");
            }

            return Ok(tariff);
        }
    }
}
