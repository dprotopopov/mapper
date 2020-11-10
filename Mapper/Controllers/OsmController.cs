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
        protected override UploadFormInfo GetInstallFormInfo()
        {
            return new UploadFormInfo()
            {
                Title = "Установка OSM",
                Label = "Полная БД OSM"
            };
        }

        protected override UploadFormInfo GetUpdateFormInfo()
        {
            return new UploadFormInfo()
            {
                Title = "Установка OSM",
                Label = "Обновление БД OSM"
            };
        }
    }
}