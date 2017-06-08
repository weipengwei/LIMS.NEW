using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public class CheckCondition : BaseCondition
    {
        public string StoreroomId
        {
            get;set;
        }

        public string Status
        {
            get; set;
        }
    }

    public class CheckScanCondition : CheckCondition
    {

    }
}
