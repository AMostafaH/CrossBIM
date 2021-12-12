using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CrossBIM.Pages.DXFToIFC
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string ProjectName { get; set; }
        [BindProperty]
        public string ProjectUnits { get; set; }
        [BindProperty]
        public string Message { get; set; }
        [BindProperty]
        public string FootingStatus { get; set; }
        public IWebHostEnvironment Hosting { get; }
        public IndexModel(IWebHostEnvironment hosting)
        {
            Hosting = hosting;
        }
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {

            var IFCFilePath = Path.Combine(Hosting.WebRootPath, "Uploads", $"{ProjectName}.ifc");

            if (System.IO.File.Exists(IFCFilePath))
            {
                Message = "Please Choose a unique name for your project";
                return Page();
            }

            if (ProjectName == null)
            {
                Message = "Please Choose a name for your project";
                return Page();
            }

            if(FootingStatus == "Yes")
            {
                return RedirectToPage("./IsoFootingBasicDetails", new
                {
                    projectName = ProjectName,
                    projectUnits = ProjectUnits
                });
            }
            else
            {
                return RedirectToPage("./StoreyBasicDetails", new
                {
                    projectName = ProjectName,
                    projectUnits = ProjectUnits
                });
            }
        }
    }
}
