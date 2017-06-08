using System;
using System.Web.Mvc;
using System.Web.Routing;
using LIMS.MVCFoundation.Helpers;

namespace LIMS.MVCFoundation.Core
{
    public static class SecurityRequestHandler
    {
        public static string UnauthenticationRequestUrl(RequestContext ctx, string returnUrl = null)
        {
            string result = string.Empty;

            //TODO:
            returnUrl = "Main/Index";
            //var urlHelper = new LIMS.MVCFoundation.Helpers.UrlHelper(ctx);
            //if (returnUrl == null)
            //{
            //    returnUrl = urlHelper.GetReturnUrl(ctx.HttpContext.Request);
            //}
            
            result = string.Format("~/Logon?returnUrl={0}", returnUrl);

            return result;
        }

        public static bool IsLogon(RequestContext ctx)
        {
            bool result = false;
            CustomPrincipal customPrincipal = ctx.HttpContext.User as CustomPrincipal;
            if (customPrincipal != null)
            {
                result = true;
            }
            return result;
        }
    }
}
