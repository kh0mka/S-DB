using Dapper;
using Beltelecom.ClassEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Beltelecom.ClassViewModels;

namespace Beltelecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AffiliateController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AffiliateController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]  // List All Affiliates
        public async Task<ActionResult<List<Affiliate>>> ListAllAffiliates()
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpAffiliateAll = await connection.QueryAsync<Affiliate>("SELECT * FROM Affiliate");
            return Ok(tmpAffiliateAll);
        }

        [HttpGet("id={affiliateId}")] // Get a list of Affiliates by the specified identifier (AffId)

        public async Task<ActionResult<Affiliate>> ListAffiliateById(int affiliateId)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpAffiliateById = await connection.QueryFirstOrDefaultAsync<Affiliate>("SELECT * FROM Affiliate WHERE AffId = @Id",
                new { Id = affiliateId });
            if (tmpAffiliateById is null)
            {
                return BadRequest($"Affiliate with ID - {affiliateId} does not exist.");
            }
            return Ok(tmpAffiliateById);
        }

        [HttpGet("CountAffiliates")]  // Get count of affiliates (AggregateFunction)
        public async Task<ActionResult<List<AffiliateCountViewModel>>> CountAffiliates()
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpCountAffiliates = await connection.QueryAsync<AffiliateCountViewModel>("SELECT COUNT(*) as 'CountAffiliates' FROM Affiliate");
            return Ok(tmpCountAffiliates);
        }

        [HttpGet("email={affiliateEmail}")] // Get a list of Affiliate by the Email
        public async Task<ActionResult<Affiliate>> ListAffiliateByEmail(string affiliateEmail)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpAffiliateByEmail = await connection.QueryFirstOrDefaultAsync<Affiliate>("SELECT * FROM Affiliate WHERE Email = @Email",
                new { Email = affiliateEmail });
            if (tmpAffiliateByEmail is null)
            {
                return BadRequest($"Affiliate Email - {affiliateEmail} does not exist.");
            }
            return Ok(tmpAffiliateByEmail);
        }

        [HttpGet("phone={affiliatePhone}")] // Get a list of Affiliate by the Phone
        public async Task<ActionResult<Affiliate>> ListAffiliateByPhone(string affiliatePhone)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            var tmpAffiliateByPhone = await connection.QueryFirstOrDefaultAsync<Affiliate>("SELECT * FROM Affiliate where Phone = @Phone",
                new { Phone = affiliatePhone });
            if (tmpAffiliateByPhone is null)
            {
                return BadRequest($"Affiliate with Phone - {affiliatePhone} does not exist.");
            }
            return Ok(tmpAffiliateByPhone);
        }

        [HttpPost] // Add New Affiliate
        public async Task<ActionResult<List<Affiliate>>> AddAffiliate(Affiliate AddAffiliate)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("INSERT INTO Affiliate (Name, Address, Phone, Site, Email, ProvId) values (@Name, @Address, @Phone, @Site, @Email, @ProvId)", AddAffiliate);
            return Ok(await SelectAllAffiliates(connection));
        }

        [HttpPut] // Update Affiliate
        public async Task<ActionResult<List<Affiliate>>> UpdateAffiliate(Affiliate UpdateAffiliate)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("UPDATE Affiliate SET Name = @Name, Address = @Address, Phone = @Phone, Site = @Site, Email = @Email, ProvId = @ProvId where AffId = @AffId", UpdateAffiliate);
            return Ok(await SelectAllAffiliates(connection));
        }

        [HttpDelete("delete={affId}")] // Delete Affiliate
        public async Task<ActionResult<List<Affiliate>>> DeleteAffiliate(int affId)
        {
            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE FROM Affiliate WHERE AffId = @AffiliateId", new { AffiliateId = affId });
            return Ok(await SelectAllAffiliates(connection));
        }
        private static async Task<IEnumerable<Affiliate>> SelectAllAffiliates(MySqlConnection connection)
        {
            return await connection.QueryAsync<Affiliate>("SELECT * FROM Affiliate");
        }
    }
}
