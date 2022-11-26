using Beltelecom.ClassEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Dapper;

namespace Beltelecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProviderController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]  // List All Providers
        public async Task<ActionResult<List<Provider>>> ListAllProviders()
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpProviderAll = await connection.QueryAsync<Provider>("SELECT * FROM Provider");
            return Ok(tmpProviderAll);
        }

        [HttpGet("id={ProviderId}")] // Get a list of Providers by the specified identifier (ProvId)

        public async Task<ActionResult<Provider>> ListProviderById(int ProviderId)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpProviderById = await connection.QueryFirstOrDefaultAsync<Provider>("SELECT * FROM Provider WHERE ProvId = @Id",
                new { Id = ProviderId });
            if (tmpProviderById is null)
            {
                return BadRequest($"Provider with ID - {ProviderId} does not exist.");
            }
            return Ok(tmpProviderById);
        }

        [HttpGet("email={ProviderEmail}")] // Get a list of Provider by the Email
        public async Task<ActionResult<Provider>> ListProviderByEmail(string ProviderEmail)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpProviderByEmail = await connection.QueryFirstOrDefaultAsync<Provider>("SELECT * FROM Provider WHERE Email = @Email",
                new { Email = ProviderEmail });
            if (tmpProviderByEmail is null)
            {
                return BadRequest($"Provider Email - {ProviderEmail} does not exist.");
            }
            return Ok(tmpProviderByEmail);
        }

        [HttpGet("phone={ProviderPhone}")] // Get a list of Provider by the Phone
        public async Task<ActionResult<Provider>> ListProviderByPhone(string ProviderPhone)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpProviderByPhone = await connection.QueryFirstOrDefaultAsync<Provider>("SELECT * FROM Provider where Phone = @Phone",
                new { Phone = ProviderPhone });
            if (tmpProviderByPhone is null)
            {
                return BadRequest($"Provider with Phone - {ProviderPhone} does not exist.");
            }
            return Ok(tmpProviderByPhone);
        }

        [HttpPost] // Add New Provider
        public async Task<ActionResult<List<Provider>>> AddProvider(Provider AddProvider)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("INSERT INTO Provider (Name, Address, Phone, Site, Email) values (@Name, @Address, @Phone, @Site, @Email)", AddProvider);
            return Ok(await SelectAllProviders(connection));
        }

        [HttpPut] // Update Provider
        public async Task<ActionResult<List<Provider>>> UpdateProvider(Provider UpdateProvider)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("UPDATE Provider SET Name = @Name, Address = @Address, Phone = @Phone, Site = @Site, Email = @Email where ProvId = @ProvId", UpdateProvider);
            return Ok(await SelectAllProviders(connection));
        }

        [HttpDelete("delete={provId}")] // Delete Provider
        public async Task<ActionResult<List<Provider>>> DeleteProvider(int provId)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE FROM Provider WHERE ProvId = @ProviderId", new { ProviderId = provId });
            return Ok(await SelectAllProviders(connection));
        }
        private static async Task<IEnumerable<Provider>> SelectAllProviders(MySqlConnection connection)
        {
            return await connection.QueryAsync<Provider>("SELECT * FROM Provider");
        }
    }
}