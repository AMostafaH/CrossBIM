using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CrossBIM.HelperClasses;
using CrossBIM.ViewModelClasses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CrossBIM.Pages.DXFToIFC
{
    public class IsoFootingFullDetailsModel : PageModel
    {
        [BindProperty]
        public ViewModelIsoFooting ViewModelIsoFooting { get; set; }
        [BindProperty]
        public ViewModelStoreys ViewModelStorey { get; set; } = new ViewModelStoreys();
        [BindProperty]
        public string ProjectName { get; set; }
        [BindProperty]
        public string ProjectUnits { get; set; }
        [BindProperty]
        public string IFCFileName { get; set; }
        [BindProperty]
        public int FootingNumberOfCategories { get; set; }
        [BindProperty]
        public int StripsNumberOfCategories { get; set; }
        public IWebHostEnvironment Hosting { get; }
        public IsoFootingFullDetailsModel(IWebHostEnvironment hosting)
        {
            Hosting = hosting;
        }
        public void OnGet(int footingNumOfCategories,int stripsNumofCategories, List<string> layersName,string filePath,string projectName,string projectUnits)
        {
            FootingNumberOfCategories = footingNumOfCategories;
            StripsNumberOfCategories = stripsNumofCategories;
            ViewModelStorey.DXFLayersNames = layersName;
            ViewModelStorey.DXFFilePath = filePath;

            ProjectName = projectName;
            ProjectUnits = projectUnits;
        }
        public IActionResult OnPost()
        {

            //var FilePath = ViewModelStorey.DXFFilePath;

            var IFCFileName = $"{ProjectName}.ifc";//"First Building8-6.ifc";
            //var xmlFileName = $"{ProjectName}.ifcxml";

            ViewModelIsoFooting.FootingNumberOfCategories = FootingNumberOfCategories;
            ViewModelIsoFooting.StripsNumberOfCategories = StripsNumberOfCategories;

            var IFCFilePath = Path.Combine(Hosting.WebRootPath, "Uploads", IFCFileName);

            HelperFunctions.IFCForIsolatedFooting(ViewModelStorey.DXFFilePath, IFCFilePath, ViewModelIsoFooting, ProjectUnits);

            return RedirectToPage("./StoreyBasicDetails", new
            {
                status = true,
                IfcFileName = IFCFileName, //has Extension .ifc
                projectName = ProjectName, //the same name but without .ifc
                projectUnits = ProjectUnits
            });
        }
        
    }
}