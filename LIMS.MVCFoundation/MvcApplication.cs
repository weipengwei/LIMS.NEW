using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using LIMSHelper = LIMS.MVCFoundation.Helpers;
using LIMS.MVCFoundation.Configs;
using LIMS.MVCFoundation.Controllers;
using LIMS.MVCFoundation.Core;
using LIMS.MVCFoundation.CustomExceptions;

using LIMS.Models;

namespace LIMS.MVCFoundation
{
    public class MvcApplication : System.Web.HttpApplication
    {
        void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            ViewEngines.Engines.Clear();
           // ViewEngines.Engines.Insert(0, new CustomRazorViewEngine());

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        void Application_OnPostAuthenticateRequest(object sender, EventArgs e)
        {
            LIMSHelper.UrlHelper urlHelper = new LIMSHelper.UrlHelper(Context.Request.RequestContext);
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket.Expired == false)
                {
                    CustomPrincipal principal = new CustomPrincipal(authTicket.Name, authTicket.UserData);
                    
                    principal.ClientIp = urlHelper.ClientIP;
                    HttpContext.Current.User = principal;
                    Thread.CurrentPrincipal = principal;
                }
            }
        }

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    Exception ex = Server.GetLastError();
        //    if (ex == null)
        //    {
        //        return;
        //    }

        //    HttpContext ctx = HttpContext.Current;

        //    ctx.Response.TrySkipIisCustomErrors = true;
        //    ctx.Response.Clear();
        //    Server.ClearError();

        //    if (ex is UnauthenticationException)
        //    {
        //        var url = SecurityRequestHandler.UnauthenticationRequestUrl(new RequestContext(new HttpContextWrapper(ctx), new RouteData()));
        //        ctx.Response.Redirect(url);

        //        return;
        //    }

        //    var action = "Info";
        //    if (ex is UnauthorizedException)
        //    {
        //        action = "Unauthorized";
        //    }

        //    RouteData routeData = new RouteData();
        //    routeData.Values.Add("controller", "Error");
        //    routeData.Values.Add("action", action);
        //    routeData.Values.Add("Exception", ex);
        //    IController errorController = new ErrorController();
        //    errorController.Execute(new RequestContext(new HttpContextWrapper(ctx), routeData));
        //    ctx.Response.End();
        //}
    }
}
