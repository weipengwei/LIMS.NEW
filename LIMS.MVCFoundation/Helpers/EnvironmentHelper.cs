using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.MVCFoundation.Helpers
{
    public static class EnvironmentHelper
    {
        public static string GetBaseUrl()
        {
            var request = System.Web.HttpContext.Current.Request;

            string strBaseUrl = "";
            //if (request.Url.Port.ToString() == "443")
            //{
            //    strBaseUrl += "https://" + request.Url.Host;
            //}
            //else if (request.Url.Port.ToString() != "80")
            //{
            //    strBaseUrl += "http://" + request.Url.Host + ":" + request.Url.Port;
            //}
            //else
            //{
            //    strBaseUrl += "http://" + request.Url.Host;
            //}
            //原因是：http://hi.baidu.com/liulanghe/item/449af78310dad02a100ef397
            //端口映射下 request.Url.Port得不到正确地址
            if (request.IsSecureConnection)
            {
                strBaseUrl += "https://";
            }
            else
            {
                strBaseUrl += "http://";
            }
            strBaseUrl += request.ServerVariables["HTTP_HOST"];
            strBaseUrl += request.ApplicationPath;
            return strBaseUrl.TrimEnd('/');
        }
    }
}
