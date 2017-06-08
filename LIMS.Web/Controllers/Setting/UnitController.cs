using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIMS.MVCFoundation.Core;
using LIMS.MVCFoundation.Controllers;
using LIMS.Services;
using LIMS.Entities;
using LIMS.Models;
using LIMS.MVCFoundation.Attributes;

namespace LIMS.Web.Controllers.Setting
{
    [RequiredLogon]
    [BaseEntityValue]
    public class UnitController : BaseController
    {
        public JsonNetResult GetUnits(string parentId)
        {
            var list = new UnitService().GetByRootId(parentId);

            return JsonNet(new ResponseResult(true, list));
        }
    }
}
