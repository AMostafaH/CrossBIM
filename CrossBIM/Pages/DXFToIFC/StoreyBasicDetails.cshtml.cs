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
    public class StoreyBasicDetailsModel : PageModel
    {
        [BindProperty]
        public string ProjectName { get; set; }
        [BindProperty]
        public string IFCFileName { get; set; }
        [BindProperty]
        public ViewModelStoreys ViewModelStorey { get; set; }
        [BindProperty]
        public bool Status { get; set; }
        [BindProperty]
        public IFormFile RefFile { get; set; }
        [BindProperty]
        public string ProjectUnits { get; set; }
        [BindProperty]
        public string Message { get; set; }
        public IWebHostEnvironment Hosting { get; }
        public StoreyBasicDetailsModel(IWebHostEnvironment hosting)
        {
            Hosting = hosting;
        }
        public void OnGet(string projectName,string projectUnits, bool status,string IfcFileName)
        {
            Status = status;
            IFCFileName = IfcFileName; // used for download button //has Extension .ifc
            ProjectName = projectName;
            ProjectUnits = projectUnits;
        }
        public IActionResult OnPost()
        {
            if (RefFile?.FileName != null)
            {

                var fileName = Path.GetFileName(RefFile.FileName);
                // Get New Path related to project
                ViewModelStorey.DXFFilePath = Path.Combine(Hosting.WebRootPath, "Uploads", fileName);
                // Copy File To the New Path in WWWroot
                using (var newStream = new FileStream(ViewModelStorey.DXFFilePath, FileMode.Create))
                {
                    RefFile.CopyTo(newStream);
                }

                DxfDocument dxfDocumentLoaded = DxfDocument.Load(ViewModelStorey.DXFFilePath);
                ViewModelStorey.DXFLayersNames = dxfDocumentLoaded.Layers.Names.ToList();
            }
            else 
            {
                Message = "Please Upload Your DXF File!";
                return Page();
            }

            if (ViewModelStorey.Height <= 0 ||
                ViewModelStorey.NumOfRepetitions < 0 ||
                ViewModelStorey.SlabsNumberOfCategories < 0 ||
                ViewModelStorey.BeamsNumberOfCategories < 0 )
            {
                Message = "Please Enter Correct Data!";
                return Page();
            }

                return RedirectToPage("./StoreyFullDetails",
            new {

                beamsCategory = ViewModelStorey.BeamsNumberOfCategories,
                slabsCategory = ViewModelStorey.SlabsNumberOfCategories,
                layersNames = ViewModelStorey.DXFLayersNames,
                filePath = ViewModelStorey.DXFFilePath,
                storeyHeight = ViewModelStorey.Height,
                storeyElevation = ViewModelStorey.Elevation,
                NumOfRepetitions = ViewModelStorey.NumOfRepetitions,
                //viewModelStorey = ViewModelStorey,
                projectName = ProjectName,
                projectUnits = ProjectUnits
            });
        }
    }
}
