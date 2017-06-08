using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public class ReceiptInfoModel : ResponseResult
    {
        public string Id
        {
            get; set;
        }

        public string HospitalId
        {
            get; set;
        }

        public string Title
        {
            get; set;
        }

        public decimal Tax
        {
            get; set;
        }
    }
}
