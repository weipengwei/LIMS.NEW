using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using LIMS.Util;
using LIMS.Models;
using LIMS.MVCFoundation.Core;
using LIMS.Repositories;

namespace LIMS.MVCFoundation.Controllers
{
    public class BaseController : Controller
    {
        private bool m_PrincipalLoaded = false;
        private CustomPrincipal m_Principal;
        public CustomPrincipal Principal
        {
            get
            {
                if (m_PrincipalLoaded)
                {
                    return m_Principal;
                }

                m_PrincipalLoaded = true;
                m_Principal = this.User as CustomPrincipal;

                return m_Principal;
            }
        }

        private bool m_UserContextLoaded = false;
        private UserContext m_UserContext;
        public UserContext UserContext
        {
            get
            {
                if (m_UserContextLoaded)
                {
                    return m_UserContext;
                }

                this.m_UserContextLoaded = true;
                if (this.Principal != null)
                {
                    m_UserContext = this.Principal.Context;
                }

                return m_UserContext;
            }
        }

        public bool IsAdmin
        {
            get
            {
                return string.Compare(this.UserContext.UserId, Constant.ADMIN_ID, true) == 0;
            }
        }

        ///// <summary>
        ///// 视图的物理路径
        ///// </summary>
        //public virtual string ViewFolder
        //{
        //    get
        //    {
        //        object viewfolder = null;
        //        if (RouteData.DataTokens.TryGetValue("viewfolder", out viewfolder))
        //        {
        //            return viewfolder.ToString();
        //        }
        //        else
        //        {
        //            return string.Empty;
        //        }
        //    }
        //}

        public JsonNetResult JsonNet()
        {
            return JsonNet(null);
        }

        public JsonNetResult JsonNet(object data)
        {
            var result = new JsonNetResult();

            if (data != null)
            {
                result.Data = data;
            }

            return result;
        }

        protected void InitCondition(DateRangeCondition condition)
        {
            condition.UserId = this.UserContext.UserId;
            condition.HospitalId = this.UserContext.CurrentHospital;

            if (condition.EndDate.HasValue)
            {
                condition.EndDate = condition.EndDate.Value.AddDays(1);
            }
        }

        protected void ClearContext()
        {
            var cookie = this.Response.Cookies[Constant.CURRENT_HOSPITAL_COOKIE];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddMinutes(-60);
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RepositoryBase.ClearTrans();

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            bool unHandledError = false;
            bool commit = true;
            if (filterContext.Exception != null && !filterContext.ExceptionHandled)
            {
                commit = false;
                unHandledError = true;
            }
            else if (filterContext.Result is JsonNetResult
                && ((JsonNetResult)filterContext.Result).Data is ResponseResult
                && !((ResponseResult)((JsonNetResult)filterContext.Result).Data).IsSuccess)
                commit = false;

            RepositoryBase.ReleaseTrans(commit, true);

            if (unHandledError
                && (filterContext.Result == null || filterContext.Result is EmptyResult))
            {
                filterContext.Result = JsonNet(new ResponseResult(filterContext.Exception));
                filterContext.ExceptionHandled = true;
            }
        }
    }
}
