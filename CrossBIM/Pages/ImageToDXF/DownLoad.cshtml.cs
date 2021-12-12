using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CrossBIM.Pages.ImageToDXF
{
    public class DownLoadModel : PageModel
    {
        [BindProperty]
        public string DXFFileName { get; set; }
        public void OnGet(string fileName)
        {
            DXFFileName = fileName;
        }
        public ActionResult OnPostDownloadFile()
        {
            return File($@"\Uploads\{DXFFileName}", "application/x-step", DXFFileName);
        }
    }
}
