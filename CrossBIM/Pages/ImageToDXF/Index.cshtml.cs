using System;
using System.IO;
using CrossBIMLib;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CrossBIM.Pages.ImageToDXF
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string ProjectName { get; set; }
        [BindProperty]
        public IFormFile RefFile { get; set; }
        [BindProperty]
        public string Message { get; set; }
        public IWebHostEnvironment Hosting { get; }
        public IndexModel(IWebHostEnvironment hosting)
        {
            Hosting = hosting;
        }
        public void OnGet()
        {
        }
        public IActionResult OnPostDownloadFile()
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

            var DXFFileName = $"{ ProjectName }.dxf";

            if (RefFile?.FileName != null)
            {

                var fileName = Path.GetFileName(RefFile.FileName);
                // Get New Path related to project
                var filePath = Path.Combine(Hosting.WebRootPath, "Uploads", fileName);
                // Copy File To the New Path in WWWroot
                using (var newStream = new FileStream(filePath, FileMode.Create))
                {
                    RefFile.CopyTo(newStream);
                }
                string dXFFilePath = Path.Combine(Hosting.WebRootPath, "Uploads", DXFFileName);
                //DxfDocument dxfDocumentLoaded = DxfDocument.Load(filePath);

                try
                {
                    ImageProcessing.GetDxfFile(filePath, dXFFilePath);
                }

                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                Message = "please, Upload your file!";
                return Page();
            }


            //return RedirectToPage("./Download", new
            //{
            //    fileName = $"{ProjectName}.dxf"
            //});

            return File($@"\Uploads\{DXFFileName}", "application/x-step", DXFFileName);
        }
    }
}
