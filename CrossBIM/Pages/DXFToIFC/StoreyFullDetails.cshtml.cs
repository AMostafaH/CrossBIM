using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CrossBIM.HelperClasses;
using CrossBIM.ViewModelClasses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CrossBIM.Pages.DXFToIFC
{
    public class StoreyFullDetailsModel : PageModel
    {
        [BindProperty]
        public string ProjectName { get; set; }
        [BindProperty]
        public ViewModelBeams ViewModelBeam { get; set; }
        [BindProperty]
        public ViewModelSlabs ViewModelSlab { get; set; }
        [BindProperty]
        public ViewModelStoreys ViewModelStorey { get; set; } = new ViewModelStoreys();
        [BindProperty]
        public ViewModelColumns ViewModelColumn { get; set; }
        [BindProperty]
        public IFormFile RefFile { get; set; }
        [BindProperty]
        public int SlabsNumberOfCategories { get; set; }
        [BindProperty] 
        public int BeamsNumberOfCategories { get; set; }
        [BindProperty]
        public bool Status { get; set; }
        [BindProperty]
        public string ProjectUnits { get; set; }
        public IWebHostEnvironment Hosting { get; }
        public StoreyFullDetailsModel(IWebHostEnvironment hosting)
        {
            Hosting = hosting;
        }
        public void OnGet(int NumOfRepetitions, int beamsCategory, int slabsCategory, string projectName, List<string> layersNames, string filePath, double storeyHeight, double storeyElevation, string projectUnits)
        {
            ViewModelStorey.NumOfRepetitions = NumOfRepetitions;
            ViewModelStorey.Elevation = storeyElevation;
            ViewModelStorey.Height = storeyHeight;
            ViewModelStorey.SlabsNumberOfCategories = slabsCategory;
            ViewModelStorey.BeamsNumberOfCategories = beamsCategory;
            ViewModelStorey.DXFLayersNames = layersNames;
            ViewModelStorey.DXFFilePath = filePath;

            ProjectName = projectName;
            ProjectUnits = projectUnits;
        }
        //public void OnGet(string projectName,ViewModelStoreys viewModelStorey)
        //{
        //    ViewModelStorey = viewModelStorey;
        //    ProjectName = projectName;
        //}
        public IActionResult OnPost()
        {
            Status = true;// to show Covert to IFC button

            var IFCFileName = $"{ProjectName}.ifc";
            var xmlFileName = $"{ProjectName}.ifcxml";

            var IFCFilePath = Path.Combine(Hosting.WebRootPath, "Uploads", IFCFileName);

            HelperFunctions.GetIFCFile(ViewModelStorey.DXFFilePath, IFCFilePath, ViewModelBeam, ViewModelSlab, ViewModelColumn, ViewModelStorey,ProjectUnits);
            /*
            // To Solve Error The process cannot access the file because it is being used by another process
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
            */
            return RedirectToPage("./StoreyBasicDetails", new
            {
                IfcFileName = IFCFileName, //has Extension .ifc
                projectName = ProjectName, //the same name but without .ifc
                status = Status,
                projectUnits = ProjectUnits
            });
        }
        
    }
    
}
