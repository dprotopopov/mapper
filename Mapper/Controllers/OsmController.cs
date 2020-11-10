using Mapper.Services;
using Mapper.Services.Upload;
using Microsoft.Extensions.Configuration;

namespace Mapper.Controllers
{
    public class OsmController : UploadController<OsmUploadService>
    {
        protected override string GetConnectionString()
        {
            return Configuration.GetConnectionString("OsmConnection");
        }

        public OsmController(IConfiguration configuration, OsmUploadService uploadService) : base(configuration, uploadService)
        {
        }
        protected override ViewBagInfo GetInstallInfo()
        {
            return new ViewBagInfo()
            {
                Title = "Установка OSM",
                Label = "Полная БД OSM"
            };
        }

        protected override ViewBagInfo GetIUpdateInfo()
        {
            return new ViewBagInfo()
            {
                Title = "Установка OSM",
                Label = "Обновление БД OSM"
            };
        }
    }
}