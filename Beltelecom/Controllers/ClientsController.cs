using Dapper;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI;

namespace Beltelecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ClientsController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]  // List All Clients
        public async Task<ActionResult<List<Clients>>> ListAllClients()
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpClientsAll = await connection.QueryAsync<Clients>("SELECT * FROM Clients");
            return Ok(tmpClientsAll);
        }

        [HttpGet("id={clientId}")] // Get a list of Clients by the specified identifier (ClientId)

        public async Task<ActionResult<Clients>> ListClientById(int clientId)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpClientById = await connection.QueryFirstOrDefaultAsync<Clients>("select * from Clients where ClientId = @Id",
                new { Id = clientId });
            if (tmpClientById is null)
            {
                return BadRequest($"ClientId - {clientId} does not exist.");
            }
            return Ok(tmpClientById);
        }

        [HttpGet("email={clientEmail}")] // Get a list of Clients by the Email
        public async Task<ActionResult<Clients>> ListClientByEmail(string clientEmail)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpClientByEmail = await connection.QueryFirstOrDefaultAsync<Clients>("select * from Clients where Email = @Email",
                new { Email = clientEmail });
            if (tmpClientByEmail is null)
            {
                return BadRequest($"Email - {clientEmail} does not exist.");
            }
            return Ok(tmpClientByEmail);
        }

        [HttpGet("phone={clientPhone}")] // Get a list of Clients by the Phone
        public async Task<ActionResult<Clients>> ListClientByPhone(string clientPhone)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpClientByPhone = await connection.QueryFirstOrDefaultAsync<Clients>("select * from Clients where Phone = @Phone",
                new { Phone = clientPhone });
            if (tmpClientByPhone is null)
            {
                return BadRequest($"Phone - {clientPhone} does not exist.");
            }
            return Ok(tmpClientByPhone);
        }

        [HttpPost] // Add New Client
        public async Task<ActionResult<List<Clients>>> AddClient(Clients AddClient)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("INSERT INTO Clients (Phone, Email, Address, TariffId) values (@Phone, @Email, @Address, @TariffId)", AddClient);
            return Ok(await SelectAllClients(connection));
        }

        [HttpPut] // Update Client
        public async Task<ActionResult<List<Clients>>> UpdateClient(Clients UpdateClient)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("UPDATE Clients SET Phone = @Phone, Email = @Email, Address = @Address, TariffId = @TariffId where ClientId = @ClientId", UpdateClient);
            return Ok(await SelectAllClients(connection));
        }

        [HttpDelete("delete={clientId}")] // Delete Client
        public async Task<ActionResult<List<Clients>>> DeleteClient(int clientId)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE FROM Clients WHERE ClientId = @ClientId", new { ClientId = clientId });
            return Ok(await SelectAllClients(connection));
        }
        private static async Task<IEnumerable<Clients>> SelectAllClients(MySqlConnection connection)
        {
            return await connection.QueryAsync<Clients>("SELECT * FROM Clients");
        }

    }

}
