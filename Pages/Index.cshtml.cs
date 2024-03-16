using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace x_siemins_dropdown.Pages
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

        public IActionResult OnPost(List<string> selectedLevel1, List<string> selectedLevel2, List<string> selectedLevel3)
        {
            SelectedLevel1 = selectedLevel1.Select(x => x.Split('.')[0]).ToList();

            SelectedLevel2 = selectedLevel2.Select(x => x.Split('.')[0] + '.' + x.Split('.')[1]).ToList();

            SelectedLevel3 = selectedLevel3.Select(x => x.Split('.')[0] + '.' + x.Split('.')[1] + '.' + x.Split('.')[2]).ToList();

            
            return Page();
        }

        public IActionResult OnGetLoadLevel2Dropdown(string[] selectedLevel1)
        {

            var selected = selectedLevel1.Select(x => x.Split('.')[0]).ToList();


            var level2Options = this.businessCapabilities
                .Where(bc => bc.BusinessCapabilityLevel == 2 && selected.Any(x => bc.BusinessCapabilittyName.StartsWith(x)))
                .Select(bc => new SelectListItem { Value = bc.BusinessCapabilittyName, Text = bc.BusinessCapabilittyName })
                .ToList();

            return new JsonResult(level2Options);
        }

        public IActionResult OnGetLoadLevel3Dropdown(string[] selectedLevel2)
        {
            var selected = selectedLevel2.Select(x => x.Split('.')[0] + '.' + x.Split('.')[1]).ToList();

            var level3Options = this.businessCapabilities
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
