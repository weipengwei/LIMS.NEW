using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public class TargetHospitalModel
    {
        public string Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public bool Selected
        {
            get; set;
        }
    }
}
