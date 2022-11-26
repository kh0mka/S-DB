using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Hosting;
using MySqlX.XDevAPI;

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
            var tmpTariff = await connection.QueryFirstOrDefaultAsync<Tariff>("select * from Tariff where TariffId = @Id",
                new { Id = tariffId });
            if (tmpTariff is null)
            {
                return BadRequest($"Id {tariffId} does not exist.");
            }

            return Ok(tmpTariff);
        }

        [HttpGet("cost={costTariff}")]

        public async Task<ActionResult<Tariff>> CostTUsers(decimal costTariff)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpCost = await connection.QueryAsync<Tariff>("SELECT Clients.ClientId AS Id, Clients.Email AS Email, Tariff.Cost AS Cost, Tariff.Name AS TariffName FROM Clients JOIN Tariff on Clients.TariffId = Tariff.TariffId WHERE Tariff.TariffId = ANY(SELECT TariffId from Tariff WHERE Cost >= @Cost); ", new { Cost = costTariff });
            //if (tmpCost is null)
            //{
            //    return BadRequest($"Cost {costTariff} very expensive.");
            //}
            return Ok(tmpCost);
        }

        [HttpPost]
        public async Task<ActionResult<List<Tariff>>> CreateTariff(Tariff createtariff)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("insert into tariff (Name, Speed, Cost, TechId, ProvId) values (@Name, @Speed, @Cost, @TechId, @ProvId)", createtariff);
            return Ok(await SelectAllTariff(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<Tariff>>> UpdateTariff(Tariff updatetariff)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("update tariff set Name = @Name, Speed = @Speed, Cost = @Cost, TechId = @TechId, ProvId = @ProvId where TariffId = @TariffId", updatetariff);
            return Ok(await SelectAllTariff(connection));
        }

        [HttpDelete("{tariffId}")]
        public async Task<ActionResult<List<Tariff>>> DeleteHero(int tariffId)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("delete from tariff where TariffId = @TariffId", new {TariffId = tariffId});
            return Ok(await SelectAllTariff(connection));
        }

        private static async Task<IEnumerable<Tariff>> SelectAllTariff(MySqlConnection connection)
        {
            return await connection.QueryAsync<Tariff>("select * from Tariff");
        }



    }
}
