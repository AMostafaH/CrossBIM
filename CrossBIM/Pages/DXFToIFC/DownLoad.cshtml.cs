using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xbim.Common.Geometry;
using Xbim.Ifc;
using Xbim.IO;
using Xbim.ModelGeometry.Scene;
using CrossBIMLib;

namespace CrossBIM.Pages.DXFToIFC
{
    public class DownLoadModel : PageModel
    {
        [BindProperty]
        public string IFCFileName { get; set; }
        [BindProperty]
        public string WexBIMUploadedFileUrl { get; set; }
        public IWebHostEnvironment Hosting { get; }
        public DownLoadModel(IWebHostEnvironment _Hosting)
        {
            Hosting = _Hosting;
        }
        public void OnGet(string fileName)
        {
            IFCFileName = fileName; //fileName; //has Extension .ifc

            string wexBIMFileName = IFCFileName.Replace(".ifc", ".wexBIM"); //"CrossBIMBuilding2.wexBIM";

            var filePath = Path.Combine(Hosting.WebRootPath, "Uploads", IFCFileName);

            string WexBimFilePath = ConverIfcToWexBim(filePath);

            var uploadParams = new RawUploadParams()
            {
                // by default, ResourceType is already set to "raw"
                File = new FileDescription(WexBimFilePath),
                Folder = "",//Folder Name
                PublicId = wexBIMFileName  //File New Name
            };

            /*
            Account account = new Account(
                  "Cloud",//user name
                  "APIKey",
                  "APISecert");
            */

            Account account = _Cloudinary.GetCloudinaryAccount();
            Cloudinary cloudinary = new Cloudinary(account);

            var uploadResult = cloudinary.Upload(uploadParams);

            WexBIMUploadedFileUrl = uploadResult.SecureUrl.ToString();
            
        }
        public ActionResult OnPostDownloadFile()
        {
            return File($@"\Uploads\{IFCFileName}", "application/x-step", IFCFileName);
        }

        private static string ConverIfcToWexBim(string filePath)
        {
            //var filePath = @$"~/Uploads/{fileName}";

            var wexBimFullPath = "";

            using (var model = IfcStore.Open(filePath))
            {

                var context = new Xbim3DModelContext(model);

                context.CreateContext();

                wexBimFullPath = Path.ChangeExtension(filePath, "wexBIM");

                var wexBimFileName = Path.GetFileName(wexBimFullPath);

                //ConfigurationManager.AppSettings.Set("wexBIMFileName", wexBimFileName);
                //ConfigurationManager.AppSettings.Set("wexBIMFullPath", "~/Uploads/" + wexBimFileName);

                using (var wexBiMfile = System.IO.File.Create(wexBimFullPath))
                {
                    using (var wexBimBinaryWriter = new BinaryWriter(wexBiMfile))
                    {
                        model.SaveAsWexBim(wexBimBinaryWriter);
                        wexBimBinaryWriter.Close();
                    }
                    wexBiMfile.Close();
                }

            }
            return wexBimFullPath;
        }
    }
}
