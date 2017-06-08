using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public class DateRangeCondition : BaseCondition
    {

        public string Content
        {
            get; set;
        }

        public DateTime? BeginDate
        {
            get; set;
        }

        public DateTime? EndDate
        {
            get; set;
        }

        Dictionary<string, object> _AdditionalFilters = new Dictionary<string, object>();
        public Dictionary<string, object> AdditionalFilters
        {
            get { return _AdditionalFilters; }
        }
    }
}
