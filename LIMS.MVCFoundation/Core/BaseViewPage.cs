using System;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace LIMS.MVCFoundation.Core
{ 
    public abstract class BaseViewPage : System.Web.Mvc.WebViewPage
    {
        /// <summary>
        ///当前的登录用户 没有登录为null
        /// </summary>
        public new CustomPrincipal User
        {
            get
            {
                return base.User as CustomPrincipal;
            }
        }

        /// <summary>
        /// 使用JsonNet序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected internal MvcHtmlString JsonNetString(object data)
        {

            return new MvcHtmlString(JsonConvert.SerializeObject(data, Formatting.None, JsonNetResult.DefaultSerializerSettings));
        }

        /// <summary>
        /// 使用JsonNet序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected internal string JsonNet(object data)
        {
            return JsonConvert.SerializeObject(data, Formatting.None, JsonNetResult.DefaultSerializerSettings);
        }

        public override void InitHelpers()
        {
            base.InitHelpers();

            this.Url = new Helpers.UrlHelper(ViewContext.RequestContext);
        }
    }
}

