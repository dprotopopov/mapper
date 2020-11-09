using Mapper.Services;
using Microsoft.Extensions.Configuration;

namespace Mapper.Controllers
{
    public class FiasController : UploadController<PgDbfService>
    {
        protected override string GetConnectionString()
        {
            return Configuration.GetConnectionString("FiasConnection");
        }

        protected override ViewBagInfo GetInstallInfo()
        {
            return new ViewBagInfo()
            {
                Title = "Установка ФИАС",
                Label = "Полная БД ФИАС (zip-архив dbf-файлов)"
            };
        }

        protected override ViewBagInfo GetIUpdateInfo()
        {
            return new ViewBagInfo()
            {
                Title = "Установка ФИАС",
                Label = "Обновление БД ФИАС (zip-архив dbf-файлов)"
            };
        }

        public FiasController(IConfiguration configuration, PgDbfService service) : base(configuration, service)
        {
        }
    }
}