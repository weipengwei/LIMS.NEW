using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using LIMS.Util;
using LIMS.Entities;
using LIMS.Models;
using LIMS.MVCFoundation.Core;
using LIMS.MVCFoundation.Attributes;
using LIMS.Services;

using LIMS.MVCFoundation.Controllers;

namespace LIMS.UI.Controllers.Main
{
    [RequiredLogon]
    [BaseEntityValue]
    public class HospitalController : BaseController
    {
        #region Order
        //public ActionResult Order()
        //{
        //    return View();
        //}

        //public JsonNetResult QueryOrder(DateRangeCondition condition, PagerInfo pager)
        //{
        //    condition = condition == null ? new DateRangeCondition() : condition;
        //    this.InitCondition(condition);

        //    try
        //    {
        //        var unitService = new UnitService();
        //        var result = new List<object>();

        //        var list = new OrderService().Query(condition, pager);
        //        foreach (var item in list)
        //        {
        //            UnitModel unit = unitService.Get(item.ApplyUnitId);
        //            var vendor = unitService.Get(item.VendorId);

        //            result.Add(new
        //            {
        //                Id = item.Id,
        //                FormNo = item.FormNo,
        //                ApplyTime = item.ApplyTime,
        //                UnitName = unit != null ? unit.Name : string.Empty,
        //                VendorName = vendor != null ? vendor.Name : string.Empty
        //            });
        //        }

        //        return JsonNet(new ResponseResult(true, result, pager));
        //    }
        //    catch (Exception e)
        //    {
        //        return JsonNet(new ResponseResult(e));
        //    }
        //}

        //public ActionResult OrderEdit(string id)
        //{
        //    var unitService = new UnitService();
        //    var applyUnits = unitService.GetApplyUnits(this.UserContext.UserId, this.UserContext.CurrentHospital).Select(item => new
        //    {
        //        Id = item.Id,
        //        Name = item.Name,
        //        defaultReceiptId = item.DefaultReceiptId,
        //        isDefault = string.Compare(this.UserContext.UnitId, item.Id, true) == 0
        //    });

        //    if (string.IsNullOrEmpty(id))
        //    {
        //        ViewBag.Context = new
        //        {
        //            FormNo = IdentityCreatorService.New(IdentityKey.ORDER_FORM),
        //            ApplyTime = DateTime.Now,
        //            ApplyUnits = applyUnits,
        //            Receipts = new ReceiptInfoService().GetByHospital(this.UserContext.CurrentHospital).Select(item => new
        //            {
        //                Id = item.Id,
        //                Title = item.Title
        //            })
        //        };
        //        ViewBag.View = false;
        //    }
        //    else
        //    {
        //        var order = new OrderService().Get(id);
        //        ViewBag.Context = new
        //        {
        //            ApplyUnits = applyUnits,
        //            ApplyTime = order.ApplyTime,
        //            VendorsData = new
        //            {
        //                Vendors = unitService.GetVendorsByHospitalUnit(order.ApplyUnitId).Select(item => new
        //                {
        //                    Id = item.Id,
        //                    Name = item.Name
        //                }),
        //                ApplyProducts = new HospitalProductService().GetByUnit(order.ApplyUnitId)
        //            },
        //            Products = new ProductService().GetByVendor(order.VendorId).Select(item => new
        //            {
        //                Id = item.Id,
        //                Name = item.Name
        //            }),
        //            Receipts = new ReceiptInfoService().GetByHospital(this.UserContext.CurrentHospital).Select(item => new
        //            {
        //                Id = item.Id,
        //                Title = item.Title
        //            }),
        //            Order = order
        //        };

        //        ViewBag.View = order.Status == OrderStatus.View;
        //    }

        //    return View();
        //}

        //public JsonNetResult SaveOrderHeader(OrderFormEntity order)
        //{
        //    try
        //    {
        //        order.HospitalId = this.UserContext.CurrentHospital;
        //        order.FillerId = this.UserContext.UserId;

        //        new OrderService().Save(order, null);
        //        return JsonNet(new ResponseResult(true, new { Id = order.Id }));
        //    }
        //    catch (Exception e)
        //    {
        //        return JsonNet(new ResponseResult(e));
        //    }
        //}

        //public JsonNetResult SaveOrderDetail(OrderFormItemEntity orderDetail)
        //{
        //    try
        //    {
        //        orderDetail.HospitalId = this.UserContext.CurrentHospital;
        //        new OrderService().SaveDetail(orderDetail);
        //        return JsonNet(new ResponseResult(true, new { Id = orderDetail.Id }));
        //    }
        //    catch (Exception e)
        //    {
        //        return JsonNet(new ResponseResult(e));
        //    }
        //}

        //public JsonNetResult DeleteOrderDetail(string id)
        //{
        //    try
        //    {
        //        new OrderService().DeleteDetail(id);
        //        return JsonNet(new ResponseResult());
        //    }
        //    catch (Exception e)
        //    {
        //        return JsonNet(new ResponseResult(false, e));
        //    }
        //}

        //public JsonNetResult DeleteOrder(string id)
        //{
        //    try
        //    {
        //        new OrderService().AutoDelete(id, this.UserContext.UserId);
        //        return JsonNet(new ResponseResult());
        //    }
        //    catch(Exception e)
        //    {
        //        return JsonNet(new ResponseResult(false, e));
        //    }
        //}
        #endregion

        public ActionResult Accept()
        {
            return View();
        }

        public ActionResult ProductRegiste()
        {
            return View();
        }


        #region Apply
        //public ActionResult Apply()
        //{
        //    return View();
        //}

        //public ActionResult ApplyEdit(string id, bool? isView)
        //{
        //    var unitService = new UnitService();
        //    var applyUnits = unitService.GetApplyUnits(this.UserContext.UserId, this.UserContext.CurrentHospital).Select(item => new
        //    {
        //        Id = item.Id,
        //        Name = item.Name,
        //        isDefault = string.Compare(this.UserContext.UnitId, item.Id, true) == 0
        //    });

        //    var products = new ProductService().GetByHospital(this.UserContext.CurrentHospital).Select(item => new { Id = item.Id, Name = item.Name });

        //    if (string.IsNullOrEmpty(id))
        //    {
        //        ViewBag.View = isView.HasValue ? isView.Value : false;
        //        ViewBag.Context = new
        //        {
        //            FormNo = IdentityCreatorService.New(IdentityKey.APPLY_FORM),
        //            ApplyDate = DateTime.Now,
        //            ApplyUnits = applyUnits,
        //            Products = products
        //        };
        //    }
        //    else
        //    {
        //        var entity = new ApplyFormService().Get(id);
        //        if(entity == null)
        //        {
        //            throw new Exception("The apply form does not exist.");
        //        }

        //        ViewBag.View = isView.HasValue 
        //            ? isView.Value 
        //            : (entity.Status == ApplyFormStatus.Granting || entity.Status == ApplyFormStatus.Granted ? true : false);
        //        ViewBag.Context = new
        //        {
        //            Form = entity,
        //            ApplyUnits = applyUnits,
        //            Products = products
        //        };
        //    }

        //    return View();
        //}

        //public JsonNetResult SaveApply(ApplyFormEntity entity)
        //{
        //    try
        //    {
        //        if(entity == null)
        //        {
        //            throw new Exception("The apply form is null.");
        //        }

        //        if(entity.Details == null || entity.Details.Count == 0)
        //        {
        //            return JsonNet(new ResponseResult());
        //        }

        //        entity.FilterId = this.UserContext.UserId;
        //        entity.ApplyDate = DateTime.Now;
        //        entity.HospitalId = this.UserContext.CurrentHospital;
        //        entity.Status = ApplyFormStatus.Applied;

        //        new ApplyFormService().Save(entity);

        //        return JsonNet(new ResponseResult());
        //    }
        //    catch(Exception e)
        //    {
        //        return JsonNet(new ResponseResult(false, e));
        //    }
        //}

        //public JsonNetResult QueryApply(ApplyQueryCondition condition, PagerInfo pager)
        //{
        //    try
        //    {
        //        condition.UserId = this.UserContext.UserId;
        //        condition.HospitalId = this.UserContext.CurrentHospital;

        //        var list = new ApplyFormService().Query(condition, pager);

        //        return JsonNet(new ResponseResult(true, list, pager));
        //    }
        //    catch(Exception e)
        //    {
        //        return JsonNet(new ResponseResult(e));
        //    }
        //}

        //public JsonNetResult CancelApply(string id)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(id))
        //        {
        //            throw new Exception("The parameter is empty.");
        //        }

        //        if (new ApplyFormService().Cancel(id))
        //        {
        //            return JsonNet(new ResponseResult());
        //        }
        //        else
        //        {
        //            return JsonNet(new ResponseResult(false, "请查看领用单是否已取消或者已开始发放！"));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return JsonNet(new ResponseResult(e));
        //    }
        //}
        #endregion


        //#region Audit
        //public ActionResult Audit()
        //{
        //    return View();
        //}

        //public JsonNetResult QueryAudit(AuditQueryCondition condition, PagerInfo pager)
        //{
        //    try
        //    {
        //        condition.HospitalId = this.UserContext.CurrentHospital;
        //        condition.UserId = this.UserContext.UserId;

        //        var list = new FormApproversService().Query(condition, pager);

        //        var result = new List<dynamic>();
        //        foreach(var item in list)
        //        {
        //            result.Add(new
        //            {
        //                Id = item.Id,
        //                Name = FormHelper.FormName((FormType)item.FormType),
        //                FormNo = item.FormNo,
        //                FormId = item.FormId,
        //                Status = item.Status,
        //                StatusName = FormAuditStatus.GetStatusName(item.Status)
        //            });
        //        }

        //        return JsonNet(new ResponseResult(true, result, pager));
        //    }
        //    catch(Exception e)
        //    {
        //        return JsonNet(new ResponseResult(e));
        //    }
        //} 

        //public ActionResult AuditOrderDetail(string id)
        //{
        //    if(string.IsNullOrEmpty(id))
        //    {
        //        return View();
        //    }

        //    var approver = new FormApproversService().Get(id);
            
        //    var service = new OrderFormService();
        //    var detail = service.GetDetail(approver.FormId);
        //    if(detail == null)
        //    {
        //        return View();
        //    }
        //    var order = service.Get(detail.OrderId);
        //    if(order == null)
        //    {
        //        return View();
        //    }

        //    var unitService = new UnitService();
        //    var productService = new ProductService();
        //    var context = new
        //    {
        //        id = id,
        //        FormNo = order.FormNo,
        //        FormType = (int)FormType.OrderAudit,
        //        ApplyTime = order.ApplyTime,
        //        ApplyUnit = unitService.Get(order.ApplyUnitId).Name,
        //        Vendor = unitService.Get(order.VendorId).Name,
        //        ProductName = productService.Get(detail.ProductId).Name,
        //        Count = detail.Count,
        //        Price = detail.Price,
        //        ExpectedPrice = detail.ExpectedPrice,
        //        ExpiredDate = detail.ExpiredDate,
        //        ExpectedDate = detail.ExpectedDate
        //    };

        //    ViewBag.Context = context;

        //    return View();
        //}

        //public JsonNetResult Approve(string id, bool yesOrNo, string remark)
        //{
        //    try
        //    {
        //        new FormApproversService().Approve(id, yesOrNo, remark);
                
        //        return JsonNet(new ResponseResult());
        //    }
        //    catch (Exception e)
        //    {
        //        return JsonNet(new ResponseResult(e));
        //    }
        //}
        //#endregion

        #region Barcode
        public ActionResult BarcodeQuery()
        {
            return View();
        }
        public JsonNetResult QueryBarcode(BarcodeQueryCondition condition)
        {
            try
            {
                var list = new GoodsExtraService().QueryBarcode(condition.BarcodeNo);

                return JsonNet(new ResponseResult(true, list));
            }
            catch (Exception e)
            {
                return JsonNet(new ResponseResult(e));
            }
        }

        #endregion
    }
}