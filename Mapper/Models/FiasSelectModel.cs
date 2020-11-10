using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mapper.Models
{
    public class FiasSelectModel
    {
        public List<SelectListItem> PreviousItems { get; set; }
        public List<SelectListItem> NextItems { get; set; }
    }
}