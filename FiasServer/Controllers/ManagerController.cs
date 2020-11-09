using System.Threading.Tasks;
using FiasServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace FiasServer.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly PgDbfService _pgDbfService;

        public ManagerController(IConfiguration configuration, PgDbfService pgDbfService)
        {
            _configuration = configuration;
            _pgDbfService = pgDbfService;
        }

        public async Task<IActionResult> Install()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Install(IFormFile file, string session)
        {
            var connString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new NpgsqlConnection(connString))
            using (var stream = file.OpenReadStream())
            {
                connection.Open();
                await _pgDbfService.Install(stream, connection, session);
                connection.Close();
            }

            return Content(file.FileName);
        }

        public async Task<IActionResult> Update()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(IFormFile file, string session)
        {
            var connString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new NpgsqlConnection(connString))
            using (var stream = file.OpenReadStream())
            {
                connection.Open();
                await _pgDbfService.Update(stream, connection, session);
                connection.Close();
            }

            return Content(file.FileName);
        }
    }
}