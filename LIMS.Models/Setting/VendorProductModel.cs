using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public class VendorProductModel : ResponseResult
    {
        public string Id
        {
            get; set;
        }

        public string VendorId
        {
            get; set;
        }

        public string ProductId
        {
            get; set;
        }

        public string ProductName
        {
            get; set;
        }

        public string ProductDescription
        {
            get; set;
        }

        public string ProductShortCode
        {
            get; set;
        }

        public string ContactId
        {
            get; set;
        }

        public ContactInfoModel ContactInfo
        {
            get; set;
        }
    }
}
