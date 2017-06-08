using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using LIMS.Util;
using LIMS.MVCFoundation.Core;
using LIMS.Models;
using LIMS.Entities;
using LIMS.MVCFoundation.Controllers;
using LIMS.Services;
using LIMS.MVCFoundation.Attributes;

namespace LIMS.UI.Controllers.Main
{
    [RequiredLogon]
    [BaseEntityValue]
    public class FormsController : BaseController
    {

        public ActionResult OrderView(bool? needApprove, bool? adjustPrice, bool? editPrice)
        {
            ViewBag.NeedApprove = needApprove.HasValue ? needApprove.Value : false;
            ViewBag.AdjustPrice = adjustPrice.HasValue ? adjustPrice.Value : false;
            ViewBag.EditPrice = editPrice.HasValue ? editPrice.Value : false;

            return View();
        }
        

        public ActionResult DeliveryEdit()
        {
            return View();
        }

        public ActionResult DeliveryView()
        {
            return View();
        }


        
    }
}