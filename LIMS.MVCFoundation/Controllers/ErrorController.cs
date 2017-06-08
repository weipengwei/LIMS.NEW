//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Web.Mvc;
//using LIMS.MVCFoundation.CustomExceptions;

//namespace LIMS.MVCFoundation.Controllers
//{
//    public class ErrorController : Controller
//    {
//        public ActionResult Unauthorized(UnauthorizedException exception)
//        {
//            var info = "未授权";
//            if (Request.IsAjaxRequest())
//            {
//                Response.TrySkipIisCustomErrors = true;
//                Response.StatusCode = 403;

//                return ErrorInfo(info);
//            }
//            else
//            {
//                return View("Unauthorized", info);
//            }
//        }

//        public ActionResult Info(Exception infoParam)
//        {
//            var info = infoParam.Message;
            
//            if (Request.IsAjaxRequest())
//            {
//                try
//                {
//                    Response.TrySkipIisCustomErrors = true;
//                    Response.StatusCode = 500;
//                }
//                catch { }

//                return ErrorInfo(info);
//            }
//            else
//            {
//                return View("Info", infoParam);
//            }
//        }
        
//        private ActionResult ErrorInfo(object data)
//        {
//            return new ContentResult() { Content = data.ToString(), ContentEncoding = Encoding.UTF8, ContentType = "text/html" };
//        }
//    }
//}
