using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public class BarcodeQueryCondition : DateRangeCondition
    {
        public string BarcodeNo
        {
            get; set;
        }
    }
}
