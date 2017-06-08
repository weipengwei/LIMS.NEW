using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public class LoginInfoModel
    {
        public string UserName
        {
            get; set;
        }

        public IList<TargetHospitalModel> Hospitals
        {
            get; set;
        }

        public bool IsAdmin
        {
            get; set;
        }

        public string RootUnitName
        {
            get; set;
        }
        public bool HospitalOrVendor
        {
            get; set;
        }
    }

}
