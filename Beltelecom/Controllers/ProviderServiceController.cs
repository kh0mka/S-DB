using Beltelecom.ClassEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Dapper;
using Beltelecom.ClassViewModels;

namespace Beltelecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderServiceController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProviderServiceController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]  // List All Providers Services
        public async Task<ActionResult<List<ProviderService>>> ListAllProviderServices()
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpProviderServiceAll = await connection.QueryAsync<ProviderService>("SELECT * FROM ProviderService");
            return Ok(tmpProviderServiceAll);
        }

        [HttpGet("id={ProviderServiceId}")] // Get a list of Provider Services by the specified identifier (ServiceId)

        public async Task<ActionResult<ProviderService>> ListProviderServicesById(int ProviderServiceId)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpProviderServiceById = await connection.QueryFirstOrDefaultAsync<ProviderService>("SELECT * FROM ProviderService WHERE ServiceId = @Id",
                new { Id = ProviderServiceId });
            if (tmpProviderServiceById is null)
            {
                return BadRequest($"Provider Service with ID - {ProviderServiceId} does not exist.");
            }
            return Ok(tmpProviderServiceById);
        }

        [HttpGet("ProvId={ProviderServiceProvId}")] // Get a list of Provider Services by the ID - ProvId
        public async Task<ActionResult<ProviderViewModel>> ListProviderServicesByProvId(int ProviderServiceProvId)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpProviderServiceByProvId = await connection.QueryAsync<ProviderViewModel>("SELECT ProviderService.ServiceId AS ServiceId, ProviderService.Name AS ServiceName, Provider.ProvId as ProviderId, Provider.Name as ProviderName FROM ProviderService JOIN Provider on ProviderService.ProvId = Provider.ProvId WHERE ProviderService.ServiceId = ANY(SELECT ServiceId from ProviderService WHERE ProvId = @ProvId)",
                new { ProvId = ProviderServiceProvId });
            if (!tmpProviderServiceByProvId.Any())
            {
                return BadRequest($"ProviderService with ProvId - {ProviderServiceProvId} does not exist.");
            }
            return Ok(tmpProviderServiceByProvId);
        }

        [HttpPost] // Add New ProviderService
        public async Task<ActionResult<List<ProviderService>>> AddProviderService(ProviderService AddProviderService)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("INSERT INTO ProviderService (Name, ProvId) values (@Name, @ProvId)", AddProviderService);
            return Ok(await SelectAllProviderServices(connection));
        }

        [HttpPut] // Update ProviderService
        public async Task<ActionResult<List<ProviderService>>> UpdateProviderService(ProviderService UpdateProviderService)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("UPDATE ProviderService SET Name = @Name, ProvId = @ProvId where ServiceId = @ServiceId", UpdateProviderService);
            return Ok(await SelectAllProviderServices(connection));
        }

        [HttpDelete("delete={providerServiceId}")] // Delete ProviderService
        public async Task<ActionResult<List<ProviderService>>> DeleteProviderService(int providerServiceId)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE FROM ProviderService WHERE ServiceId = @ServiceId", new { ServiceId = providerServiceId });
            return Ok(await SelectAllProviderServices(connection));
        }
        private static async Task<IEnumerable<ProviderService>> SelectAllProviderServices(MySqlConnection connection)
        {
            return await connection.QueryAsync<ProviderService>("SELECT * FROM ProviderService");
        }
    }
}
