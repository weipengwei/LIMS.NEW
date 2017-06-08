using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public class LogonRequestModel
    {
        public string Account
        {
            get; set;
        }

        public string Password
        {
            get; set;
        }

        public string ReturnUrl
        {
            get; set;
        }
    }
}
