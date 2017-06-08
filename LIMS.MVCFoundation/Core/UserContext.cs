using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Util;
using LIMS.Models;
using LIMS.Entities;
using LIMS.Services;

namespace LIMS.MVCFoundation.Core
{
    public class UserContext
    {

        public UserContext(string userId)
        {
            UserId = userId;

            if (!string.IsNullOrEmpty(UserId))
            {
                Init();
            }

        }

        #region User Context
        public string UserId
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public string Account
        {
            get;
            private set;
        }

        public string FunctionCode
        {
            get;
            set;
        }

        public string UnitId
        {
            get;
            private set;
        }

        public UnitType UnitType
        {
            get;
            private set;
        }
        
        public string RootUnitId
        {
            get;
            private set;
        }

        public string RootUnitName
        {
            get;
            private set;
        }

        public bool HospitalOrVendor
        {
            get; set;
        }
        
        public ICollection<string> PrivilegeUnits
        {
            get;
            set;
        }

        public string CurrentHospital
        {
            get;
            private set;
        }
        #endregion

        public bool Initialized
        {
            get;
            private set;
        }

        public void Init()
        {
            UserEntity entity;
            if(!UserService.TryGet(this.UserId, out entity))
            {
                throw new Exception("The user does not exist.");
            }

            this.Name = entity.Name;
            this.Account = entity.Account;
            this.UnitId = entity.UnitId;

            var unit = new UnitService().Get(this.UnitId);
            
            if (unit != null)
            {
                this.UnitType = unit.Type;
                this.RootUnitId = unit.RootId;
                this.HospitalOrVendor = unit.Type == UnitType.Hospital || unit.Type == Util.UnitType.HospitalUnit;
            }
            if (this.UnitId != this.RootUnitId)
            {
                this.RootUnitName = "";
                if (this.RootUnitId != null)
                {
                    var unitRoot = new UnitService().Get(this.RootUnitId);
                    this.RootUnitName = unitRoot.Name;
                }
            }
            else
                this.RootUnitName = unit.Name;
            


            var cookie = HttpContext.Current.Request.Cookies[Constant.CURRENT_HOSPITAL_COOKIE];
            if (cookie != null)
            {
                this.CurrentHospital = cookie.Value;
            }
            else
            {
                this.CurrentHospital = "";
            }

            Initialized = true;
        }
    }
}
