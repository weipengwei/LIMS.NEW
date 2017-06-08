using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public class ProductInventoryCondition : BaseCondition
    {
        public string StoreroomId
        {
            get; set;
        }

        public string ProductId
        {
            get; set;
        }

        public string NotIncludeZero
        {
            get; set;
        }
    }
}
