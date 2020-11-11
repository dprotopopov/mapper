using Mapper.Services.Api;
using Mapper.Services.Upload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Mapper.Controllers
{
    public class OsmController : UploadController<OsmUploadService>
    {
        private readonly OsmApiService _osmApiService;

        public OsmController(IConfiguration configuration, OsmUploadService uploadService, OsmApiService osmApiService)
            : base(configuration, uploadService)
        {
            _osmApiService = osmApiService;
        }

        protected override string GetConnectionString()
        {
            return Configuration.GetConnectionString("OsmConnection");
        }

        protected override UploadFormInfo GetInstallFormInfo()
        {
            return new UploadFormInfo
            {
                Title = "Установка OSM",
                Label = "Полная БД OSM"
            };
        }

        protected override UploadFormInfo GetUpdateFormInfo()
        {
            return new UploadFormInfo
            {
                Title = "Установка OSM",
                Label = "Обновление БД OSM"
            };
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string osm_id, string osm_type)
        {
            long.TryParse(osm_id, out var id);
            int.TryParse(osm_type, out var type);

            return Content(JsonConvert.SerializeObject(_osmApiService.GetOsmById(id, (OsmType) type)));
        }
    }
}