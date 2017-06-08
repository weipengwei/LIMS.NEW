using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public class ReturnQueryCondition : DateRangeCondition
    {
        public int Status
        {
            get; set;
        }
    }
}
