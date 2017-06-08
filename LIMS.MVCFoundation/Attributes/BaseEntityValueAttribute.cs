using System;
using System.Web;
using System.Web.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using LIMS.MVCFoundation.Core;
using LIMS.MVCFoundation.Helpers;
using LIMS.MVCFoundation.CustomExceptions;
using LIMS.Entities;

namespace LIMS.MVCFoundation.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class BaseEntityValueAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = this.GetUser(filterContext.HttpContext);
            if(user == null)
            {
                return;
            }

            var parameters = filterContext.ActionParameters.Values;
            if(parameters.Count > 0)
            {
                foreach(var parameter in parameters)
                {
                    var baseEntity = parameter as BaseEntity;
                    if(baseEntity != null)
                    {
                        baseEntity.CreatedId = user.UserId;
                        baseEntity.UpdatedId = user.UserId;
                    }
                }
            }
        }

        private CustomPrincipal GetUser(HttpContextBase httpContext)
        {
            return httpContext.User as CustomPrincipal;
        }
    }
}
