using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.Threading.Tasks;
using LIMS.MVCFoundation.Core;
using LIMS.MVCFoundation.Controllers;
using LIMS.Services;
using LIMS.Models;
using LIMS.Entities;
using LIMS.Util;
using LIMS.MVCFoundation.Attributes;

namespace LIMS.Web.Controllers.Setting
{
    [RequiredLogon]
    [BaseEntityValue]
    public class SystemFunctionController : BaseController
    {
        public ActionResult Index(string unitId)
        {
            if (string.IsNullOrEmpty(unitId))
            {
                return View();
            }

            ViewBag.UnitId = unitId;
            ViewBag.Functions = new SystemFunctionService().GetAll();
            ViewBag.Privileges = new SystemPrivilegeService().GetByObjectId(unitId);

            return View();
        }

        public JsonNetResult Save(string unitId, IList<PrivilegeItem> privileges)
        {
            if (string.IsNullOrEmpty(unitId))
            {
                return JsonNet(new ResponseResult(false, "The unit id is empty."));
            }

            var entities = new List<SystemPrivilegeEntity>();
            if (privileges != null)
            {
                entities = privileges.Select(item => new SystemPrivilegeEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    ObjectId = unitId,
                    ObjectType = 0,
                    FunKey = item.Id,
                    Query = item.Query,
                    Operate = item.Operate
                }).ToList();
            }

            new SystemPrivilegeService().Save(unitId, entities);

            return JsonNet(new ResponseResult());
        }


        public ActionResult UserPrivilege(string userId)
        {
            ViewBag.UserId = userId;
            ViewBag.Roots = GetRoots(userId);

            var user = new UserService().Get(userId);
            var unit = new UnitService().Get(user.UnitId);

            ViewBag.CurrentRootId = unit.RootId;

            return View();
        }

        private IDictionary<UnitType, List<object>> GetRoots(string userId)
        {
            var user = new UserService().Get(userId);
            var unitService = new UnitService();

            var unit = unitService.Get(user.UnitId);

            if(unit.Type == UnitType.None)
            {
                return null;
            }

            var dic = new Dictionary<UnitType, List<object>>();
            if (unit.Type == UnitType.Hospital || unit.Type == UnitType.HospitalUnit)
            {
                var roots = unitService.GetByRootId(Constant.DEFAULT_UNIT_ROOT_ID);

                dic[UnitType.Hospital] = roots.Where(item => item.Type == UnitType.Hospital).Select(item =>
                new
                {
                    Id = item.Id,
                    Name = item.Name
                }).ToList<object>();
            }
            else
            {
                var vendor = new UnitService().Get(unit.RootId == Constant.DEFAULT_UNIT_ROOT_ID ? unit.Id : unit.RootId);
                dic[UnitType.Vendor] = new List<object>()
                {
                    new
                    {
                        Id = vendor.Id,
                        Name = vendor.Name
                    }
                };
            }

            return dic;
        }

        public JsonNetResult SaveUserPrivilege(string userId, string rootId, IList<PrivilegeItem> privileges)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return JsonNet(new ResponseResult(false, "The user id is empty."));
            }

            if (string.IsNullOrEmpty(rootId))
            {
                return JsonNet(new ResponseResult(false, "The root id is empty."));
            }

            var entities = new List<UserPrivilegeEntity>();
            if (privileges != null)
            {
                entities = privileges.Select(item => new UserPrivilegeEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    UnitId = item.Id,
                    UnitRootId = rootId,
                    Query = item.Query,
                    Operate = item.Operate
                }).ToList();
            }

            new UserPrivilegeService().Save(userId, rootId, entities);

            return JsonNet(new ResponseResult());
        }

        public JsonNetResult GetUserPrivileges(string userId, string parentId)
        {
            var units = new UnitService().GetByRootId(parentId);
            var privileges = new UserPrivilegeService().Query(userId, parentId);
            
            return JsonNet(new ResponseResult(true, new { units = units, privileges = privileges }));
        }
    }
}
