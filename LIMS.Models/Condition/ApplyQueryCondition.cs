using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public class ApplyQueryCondition : BaseCondition
    {
        public int? FormNo
        {
            get; set;
        }

        public string Status
        {
            get; set;
        }

        public bool NotIncludeNotGrant
        {
            get; set;
        }
    }
}
