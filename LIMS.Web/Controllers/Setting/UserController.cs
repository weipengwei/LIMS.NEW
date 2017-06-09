using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using LIMS.MVCFoundation.Core;
using LIMS.MVCFoundation.Controllers;
using LIMS.MVCFoundation.Helpers;
using LIMS.Services;
using LIMS.Entities;
using LIMS.Models;
using LIMS.Util;
using LIMS.MVCFoundation.Attributes;

namespace LIMS.Web.Controllers.Setting
{
    [RequiredLogon]
    [BaseEntityValue]
    public class UserController : BaseController
    {
        private  readonly UserService _userService = new UserService();
        private readonly SystemPrivilegeService _systemPrivilegeService = new SystemPrivilegeService();
        private readonly UnitService _unitService = new UnitService();

        public ActionResult Index()
        {
            ViewBag.Roots = GetRoots();

            return View();
        }

        public JsonNetResult Query(string condition, string rootId, string unitId, PagerInfo pager)
        {
            var list = new UserService().Query(condition, rootId, unitId, pager);
            return JsonNet(new ResponseResult(true, list, pager));
        }

        public ActionResult Edit(string id)
        {
            ViewBag.ShowRoots = true;
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.Roots = GetRoots();
                return View();
            }
            else
            {
                var mode = new UserService().Get(id);

                ViewBag.ShowRoots = false;
                ViewBag.Units = new UnitService().GetAllById(mode.UnitId);

                return View(mode);
            }
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public JsonNetResult Save(UserModel user)
        {
            if (!this.Validate(user))
            {
                return JsonNet(new ResponseResult(false, "The required attributes of user are not filled.", ErrorCodes.RequireField));
            }

            var service = new UserService();
            var validationUser = service.GetByAccount(user.Account, user.Id);
            if(validationUser != null)
            {
                return JsonNet(new ResponseResult(true, "账号重复了！"));
            }

            service.Save(new UserEntity
            {
                Id = user.Id,
                Name = user.Name,
                Account = user.Account,
                Password = string.IsNullOrEmpty(user.Password) ? string.Empty : SecurityHelper.HashPassword(user.Password),
                Title = user.Title,
                UnitId = user.UnitId,
                IsChangePassword = true
            });

            return JsonNet(new ResponseResult());
        }

        #region 私有方法

        private bool Validate(UserModel user)
        {
            if (string.IsNullOrEmpty(user.Name))
            {
                return false;
            }

            if (string.IsNullOrEmpty(user.Account))
            {
                return false;
            }

            if (string.IsNullOrEmpty(user.Id) && (string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.ValidPassword)))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(user.Password) && string.IsNullOrEmpty(user.ValidPassword))
            {
                if (string.Compare(user.Password, user.ValidPassword) != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private IDictionary<UnitType, List<object>> GetRoots()
        {
            var roots = new UnitService().GetByRootId(Constant.DEFAULT_UNIT_ROOT_ID);
            var hospitals = roots.Where(item => item.Type == UnitType.Hospital).Select(item =>
                new
                {
                    Id = item.Id,
                    Name = item.Name
                }).ToList<object>();
            var vendors = roots.Where(item => item.Type == UnitType.Vendor).Select(item =>
                new
                {
                    Id = item.Id,
                    Name = item.Name
                }).ToList<object>();

            var dic = new Dictionary<UnitType, List<object>>();
            dic[UnitType.Hospital] = hospitals;
            dic[UnitType.Vendor] = vendors;

            return dic;
        }

        #endregion



        /// <summary>
        /// 根据用户ID获取对应单位信息与权限
        /// </summary>
        /// <param name="account">用户ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonNetResult GetUserRoot(string account)
        {
            UserEntity user = null;
            UnitModel unit = null;
            IList<SystemPrivilegeEntity> privilege = null;
            if (UserService.TryGetUserByAccount(account, out user))
            {
                unit= _unitService.Get(user.UnitId);
            }
            if (unit != null)
            {
                privilege= _systemPrivilegeService.GetByObjectId(unit.Id,0);
            }
            return JsonNet(new ResponseResult(true,new
            {
                user_id= user==null?"": user.Id,
                unit_id = unit == null ? "" : unit.Id,
                unit_Type= unit?.Type.GetHashCode() ?? 0,
                unit_ParentId = unit == null ? "" : unit.ParentId,
                SystemPrivilege= privilege?.Select(m=> new {m.Id,m.FunKey})
            }));
        }
    }
}
