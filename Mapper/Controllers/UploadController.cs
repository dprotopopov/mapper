using System.Threading.Tasks;
using Mapper.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Mapper.Controllers
{
    public abstract class UploadController<TService> : Controller where TService : IManagerService
    {
        protected readonly IConfiguration Configuration;
        protected readonly TService Service;

        public UploadController(IConfiguration configuration, TService service)
        {
            Configuration = configuration;
            Service = service;
        }

        public async Task<IActionResult> Install()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.UploadLink = $"/{controllerName}/{actionName}";
            var info = GetInstallInfo();
            ViewBag.Title = info.Title;
            ViewBag.Label = info.Label;
            return View("~/Views/_Upload.cshtml");
        }

        protected abstract string GetConnectionString();

        public class ViewBagInfo
        {
            public string Title { get; set; }
            public string Label { get; set; }
        }
        protected abstract ViewBagInfo GetInstallInfo();
        protected abstract ViewBagInfo GetIUpdateInfo();

        [HttpPost]
        public async Task<IActionResult> Install(IFormFile file, string session)
        {
            var connectionString = GetConnectionString();

            using (var connection = new NpgsqlConnection(connectionString))
            using (var stream = file.OpenReadStream())
            {
                connection.Open();
                await Service.Install(stream, connection, session);
                connection.Close();
            }

            return Content(file.FileName);
        }

        public async Task<IActionResult> Update()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.UploadLink = $"/{controllerName}/{actionName}";
            var info = GetIUpdateInfo();
            ViewBag.Title = info.Title;
            ViewBag.Label = info.Label;

            return View("~/Views/_Upload.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Update(IFormFile file, string session)
        {
            var connectionString = GetConnectionString();

            using (var connection = new NpgsqlConnection(connectionString))
            using (var stream = file.OpenReadStream())
            {
                connection.Open();
                await Service.Update(stream, connection, session);
                connection.Close();
            }

            return Content(file.FileName);
        }
    }
}