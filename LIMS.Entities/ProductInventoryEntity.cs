using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace LIMS.Entities
{
    public class ProductInventoryEntity
    {
        public string StoreroomId
        {
            get;set;
        }

        public string StoreroomName
        {
            get;set;
        }

        public string ProductId
        {
            get; set;
        }

        public string ProductName
        {
            get; set;
        }

        public bool NeedSplit
        {
            get;set;
        }

        public int SplitableCount
        {
            get;set;
        }

        public int ApplyCount
        {
            get;set;
        }

        public string SerialNo
        {
            get; set;
        }

        public string BatchNo
        {
            get; set;
        }

        public string VendorID
        {
            get; set;
        }

        public string VendorName
        {
            get; set;
        }

        public string BrandName
        {
            get; set;
        }

        public string MiniPackageSpec
        {
            get; set;
        }

        public DateTime ReceivedDate
        {
            get; set;
        }

        public DateTime ExpiredDate
        {
            get; set;
        }
    }
}
