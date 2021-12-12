using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CrossBIM.ViewModelClasses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using netDxf;

namespace CrossBIM.Pages.DXFToIFC
{
    public class IsoFootingBasicDetailsModel : PageModel
    {
        [BindProperty]
        public IFormFile RefFile { get; set; }
        [BindProperty]
        public ViewModelIsoFooting ViewModelIsoFooting { get; set; }
        [BindProperty]
        public string ProjectName { get; set; }
        [BindProperty]
        public string ProjectUnits { get; set; }
        [BindProperty]
        public string Message { get; set; }
        public IWebHostEnvironment Hosting { get; }
        public IsoFootingBasicDetailsModel(IWebHostEnvironment hosting)
        {
            Hosting = hosting;
        }
        public void OnGet(string projectName, string projectUnits)
        {
            ProjectName = projectName;
            ProjectUnits = projectUnits;
        }
        public IActionResult OnPost()
        {
            List<string> IsolatedFootingLayersName;
            
            if (RefFile?.FileName != null)
            {

                var fileName = Path.GetFileName(RefFile.FileName);
                // Get New Path related to project
                ViewModelIsoFooting.DXFFilePath = Path.Combine(Hosting.WebRootPath, "Uploads", fileName);
                // Copy File To the New Path in WWWroot
                using (var newStream = new FileStream(ViewModelIsoFooting.DXFFilePath, FileMode.Create))
                {
                    RefFile.CopyTo(newStream);
                }

                DxfDocument dxfDocumentLoaded = DxfDocument.Load(ViewModelIsoFooting.DXFFilePath);

                IsolatedFootingLayersName = dxfDocumentLoaded.Layers.Names.ToList();
            }
            else
            {
                Message = "Please Upload Your DXF File!";
                return Page();
            }

            if (ViewModelIsoFooting.FootingNumberOfCategories < 0 ||
                ViewModelIsoFooting.StripsNumberOfCategories < 0)
            {
                Message = "Please Enter Correct Data!";
                return Page();
            }

            
             
            return RedirectToPage("./IsoFootingFullDetails", new
            {
                footingNumOfCategories = ViewModelIsoFooting.FootingNumberOfCategories,
                stripsNumofCategories = ViewModelIsoFooting.StripsNumberOfCategories,
                layersName = IsolatedFootingLayersName,
                filePath = ViewModelIsoFooting.DXFFilePath,
                projectName = ProjectName,
                projectUnits = ProjectUnits
            });
        }
    }
}
