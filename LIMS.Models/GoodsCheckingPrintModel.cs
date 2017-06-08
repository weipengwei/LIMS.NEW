using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public class GoodsCheckingPrintModel
    {
        public string Barcode
        {
            get; set;
        }

        public string ProductName
        {
            get; set;
        }

        public string PackageCapacity
        {
            get; set;
        }

        public int PackageCount
        {
            get; set;
        }

        public int GrantCount
        {
            get; set;
        }

        public string StatusName
        {
            get; set;
        }
    }
}
