using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

using LIMS.Util;
using LIMS.Models;
using LIMS.Entities;
using LIMS.Services;
using LIMS.MVCFoundation.Controllers;
using LIMS.MVCFoundation.Core;
using LIMS.MVCFoundation.Helpers;

namespace LIMS.Web.Controllers
{
    public class LogonController : BaseController
    {
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            this.ClearContext();

            return View();
        }

        [HttpPost]
        public JsonNetResult Validate(LogonRequestModel logon)
        {
            UserEntity user;
            if (UserService.TryGetUserByAccount(logon.Account, out user)
                && SecurityHelper.ValidatePassword(logon.Password, user.Password))
            {
                if (false)//user.IsChangePassword)
                {
                    return JsonNet(new LogonResponseModel
                    {
                        RedirectUrl = this.Url.Content("~/Logon/ChangePassword")
                    });
                }
                else
                {
                    SecurityHelper.CreateTicket(this.Response, user);

                    return JsonNet(new LogonResponseModel
                    {
                        IsSuccess = true,
                        RedirectUrl = string.IsNullOrEmpty(logon.ReturnUrl) ? this.Url.Content("~/Main/Index") : logon.ReturnUrl
                    });
                }
            }
            else
            {
                return JsonNet(new ResponseResult(false, "账号或密码不存在或不匹配！"));
            }
        }

        //[HttpPost]
        //public JsonNetResult Validate2(LogonRequestModel logon)
        //{
        //    UserEntity user;
        //    if (UserService.TryGetUserByAccount(logon.Account, out user)
        //        && SecurityHelper.ValidatePassword(logon.Password, user.Password))
        //    {
        //        SecurityHelper.CreateTicket(this.Response, user);

        //        return JsonNet(new LogonResponseModel
        //        {
        //            IsSuccess = true
        //        });
        //    }
        //    else
        //    {
        //        return JsonNet(new ResponseResult(false, "账号或密码不存在或不匹配！"));
        //    }
        //}
    }
}
