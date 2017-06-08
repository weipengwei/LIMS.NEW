using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using LIMS.Util;
using LIMS.MVCFoundation.Attributes;
using LIMS.MVCFoundation.Core;
using LIMS.Models;
using LIMS.Entities;
using LIMS.MVCFoundation.Controllers;
using LIMS.Services;

namespace LIMS.UI.Controllers.Main
{
    [RequiredLogon]
    [BaseEntityValue]
    public class HelperController : BaseController
    {
        public JsonNetResult HospitalUnitVendors(string unitId)
        {
            var vendors = new UnitService().GetVendorsByHospitalUnit(unitId).Select(item => new
            {
                Id = item.Id,
                Name = item.Name
            });
            var applyProducts = new HospitalProductService().GetByUnit(unitId);

            return JsonNet(new ResponseResult(true, new { Vendors = vendors, ApplyProducts = applyProducts }));
        }

        public JsonNetResult VendorProducts(string vendorId, DateTime applyTime)
        {
            var auditingList = new AuditingProductService().QueryValidVersion(this.UserContext.CurrentHospital, vendorId, applyTime);
            var products = new ProductService().GetByVendor(vendorId);

            AuditingProductEntity auditing;
            var list = products.Select(item =>
            {
                return new
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    ShortCode = item.ShortCode,
                    MeasuringUnit = item.MiniPackageUnit,
                    MiniPackageCount = item.MiniPackageCount,
                    MiniPackageCapacity = item.MiniPackageSpec,
                    Barcode = item.Barcode,
                    IsAuditing = auditingList.TryGetValue(item.Id, out auditing) ? auditing.IsAudit : false
                };
            });

            return JsonNet(new ResponseResult(true, list));
        }

        public JsonNetResult GetApprovers(string formId, int formType)
        {
            try
            {
                if (string.IsNullOrEmpty(formId))
                {
                    return JsonNet(new ResponseResult());
                }

                var approverList = new FormApproversService().GetDetailApprovers(formId, (int)FormType.OrderAudit);
                var userService = new UserService();

                var list = new List<dynamic>();
                foreach (var item in approverList)
                {
                    var user = userService.Get(item.ApproverId);
                    if (user == null)
                    {
                        continue;
                    }

                    list.Add(new
                    {
                        Id = item.Id,
                        Name = user.Name,
                        Status = item.Status,
                        StatusName = FormAuditStatus.GetStatusName(item.Status),
                        Sequence = item.Sequence,
                        Remark = item.Remark,
                        ApprovedTime = item.ApprovedTime
                    });
                }

                return JsonNet(new ResponseResult(true, list));
            }
            catch (Exception e)
            {
                return JsonNet(new ResponseResult(false, e));
            }
        }

        public JsonNetResult QueryHospitalPersons(string term, string hospitalId)
        {
            if (string.IsNullOrEmpty(term))
            {
                return JsonNet(new ResponseResult());
            }

            try
            {
                var units = new UserService().Query(term, hospitalId);

                var list = new List<dynamic>();
                foreach (var item in units)
                {
                    list.Add(new
                    {
                        Id = item.Id,
                        Value = item.Name,
                    });
                }

                return JsonNet(list);
            }
            catch (Exception e)
            {
                return JsonNet();
            }
        }

        public JsonNetResult QueryHospitalUnits(string term, string hospitalId)
        {
            if (string.IsNullOrEmpty(term))
            {
                return JsonNet(new ResponseResult());
            }

            try
            {
                var units = new UnitService().Query(term, hospitalId);

                var list = new List<dynamic>();
                foreach (var item in units)
                {
                    list.Add(new
                    {
                        Id = item.Id,
                        Value = item.Name,
                    });
                }

                return JsonNet(list);
            }
            catch (Exception e)
            {
                return JsonNet();
            }
        }
    }
}