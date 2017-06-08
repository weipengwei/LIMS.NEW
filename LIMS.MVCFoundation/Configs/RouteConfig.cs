using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LIMS.MVCFoundation.Configs
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Logon", action = "Index", id = UrlParameter.Optional }
            );

            SetProfileBizRoute(routes);
            SetHospitalBizRoute(routes);
            SetVendorBizRoute(routes);
            SetSettingRoute(routes);
        }

        private static void SetProfileBizRoute(RouteCollection routes)
        {
            var dic = new RouteValueDictionary();

            dic = new RouteValueDictionary();
            dic.Add("viewfolder", "profile");
            dic.Add("UseNamespaceFallback", false);
            dic.Add("RouteName", "profile");

            routes.Add("profile", new Route(
                 "profile/{controller}/{action}",
                 new RouteValueDictionary(),
                 new RouteValueDictionary(),
                 dic,
                 new MvcRouteHandler()
            ));
        }

        private static void SetHospitalBizRoute(RouteCollection routes)
        {
            var dic = new RouteValueDictionary();
            dic.Add("viewfolder", "main");
            dic.Add("UseNamespaceFallback", false);
            dic.Add("RouteName", "main");

            routes.Add("main", new Route(
                 "main/{controller}/{action}",
                 new RouteValueDictionary(),
                 new RouteValueDictionary(),
                 dic,
                 new MvcRouteHandler()
            ));

            dic = new RouteValueDictionary();
            dic.Add("viewfolder", "hospital");
            dic.Add("UseNamespaceFallback", false);
            dic.Add("RouteName", "hospital");

            routes.Add("hospital", new Route(
                 "hospital/{controller}/{action}",
                 new RouteValueDictionary(),
                 new RouteValueDictionary(),
                 dic,
                 new MvcRouteHandler()
            ));
        }

        private static void SetVendorBizRoute(RouteCollection routes)
        {
            var dic = new RouteValueDictionary();
            dic.Add("viewfolder", "vendor");
            dic.Add("UseNamespaceFallback", false);
            dic.Add("RouteName", "vendor");

            routes.Add("vendor", new Route(
                 "vendor/{controller}/{action}",
                 new RouteValueDictionary(),
                 new RouteValueDictionary(),
                 dic,
                 new MvcRouteHandler()
            ));
        }

        private static void SetSettingRoute(RouteCollection routes)
        {
            var dic = new RouteValueDictionary();
            dic.Add("viewfolder", "setting");
            dic.Add("UseNamespaceFallback", false);
            dic.Add("RouteName", "setting");

            routes.Add("setting", new Route(
                 "setting/{controller}/{action}",
                 new RouteValueDictionary(),
                 new RouteValueDictionary(),
                 dic,
                 new MvcRouteHandler()
            ));
        }
    }
}
