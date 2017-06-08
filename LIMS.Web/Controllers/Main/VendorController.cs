using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using LIMS.Util;
using LIMS.MVCFoundation.Controllers;
using LIMS.MVCFoundation.Core;
using LIMS.MVCFoundation.Attributes;
using LIMS.Models;
using LIMS.Entities;
using LIMS.Services;

namespace LIMS.UI.Controllers.Main
{
    [RequiredLogon]
    [BaseEntityValue]
    public class VendorController : BaseController
    {
        #region Dispatch
        //public ActionResult Dispatch()
        //{
        //    var forms = new DispatchFormService().QueryConfirm(this.UserContext.RootUnitId)
        //        .Select(item => item.OrderFormNo).Distinct();
        //    ViewBag.OrderForms = forms;

        //    return View();
        //}

        //public JsonNetResult QueryDispatch(int? formNo, bool isValid = false)
        //{
        //    try
        //    {
        //        var vendor = new UnitService().Get(this.UserContext.RootUnitId);
        //        IList<GoodsStateEntity> entities;
        //        if (!formNo.HasValue || formNo.Value <= 0)
        //        {
        //            entities = new GoodsStateService().QueryInvalid(FormType.Dispatch, this.UserContext.RootUnitId, this.UserContext.CurrentHospital);
        //        }
        //        else
        //        {
        //            entities = new GoodsStateService().QueryInvalid(formNo.Value, FormType.Dispatch, this.UserContext.RootUnitId, this.UserContext.CurrentHospital);
        //        }

        //        var service = new ProductService();

        //        var list = entities.Select((item) => {
        //            var product = service.Get(item.ProductId);
        //            if (product == null)
        //            {
        //                throw new Exception(string.Format("The product({0}) does not exist", item.ProductId));
        //            }

        //            return new
        //            {
        //                Id = item.Id,
        //                ProductName = product.Name,
        //                VendorName = vendor.Name,
        //                Barcode = item.Barcode,
        //                IsValid = item.FutureValid
        //            };
        //        });
                
        //        return JsonNet(new ResponseResult(true, list));
        //    }
        //    catch(Exception e)
        //    {
        //        return JsonNet(new ResponseResult(false, e));
        //    }
        //}

        //public JsonNetResult CancelValidDispatch(string barcode)
        //{
        //    try
        //    {
        //        var service = new GoodsStateService();

        //        string errorCode;
        //        GoodsBarModel model;
        //        if(!service.Cancel(barcode, FormType.Dispatch, this.UserContext.CurrentHospital, this.UserContext.RootUnitId, this.UserContext.UserId, out model, out errorCode))
        //        {
        //            return JsonNet(new ResponseResult(false, GoodsStateValidateCodes.GetMessage(errorCode)));
        //        }

        //        return JsonNet(new ResponseResult(true, model));
        //    }
        //    catch (Exception e)
        //    {
        //        return JsonNet(new ResponseResult(false, e));
        //    }
        //}

        //public JsonNetResult ValidDispatch(string barcode)
        //{
        //    try
        //    {
        //        var service = new GoodsStateService();

        //        string errorCode;
        //        if(!service.Validate(barcode, FormType.Dispatch, this.UserContext.CurrentHospital, this.UserContext.RootUnitId, this.UserContext.UserId, out errorCode))
        //        {
        //            return JsonNet(new ResponseResult(false, GoodsStateValidateCodes.GetMessage(errorCode)));
        //        }

        //        return JsonNet(new ResponseResult());
        //    }
        //    catch(Exception e)
        //    {
        //        return JsonNet(new ResponseResult(false, e));
        //    }
        //}
        #endregion


        #region Dispatch Confirm
        //public ActionResult DispatchConfirm()
        //{
        //    var list = new DispatchFormService().QueryConfirm(this.UserContext.RootUnitId);

        //    var unitService = new UnitService();
        //    var productService = new ProductService();
        //    ViewBag.Context = new
        //    {
        //        List = list.Select(item =>
        //        {
        //            var hospital = unitService.Get(item.HospitalId).Name;
        //            var unit = unitService.Get(item.ApplyUnitId).Name;
        //            var product = productService.Get(item.ProductId).Name;

        //            return new
        //            {
        //                Id = item.Id,
        //                orderDetailId = item.OrderDetailId,
        //                FormNo = item.OrderFormNo,
        //                Hospital = hospital,
        //                Unit = unit,
        //                Product = product,
        //                DispatchCount = item.DispatchedCount,
        //                ScanCount = new GoodsStateService().CountValid(item.Id, FormType.Dispatch)
        //            };
        //        })
        //    };

        //    return View();
        //}

        //public JsonNetResult ConfirmDispatch(string id, string batchNo, DateTime? goodsExpiredDate, string logisticsBarcode, string logisticsInfo)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(id))
        //        {
        //            throw new Exception("The dispatch form id is empty.");
        //        }
        //        if (string.IsNullOrEmpty(batchNo))
        //        {
        //            throw new Exception("The batch no. is empty.");
        //        }
        //        if (!goodsExpiredDate.HasValue)
        //        {
        //            throw new Exception("The expired date of goods is empty.");
        //        }
        //        if (string.IsNullOrEmpty(logisticsBarcode))
        //        {
        //            throw new Exception("The logistics barcode is empty.");
        //        }
        //        if (string.IsNullOrEmpty(logisticsInfo))
        //        {
        //            throw new Exception("The logistics info is empty.");
        //        }

        //        new DispatchFormService().Confirm(id, batchNo, goodsExpiredDate.Value, logisticsBarcode, logisticsInfo, this.UserContext.UserId);
        //        return JsonNet(new ResponseResult());
        //    }
        //    catch(Exception e)
        //    {
        //        return JsonNet(new ResponseResult(false, e));
        //    }
        //}
        #endregion
    }
}