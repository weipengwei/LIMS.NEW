using System;
using System.Web;
using System.Web.Routing;
using System.Configuration;

namespace LIMS.MVCFoundation.Helpers
{

    /// <summary>
    /// 替代系统默认的URLHelper
    /// </summary>
    public sealed class UrlHelper : System.Web.Mvc.UrlHelper
    {

        public UrlHelper(RequestContext requestContext)
            : base(requestContext)
        {

        }

        public UrlHelper(RequestContext requestContext, RouteCollection routeCollection)
            : base(requestContext, routeCollection)
        {

        }

        public string GetReturnUrl(HttpRequestBase request)
        {
            string hostUrl = GetBaseUrl(request);
            string returnUrl = request.HttpMethod.Equals("GET", StringComparison.InvariantCultureIgnoreCase) ?
              request.RequestContext.HttpContext.Server.UrlEncode(string.Format("{0}/{1}",
              hostUrl,
              request.Url.PathAndQuery.Remove(0, request.ApplicationPath.Length).TrimStart('/')))
              : string.Empty;

            return returnUrl;
        }

        public string GetBaseUrl()
        {
            return GetBaseUrl(RequestContext.HttpContext.Request);
        }

        public static string GetBaseUrl(HttpRequestBase request)
        {
            return EnvironmentHelper.GetBaseUrl();
        }

        private string _clientIP = null;
        public string ClientIP
        {
            get
            {
                if (_clientIP == null)
                {
                    _clientIP = this.RequestContext.HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(_clientIP))
                    {
                        _clientIP = this.RequestContext.HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                    }
                }
                return _clientIP;
            }
        }

        public override string Content(string contentPath)
        {
            var isRes = contentPath.IndexOf(".js") > 0 || contentPath.IndexOf(".css") > 0;
            if (isRes)
            {
                contentPath = contentPath.IndexOf("?") > 0
                    ? string.Format("{0}&v={1}", contentPath, ConfigurationManager.AppSettings["ResVersion"])
                    : string.Format("{0}?v={1}", contentPath, ConfigurationManager.AppSettings["ResVersion"]);
            }
            return base.Content(contentPath);
        }
    }
}

