using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using LIMS.MVCFoundation.Core;
using LIMS.MVCFoundation.Controllers;
using LIMS.Services;
using LIMS.Entities;
using LIMS.Models;
using LIMS.Util;
using LIMS.MVCFoundation.Attributes;

namespace LIMS.Web.Controllers.Setting
{
    [RequiredLogon]
    [BaseEntityValue]
    public class ProductController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonNetResult Query(string condition, PagerInfo pager)
        {
            var list = new ProductService().Query(condition, pager);
            return JsonNet(new ResponseResult(true, list, pager));
        }

        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View();
            }
            else
            {
                var mode = new ProductService().Get(id);

                return View(mode);
            }
        }

        public JsonNetResult Save(ProductEntity product)
        {
            if (!this.Validate(product))
            {
                return JsonNet(new ResponseResult(false, "The required attributes of product are not filled.", ErrorCodes.RequireField));
            }

            new ProductService().Save(product);

            return JsonNet(new ResponseResult());
        }

        private bool Validate(ProductEntity product)
        {
            if (string.IsNullOrEmpty(product.Name))
            {
                return false;
            }

            if (string.IsNullOrEmpty(product.MiniPackageUnit))
            {
                return false;
            }

            if (product.MiniPackageCount <= 0)
            {
                return false;
            }

            return true;
        }
    }
}
