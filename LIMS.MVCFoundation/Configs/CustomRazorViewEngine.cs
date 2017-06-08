//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Web.Mvc;
//using System.IO;

//using LIMS.MVCFoundation.Controllers;

//namespace LIMS.MVCFoundation.Configs
//{
//    / <summary>
//    / Gaia的ViewEnine 定制了RazorViewEngine的一些路径，弃用了Area
//    / </summary>
//    public class CustomRazorViewEngine : RazorViewEngine
//    {
//        public CustomRazorViewEngine()
//            : base()
//        {
//            AreaViewLocationFormats = null;
//            AreaMasterLocationFormats = null;
//            AreaPartialViewLocationFormats = null;

//            ViewLocationFormats = new[]
//            {
//                "~/Views/{1}/{0}.cshtml",
//                "~/Views/Shared/{0}.cshtml"
//            };
//            MasterLocationFormats = new[]
//            {
//                "~/Views/{1}/{0}.cshtml",
//                "~/Views/Shared/{0}.cshtml"
//            };
//            PartialViewLocationFormats = new[]
//            {
//                "~/Views/{1}/{0}.cshtml",
//                "~/Views/Shared/{0}.cshtml",
//            };

//            FileExtensions = new[]
//            {
//                "cshtml"
//            };

//        }
//        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
//        {
//            if (controllerContext.Controller is BaseController)
//            {
//                BaseController controller = controllerContext.Controller as BaseController;
//                if (!string.IsNullOrEmpty(controller.ViewFolder))
//                {
//                    if (!partialPath.StartsWith("~/Views/Shared/"))
//                    {
//                        partialPath = partialPath.Replace("~/Views/", string.Format("~/Views/{0}/", controller.ViewFolder));
//                    }
//                }
//            }
//            return base.CreatePartialView(controllerContext, partialPath);
//        }
//        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
//        {
//            if (controllerContext.Controller is BaseController)
//            {
//                BaseController controller = controllerContext.Controller as BaseController;
//                if (!string.IsNullOrEmpty(controller.ViewFolder))
//                {
//                    if (!virtualPath.StartsWith("~/Views/Shared/")) //替换路径
//                    {
//                        virtualPath = virtualPath.Replace("~/Views/", string.Format("~/Views/{0}/", controller.ViewFolder));
//                    }
//                }

//            }
//            return base.FileExists(controllerContext, virtualPath);
//        }
//        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
//        {
//            if (controllerContext.Controller is BaseController)
//            {
//                BaseController controller = controllerContext.Controller as BaseController;
//                if (!string.IsNullOrEmpty(controller.ViewFolder))
//                {
//                    if (!viewPath.StartsWith("~/Views/Shared/"))
//                    {
//                        viewPath = viewPath.Replace("~/Views/", string.Format("~/Views/{0}/", controller.ViewFolder));
//                    }
//                }
//            }
//            return base.CreateView(controllerContext, viewPath, masterPath);
//        }
//    }
//}
