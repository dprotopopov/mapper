using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapper.Models;
using Mapper.Services;
using Mapper.Services.Api;
using Mapper.Services.Upload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace Mapper.Controllers
{
    public class FiasController : UploadController<FiasUploadService>
    {
        private readonly FiasApiService _fiasApiService;

        public FiasController(IConfiguration configuration, FiasUploadService service, FiasApiService fiasApiService) : base(
            configuration, service)
        {
            _fiasApiService = fiasApiService;
        }

        protected override string GetConnectionString()
        {
            return Configuration.GetConnectionString("FiasConnection");
        }

        protected override UploadFormInfo GetInstallFormInfo()
        {
            return new UploadFormInfo
            {
                Title = "Установка ФИАС",
                Label = "Полная БД ФИАС (zip-архив dbf-файлов)"
            };
        }

        protected override UploadFormInfo GetUpdateFormInfo()
        {
            return new UploadFormInfo
            {
                Title = "Установка ФИАС",
                Label = "Обновление БД ФИАС (zip-архив dbf-файлов)"
            };
        }

        public async Task<IActionResult> Index(string guid = null)
        {
            var model = new FiasSelectModel();


            if (string.IsNullOrEmpty(guid))
            {
                model.PreviousItems = new List<SelectListItem>();

                model.NextItems = (await _fiasApiService.GetRoots()).Select(x => new SelectListItem
                {
                    Text = x.title,
                    Value = x.guid.ToString()
                }).ToList();
            }
            else
            {
                model.PreviousItems = (await _fiasApiService.GetDetails(guid)).Select(x => new SelectListItem
                {
                    Text = x.title,
                    Value = x.guid.ToString()
                }).ToList();

                model.NextItems = (await _fiasApiService.GetChildren(guid)).Select(x => new SelectListItem
                {
                    Text = x.title,
                    Value = x.guid.ToString()
                }).ToList();
            }

            return View(model);
        }
    }
}