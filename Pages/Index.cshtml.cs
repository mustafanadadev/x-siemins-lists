using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace x_siemens_dropdown.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        public List<BusinessCapability> businessCapabilities = new List<BusinessCapability>();

        public IndexModel(IWebHostEnvironment env)
        {
            _env = env;

            // Load Lv1 options
            var jsonFilePath = Path.Combine(_env.WebRootPath, "lists.json");
            var jsonData = System.IO.File.ReadAllText(jsonFilePath);
            this.businessCapabilities = JsonConvert.DeserializeObject<List<BusinessCapability>>(jsonData) ?? new List<BusinessCapability>();

            // Initialize Level 1 options
            Level1Options = businessCapabilities
                .Where(bc => bc.BusinessCapabilityLevel == 1)
                .Select(bc => new SelectListItem { Value = bc.BusinessCapabilittyName, Text = bc.BusinessCapabilittyName })
                .ToList();
        }

        public List<SelectListItem> Level1Options { get; set; }
        public List<SelectListItem> Level2Options { get; set; }
        public List<SelectListItem> Level3Options { get; set; }
        public List<string> SelectedLevel1 { get; set; }
        public List<string> SelectedLevel2 { get; set; }
        public List<string> SelectedLevel3 { get; set; }
      
        public IActionResult OnGetShowSelected(string[] selectedLevel1, string[] selectedLevel2, string[] selectedLevel3)
        {
            // Process selected levels and return the result
            var result = new
            {
                SelectedLevel1 = string.Join(", ", selectedLevel1.Select(x => x.Split('.')[0]).ToList()),
                SelectedLevel2 = string.Join(", ", selectedLevel2.Select(x => x.Split('.')[0] + '.' + x.Split('.')[1]).ToList()),
                SelectedLevel3 = string.Join(", ", selectedLevel3.Select(x => x.Split('.')[0] + '.' + x.Split('.')[1] + '.' + x.Split('.')[2]).ToList())
            };

            return new JsonResult(result);
        }

        public IActionResult OnGetLoadLevel2Dropdown(string[] selectedLevel1)
        {
            var selected = selectedLevel1.Select(x => x.Split('.')[0]).ToList();

            // Filter Level 2 options based on selected Level 1
            var level2Options = businessCapabilities
                .Where(bc => bc.BusinessCapabilityLevel == 2 && selected.Any(x => bc.BusinessCapabilittyName.StartsWith(x)))
                .Select(bc => new SelectListItem { Value = bc.BusinessCapabilittyName, Text = bc.BusinessCapabilittyName })
                .ToList();

            return new JsonResult(level2Options);
        }

        public IActionResult OnGetLoadLevel3Dropdown(string[] selectedLevel2)
        {
            var selected = selectedLevel2.Select(x => x.Split('.')[0] + '.' + x.Split('.')[1]).ToList();

            // Filter Level 3 options based on selected Level 2
            var level3Options = businessCapabilities
                .Where(bc => bc.BusinessCapabilityLevel == 3 && selected.Any(x => bc.BusinessCapabilittyName.StartsWith(x)))
                .Select(bc => new SelectListItem { Value = bc.BusinessCapabilittyName, Text = bc.BusinessCapabilittyName })
                .ToList();

            return new JsonResult(level3Options);
        }
    }

    public class BusinessCapability
    {
        public int Id { get; set; }
        public int BusinessCapabilityLevel { get; set; }
        public string BusinessCapabilittyName { get; set; }
    }
}
