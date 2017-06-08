using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using System.Threading.Tasks;

using LIMS.Util;
using LIMS.Models;
using LIMS.Entities;
using LIMS.Services;
using LIMS.MVCFoundation.Attributes;
using LIMS.MVCFoundation.Controllers;
using LIMS.MVCFoundation.Core;

namespace LIMS.Web.Controller
{
    [RequiredLogon]
    public class MainController : BaseController
    {
        public ActionResult Index()
        {

            return View();
        }

        public PartialViewResult Menus()
        {
            var mainMenus = new MainMenusModel
            {
                Menus = new List<MenuModel>(),
                IsAdmin = this.IsAdmin
            };

            IList<SystemFunctionEntity> funs;
            if(this.IsAdmin)
            {
                funs = new List<SystemFunctionEntity>();
                funs.Add(new SystemFunctionService().GetSettingFunction());
            }
            else
            {
                if (this.UserContext.HospitalOrVendor)
                {
                    var hospitalId = string.IsNullOrEmpty(this.UserContext.CurrentHospital)
                        ? this.UserContext.RootUnitId : this.UserContext.CurrentHospital;
                    funs = new SystemFunctionService().GetUserFunctions(hospitalId, this.UserContext.UserId);
                }
                else
                {
                    funs = new SystemFunctionService().GetUserFunctions(this.UserContext.RootUnitId, this.UserContext.UserId);
                }
            }
            
            foreach(var fun in funs)
            {
                var menu = new MenuModel
                {
                    Id = fun.Id,
                    Title = fun.Title,
                    Url = fun.Url,
                    SubMenus = new List<MenuModel>()
                };
                menu.SubMenus = fun.SubFunctions.Select(item => new MenuModel
                {
                    Id = item.Id,
                    Title = item.Title,
                    Url = item.Url
                }).ToList();

                mainMenus.Menus.Add(menu);
            }

            return PartialView("~/Views/Main/_Menus.cshtml", mainMenus);
        }

        public PartialViewResult LoginInfo()
        {
            var loginInfo = new LoginInfoModel
            {
                IsAdmin = this.IsAdmin,
                UserName = this.UserContext.Name,
                RootUnitName = this.UserContext.RootUnitName,
                HospitalOrVendor = this.UserContext.HospitalOrVendor
            };

            var hospitalId = string.Empty;
            if (this.UserContext.HospitalOrVendor)
            {
                hospitalId = string.IsNullOrEmpty(this.UserContext.CurrentHospital)
                    ? this.UserContext.RootUnitId : this.UserContext.CurrentHospital;

                var found = false;
                loginInfo.Hospitals = new UnitService().GetHospitalsByUserId(this.UserContext.UserId).Select(item =>
                {
                    var result = new TargetHospitalModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Selected = string.Compare(hospitalId, item.Id, true) == 0
                    };

                    if (!found)
                    {
                        found = result.Selected;
                    }
                    return result;
                }).ToList();

                hospitalId = found ? hospitalId : this.UserContext.RootUnitId;
            }
            else
            {
                loginInfo.Hospitals = new UnitService().GetHospitalsByVendor(this.UserContext.RootUnitId).Select(item =>
                new TargetHospitalModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Selected = string.Compare(this.UserContext.CurrentHospital, item.Id, true) == 0
                }).ToList();

                hospitalId = string.IsNullOrEmpty(this.UserContext.CurrentHospital)
                    ? (loginInfo.Hospitals.Count > 0 ? loginInfo.Hospitals[0].Id : "") : this.UserContext.CurrentHospital;
                
            }

            InitCookie(hospitalId);

            return PartialView("~/Views/Main/_LoginInfo.cshtml", loginInfo);
        }

        public JsonNetResult ChangeHospital(string id)
        {
            this.InitCookie(id);

            return JsonNet(new ResponseResult());
        }

        public ActionResult Logout()
        {
            this.ClearContext();
            FormsAuthentication.SignOut();

            return new RedirectResult("~/Logon");
        }

        private void InitCookie(string hospitalId)
        {
            HttpCookie cookie = this.Request.Cookies[Constant.CURRENT_HOSPITAL_COOKIE];
            if (cookie == null)
            {
                cookie = new HttpCookie(Constant.CURRENT_HOSPITAL_COOKIE, hospitalId);
            }
            else
            {
                cookie.Value = hospitalId;
            }
            this.Request.Cookies.Remove(Constant.CURRENT_HOSPITAL_COOKIE);

            cookie.HttpOnly = false;
            this.Response.Cookies.Add(cookie);
        }
    }
}
