using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIMS.Util;

namespace LIMS.Models
{
    public class UnitModel : ResponseResult
    {
        public string Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public UnitType Type
        {
            get; set;
        }

        public string ShortCode
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
        public string ReceiptId
        {
            get; set;
        }

        public string ReceiptTitle
        {
            get; set;
        }

        public decimal Tax
        {
            get; set;
        }

        public string ParentId
        {
            get; set;
        }

        public string RootId
        {
            get; set;
        }

        public ReceiptInfoModel Receipt
        {
            get; set;
        }

        public UnitBusinessType BusinessType
        {
            get; set;
        }
    }
}
