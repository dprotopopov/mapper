using Mapper.Services;
using Microsoft.Extensions.Configuration;

namespace Mapper.Controllers
{
    public class OsmController : UploadController<OsmService>
    {
        protected override string GetConnectionString()
        {
            return Configuration.GetConnectionString("OsmConnection");
        }

        public OsmController(IConfiguration configuration, OsmService service) : base(configuration, service)
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