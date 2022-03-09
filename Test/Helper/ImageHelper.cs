using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Helper
{
    class ImageHelper
    {
        public static IFormFile GetImage()
        {
            var imageCurretPath = Directory.GetCurrentDirectory();
            var index = imageCurretPath.IndexOf("Test\\");
            var finalPath = imageCurretPath.Substring(0, index + 4) + "\\Image\\Captura1.png";
            var imageFile = File.OpenRead(finalPath);
            IFormFile image = new FormFile(imageFile, 0, imageFile.Length, "Captura1", "Captura1.png")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };
            return image;
        }
    }
}
