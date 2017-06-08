using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

using LIMS.Util;
using LIMS.Models;

namespace LIMS.MVCFoundation.Core
{
    public class CustomPrincipal : IPrincipal
    {
        public IIdentity Identity { get; private set; }

        public CustomPrincipal(string name, string ticketUserData)
        {
            this.Identity = new GenericIdentity(name);

            string userId;
            if (!ParseTicketUserData(ticketUserData, out userId))
            {
                throw new FormatException("UserData format error");
            }
            this.UserId = userId;

            this.Context = new UserContext(this.UserId);
        }

        public bool IsInRole(string role)
        {
            if (this.Context.PrivilegeUnits != null)
            {
                return Context.PrivilegeUnits.Contains(role, StringComparer.InvariantCultureIgnoreCase);
            }
            return false;
        }
        
        public static string MakeTicketUserData(string userId)
        {
            return string.Format("{0}_{1}_***", Constant.SYSTEM_NAME, userId);
        }

        private static bool ParseTicketUserData(string userDate, out string userId)
        {
            userId = string.Empty;

            var values = userDate.Split('_');
            if(values.Length != 3)
            {
                return false;
            }

            if(string.Compare(values[0], Constant.SYSTEM_NAME, false) != 0)
            {
                return false;
            }

            if (values[1].Length >= 1)
            {
                userId = values[1];
                return true;
            }
            else
            {
                return false;
            }
        }

        public string Name
        {
            get { return this.Identity.Name; }
        }

        /// <summary>
        /// 登陆用户ID 加密 在AuthCookie UserData
        /// </summary>
        public string UserId
        {
            get;
            private set;
        }

        /// <summary>
        /// 用户的IP
        /// </summary>
        public string ClientIp
        {
            get;
            set;
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }
        
        /// <summary>
        /// 用户上下文
        /// </summary>
        public UserContext Context
        {
            get;
            private set;
        }
    }
}
