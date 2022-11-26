using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Hosting;
using MySqlX.XDevAPI;
using Beltelecom.ClassEntities;
using Beltelecom.ClassViewModels;

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

        [HttpGet]  // Get a list of all available Tariffs
        public async Task<ActionResult<List<Tariff>>> ListAllTariffs()
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var alltariff = await connection.QueryAsync<Tariff>("SELECT * FROM Tariff");
            return Ok(alltariff);
        }

        [HttpGet("AVGCost")]  // Get average cost of Tariffs (Aggregate Function)
        public async Task<ActionResult<List<TariffAVGViewModel>>> CountAffiliates()
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpAVGCost = await connection.QueryAsync<TariffAVGViewModel>("SELECT AVG(Cost) as 'AVGCost' FROM Tariff");
            return Ok(tmpAVGCost);
        }

        [HttpGet("costDESC")] // Order Tariffs Cost by DESC
        public async Task<ActionResult<List<Tariff>>> OrderByTariffsDESC()
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var OrderByTariffsDesc = await connection.QueryAsync<Tariff>("SELECT * FROM Tariff ORDER by Cost DESC");
            return Ok(OrderByTariffsDesc);
        }

        [HttpGet("costASC")] // Order Tariffs Cost by ASC
        public async Task<ActionResult<List<Tariff>>> OrderByTariffsASC()
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var OrderByTariffsDesc = await connection.QueryAsync<Tariff>("SELECT * FROM Tariff ORDER by Cost ASC");
            return Ok(OrderByTariffsDesc);
        }

        //[HttpGet("CostOrderBy={order}")] // Get a list of tariffs by the specified identifier (TariffId)
        //public async Task<ActionResult<Tariff>> Lirbwrbwrbwbr(string order)
        //{
        //    var connectionString = _config.GetConnectionString("DbConnection");
        //    await using var connection = new MySqlConnection(connectionString);
        //    var tmpCostOrderBy = await connection.QueryFirstOrDefaultAsync<Tariff>("SELECT * FROM Tariff ORDER by Cost @OrderBy",
        //        new { OrderBy = order });
        //    if (tmpCostOrderBy is null)
        //    {
        //        return BadRequest($"Id {order} does not exist.");
        //    }

        //    return Ok(tmpCostOrderBy);
        //}

        [HttpGet("id={tariffId}")] // Get a list of tariffs by the specified identifier (TariffId)

        public async Task<ActionResult<Tariff>> ListTariffById(int tariffId)
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

        [HttpGet("SpeedIsHigher={SpeedValue}")] // Get a list of tariffs, the speed of which is greater than or equal to the specified value

        public async Task<ActionResult<Tariff>> SpeedIsHigher(float SpeedValue)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpSpeedValue = await connection.QueryFirstOrDefaultAsync<Tariff>("SELECT * FROM Tariff where Speed >= @Speed",
                new { Speed = SpeedValue });
            if (tmpSpeedValue is null)
            {
                return BadRequest($"Tariffs, the speed of which is more than {SpeedValue} MB/s was not found.");
            }
            return Ok(tmpSpeedValue);
        }

        [HttpGet("SpeedIsBelow={SpeedValue}")] // Get a list of tariffs whose speed is less than or equal to the specified value

        public async Task<ActionResult<Tariff>> SpeedIsBelow(float SpeedValue)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpSpeedValue = await connection.QueryFirstOrDefaultAsync<Tariff>("SELECT * FROM Tariff where Speed <= @Speed",
                new { Speed = SpeedValue });
            if (tmpSpeedValue is null)
            {
                return BadRequest($"Tariffs, the speed of which is less than {SpeedValue} MB/s was not found.");
            }
            return Ok(tmpSpeedValue);
        }

        [HttpGet("cost={costTariff}")] // Get a list of customers whose tariff exceeds the specified price

        public async Task<ActionResult<ClientsViewModel>> RichPoor(decimal costTariff)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpCost = await connection.QueryAsync<ClientsViewModel>("SELECT Clients.ClientId AS Id, Clients.Email AS Email, Tariff.Cost AS Cost, Tariff.Name AS TariffName FROM Clients JOIN Tariff on Clients.TariffId = Tariff.TariffId WHERE Tariff.TariffId = ANY(SELECT TariffId from Tariff WHERE Cost >= @Cost);", new { Cost = costTariff });
            return Ok(tmpCost);
        }

        [HttpPost] // Add New Tariff
        public async Task<ActionResult<List<Tariff>>> AddTariff(Tariff createtariff)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("insert into tariff (Name, Speed, Cost, TechId, ProvId) values (@Name, @Speed, @Cost, @TechId, @ProvId)", createtariff);
            return Ok(await SelectAllTariff(connection));
        }

        [HttpPut] // Update Tariff
        public async Task<ActionResult<List<Tariff>>> UpdateTariff(Tariff updatetariff)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("update tariff set Name = @Name, Speed = @Speed, Cost = @Cost, TechId = @TechId, ProvId = @ProvId where TariffId = @TariffId", updatetariff);
            return Ok(await SelectAllTariff(connection));
        }

        [HttpDelete("delete={tariffId}")] // Delete Tariff
        public async Task<ActionResult<List<Tariff>>> DeleteTariff(int tariffId)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("delete from tariff where TariffId = @TariffId", new { TariffId = tariffId });
            return Ok(await SelectAllTariff(connection));
        }
        private static async Task<IEnumerable<Tariff>> SelectAllTariff(MySqlConnection connection)
        {
            return await connection.QueryAsync<Tariff>("select * from Tariff");
        }

    }
}
