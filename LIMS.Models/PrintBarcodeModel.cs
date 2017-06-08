using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public class PrintBarcodeModel
    {
        public string Id
        {
            get; set;
        }

        public string Barcode
        {
            get; set;
        }

        public string SerialNo
        {
            get; set;
        }

        public string ProductId
        {
            get; set;
        }

        public string VendorId
        {
            get; set;
        }

        public DateTime ExpiredDate
        {
            get; set;
        }

        public string BatchNo
        {
            get; set;
        }

        public bool IsPrinted
        {
            get; set;
        }
    }
}
