using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public class GoodsBarModel
    {
        public string Id
        {
            get; set;
        }

        public int FormNo
        {
            get; set;
        }

        public string Barcode
        {
            get; set;
        }

        public string ProductName
        {
            get; set;
        }

        public bool IsValid
        {
            get; set;
        }
    }
}
