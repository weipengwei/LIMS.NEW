using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public class UserModel : ResponseResult
    {
        public string Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string Account
        {
            get; set;
        }

        public string Password
        {
            get; set;
        }

        public string ValidPassword
        {
            get; set;
        }

        public string WeiXingId
        {
            get; set;
        }

        public string UnitId
        {
            get; set;
        }

        public string UnitName
        {
            get; set;
        }

        public bool IsChangePwd
        {
            get; set;
        }

        public int Title
        {
            get; set;
        }
    }
}
