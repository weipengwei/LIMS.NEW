using System;
using System.IO;
using System.Web;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZXing;
using ZXing.QrCode;

namespace LIMS.Util
{
    public class BarcodeHelper
    {
        private const int IMAGE_WIDTH = 300;
        private const int IMAGE_HEIGHT = 60;

        public static string CreateImg(string barcode)
        {
            var fileName = string.Format("~/Content/temp/{0}.jpg", barcode);
            fileName = HttpContext.Current.Server.MapPath(fileName);

            using (var img = DrawingBarcode(barcode))
            {
                if(File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                img.Save(fileName);
            }

            return fileName;
        }

        private static Bitmap DrawingBarcode(string barcode)
        {
            var options = new QrCodeEncodingOptions
            {
                CharacterSet = "utf-8",
                Width = IMAGE_WIDTH,
                Height = IMAGE_HEIGHT,
                Margin = 0,
                PureBarcode = false
            };
            var writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.CODE_128;
            writer.Options = options;

            return writer.Write(barcode);
        }
    }
}
