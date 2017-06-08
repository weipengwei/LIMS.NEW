using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

using LIMS.Util;
using LIMS.Models;
using LIMS.MVCFoundation.Helpers;
using LIMS.MVCFoundation.Core;
using LIMS.MVCFoundation.CustomExceptions;
using LIMS.Services;

namespace LIMS.MVCFoundation.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class AuthorizationAttribute : RequiredLogonAttribute
    {
        static IDictionary<string, IList<string>> ms_UserFunctionsMap;
        static SystemFunctionService ms_Service;

        static AuthorizationAttribute()
        {
            ms_UserFunctionsMap = new Dictionary<string, IList<string>>();
            ms_Service = new SystemFunctionService();
        }

        public AuthorizationAttribute()
        {
        }

        public AuthorizationAttribute(params string[] funcodes)
        {
            this.FunctionCodes = funcodes;
        }

        protected bool isLogon = true;

        /// <summary>
        /// 登录验证，功能代码验证，其它验证的核心方法
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext))
            {
                isLogon = false;
                return false; //未登录 
            }
            else
            {
                isLogon = true;
                if (!AuthorizeFuctionCode(httpContext))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 验证功能代码权限
        /// </summary>
        /// <returns></returns>
        protected virtual bool AuthorizeFuctionCode(HttpContextBase httpContext)
        {
            bool result = false;
            
            if (FunctionCodes != null && this.FunctionCodes.Length > 0)
            {
                var authFunctionCodes = GetAuthorizeFunctionCodes();

                foreach (var code in FunctionCodes)
                {
                    if (authFunctionCodes.Contains(code))
                    {
                        result = true;

                        //设置当前使用的功能号
                        //ContextHelper.CurrentPrincipal.Context.FunctionCode = code;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 要求检验的功能编号
        /// </summary>
        protected string[] FunctionCodes
        {
            get;
            set;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (isLogon)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    var response = filterContext.HttpContext.Response;
                    //response.ClearHeaders();
                    //response.ClearContent();

                    response.ContentType = "application/json";

                    response.Headers.Add("SiteError", "true");
                    var data = new ResponseResult(false, "没有功能权限", ErrorCodes.Unauthorization);

                    if (data != null)
                    {
                        JsonTextWriter writer = new JsonTextWriter(response.Output);
                        JsonSerializer serializer = JsonSerializer.Create(JsonNetResult.DefaultSerializerSettings);
                        serializer.Serialize(writer, data);
                    }

                    response.StatusCode = 401;

                    response.End();
                }
                else
                {
                    throw new UnauthorizedException("没有功能权限");
                }
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }

        protected IList<string> GetAuthorizeFunctionCodes()
        {
            var userContext = ContextHelper.CurrentPrincipal.Context;

            var rootId = userContext.HospitalOrVendor ? userContext.CurrentHospital : userContext.RootUnitId;
            var key = GetUserKey(rootId, userContext.UserId);

            IList<string> funCodes;
            //TODO:
            //if(!ms_UserFunctionsMap.TryGetValue(key, out funCodes))
            {
                funCodes = new List<string>();

                var funsTree = ms_Service.GetUserFunctions(rootId, userContext.UserId);
                foreach (var item in funsTree)
                {
                    if(item.SubFunctions != null)
                    {
                        foreach(var subItem in item.SubFunctions)
                        {
                            funCodes.Add(subItem.FunKey);
                        }
                    }
                }

                ms_UserFunctionsMap[key] = funCodes;
            }

            return funCodes;
        }

        static string GetUserKey(string rootId, string userId)
        {
            return string.Format("__{0}_|_{0}__", rootId, userId);
        }
    }
}
