using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public class OrderQueryCondition : DateRangeCondition
    {
        public string VendorId
        {
            get; set;
        }

        private IList<string> m_Status = new List<string>();
        public IList<string> Status
        {
            get
            {
                return m_Status;
            }
        }

        public string VendorName
        { get; set; }

        public string ProductName
        { get; set; }
    }
}
