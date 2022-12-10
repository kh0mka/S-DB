using Beltelecom.ClassEntities;
using Beltelecom.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace Beltelecom.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _context;

        public HomeController(ILogger<HomeController> logger, IConfiguration config, IHttpContextAccessor context)
        {
            _logger = logger;
            _config = config;
            _context = context;
        }

        public IActionResult Index()
        {
            var isUserAuthorized = _context.HttpContext.Request.Cookies.TryGetValue("number", out var phoneNumber);

            var viewDataMessage = isUserAuthorized ? $"Вы в системе. Ваш номер телефона {phoneNumber}" : "Вы не авторизованы.";

            ViewData["AuthMessage"] = viewDataMessage;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Account()
        {
            var isUserAuthorized = _context.HttpContext.Request.Cookies.TryGetValue("number", out var phoneNumber);

            if (!isUserAuthorized) return View(new Clients { });

            var connectionString = _config.GetConnectionString("DbConnection");
            await using var connection = new MySqlConnection(connectionString);

            var user = await connection.QueryFirstOrDefaultAsync<Clients>("SELECT * FROM Clients where Phone = @Phone",
             new { Phone = phoneNumber });

            return View(user);
        }


        public IActionResult About()
        {
            return View();
        }

        public IActionResult Tariffs()
        {
            return View();
        }


        public ActionResult Authorization()
        {
            var isUserAuthorized = _context.HttpContext.Request.Cookies.TryGetValue("number", out var phoneNumber);

            return View(isUserAuthorized);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Authorization(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var connectionString = _config.GetConnectionString("DbConnection");
                await using var connection = new MySqlConnection(connectionString);

                var user = await connection.QueryFirstOrDefaultAsync<Clients>("SELECT * FROM Clients where Phone = @Phone",
                 new { Phone = model.PhoneNumber });

                if (user != null)
                {

                    _context.HttpContext.Response.Cookies.Append("number", model.PhoneNumber);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData["Error"] = "Пользователя с таким номером телефона нет.";
                }
            }

            return View(model);
        }


        public IActionResult Logout()
        {
            _context.HttpContext.Response.Cookies.Delete("number");

            return RedirectToAction("Index", "Home");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}