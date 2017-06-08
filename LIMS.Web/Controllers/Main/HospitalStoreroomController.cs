using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Threading.Tasks;

using LIMS.Util;
using LIMS.Entities;
using LIMS.Models;
using LIMS.MVCFoundation.Core;
using LIMS.Services;
using LIMS.MVCFoundation.Attributes;

using LIMS.MVCFoundation.Controllers;

namespace LIMS.Web.Controllers.Main
{
    [RequiredLogon]
    [BaseEntityValue]
    public class HospitalStoreroomController : BaseController
    {
        #region Receive
        //public ActionResult Receive()
        //{
        //    return View();
        //}

        //public JsonNetResult ValidReceive(string barcode)
        //{
        //    try
        //    {
        //        var service = new GoodsStateService();

        //        string errorCode;
        //        GoodsBarModel model;

        //        if (!service.Validate(barcode, FormType.Receive, this.UserContext.CurrentHospital, this.UserContext.UserId, out model, out errorCode))
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

        //public ActionResult ReceiveConfirm()
        //{
        //    var list = new ReceiveFormService().QueryConfirm(this.UserContext.RootUnitId);

        //    var unitService = new UnitService();
        //    var productService = new ProductService();
        //    ViewBag.Context = new
        //    {
        //        List = list.Select(item =>
        //        {
        //            var unit = unitService.Get(item.ApplyUnitId).Name;
        //            var product = productService.Get(item.ProductId).Name;

        //            return new
        //            {
        //                Id = item.Id,
        //                orderDetailId = item.OrderDetailId,
        //                FormNo = item.OrderFormNo,
        //                Unit = unit,
        //                Product = product,
        //                DispatchCount = item.ReceivedCount,
        //                ScanCount = new GoodsStateService().CountValid(item.Id, FormType.Receive)
        //            };
        //        })
        //    };

        //    return View();
        //}

        //public JsonNetResult CancelValidReceive(string barcode)
        //{
        //    try
        //    {
        //        var service = new GoodsStateService();

        //        string errorCode;
        //        GoodsBarModel model;

        //        if (!service.Cancel(barcode, FormType.Receive, this.UserContext.CurrentHospital, this.UserContext.UserId, out model, out errorCode))
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

        //public JsonNetResult BatchCancelValidReceive(string id)
        //{
        //    try
        //    {
        //        var service = new GoodsStateService();

        //        string errorcode;
        //        if (!service.CanFormValidate(id, FormType.Receive, this.UserContext.CurrentHospital, out errorcode))
        //        {
        //            return JsonNet(new ResponseResult(false, GoodsStateValidateCodes.GetMessage(errorcode)));
        //        }

        //        service.Cancel(id, FormType.Receive, this.UserContext.UserId);

        //        return JsonNet(new ResponseResult());
        //    }
        //    catch (Exception e)
        //    {
        //        return JsonNet(new ResponseResult(e));
        //    }
        //}

        //public JsonNetResult ConfirmReceive(string id)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(id))
        //        {
        //            throw new Exception("The receive form id is empty.");
        //        }

        //        new ReceiveFormService().Confirm(id, this.UserContext.UserId);
        //        return JsonNet(new ResponseResult());
        //    }
        //    catch (Exception e)
        //    {
        //        return JsonNet(new ResponseResult(false, e));
        //    }
        //}
        #endregion


        #region Inspection
        public ActionResult Inspection()
        {
            return View();
        }

        public JsonNetResult ValidInspection(string barcode)
        {
            try
            {
                var service = new GoodsStateService();

                string errorCode;
                GoodsBarModel model;

                if (!service.Validate(barcode, FormType.Inspection, this.UserContext.CurrentHospital, this.UserContext.UserId, out model, out errorCode))
                {
                    return JsonNet(new ResponseResult(false, GoodsStateValidateCodes.GetMessage(errorCode)));
                }

                return JsonNet(new ResponseResult(true, model));
            }
            catch (Exception e)
            {
                return JsonNet(new ResponseResult(false, e));
            }
        }

        public JsonNetResult CancelInspection(string barcode)
        {
            try
            {
                var service = new GoodsStateService();

                string errorCode;
                GoodsBarModel model;

                if (!service.Cancel(barcode, FormType.Inspection, this.UserContext.CurrentHospital, this.UserContext.UserId, out model, out errorCode))
                {
                    return JsonNet(new ResponseResult(false, GoodsStateValidateCodes.GetMessage(errorCode)));
                }

                return JsonNet(new ResponseResult(true, model));
            }
            catch (Exception e)
            {
                return JsonNet(new ResponseResult(false, e));
            }
        }

        public ActionResult InspectionConfirm()
        {
            var list = new InspectionFormService().QueryConfirm(this.UserContext.RootUnitId);

            var unitService = new UnitService();
            var productService = new ProductService();
            ViewBag.Context = new
            {
                List = list.Select(item =>
                {
                    var unit = unitService.Get(item.ApplyUnitId).Name;
                    var product = productService.Get(item.ProductId).Name;

                    return new
                    {
                        Id = item.Id,
                        orderDetailId = item.OrderDetailId,
                        FormNo = item.OrderFormNo,
                        Unit = unit,
                        Product = product,
                        InspectionCount = item.InspectionCount,
                        ScanCount = new GoodsStateService().CountValid(item.Id, FormType.Inspection)
                    };
                })
            };

            return View();
        }

        public JsonNetResult ConfirmInspection(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("The inspection form id is empty.");
                }

                new InspectionFormService().Confirm(id, this.UserContext.UserId);
                return JsonNet(new ResponseResult());
            }
            catch (Exception e)
            {
                return JsonNet(new ResponseResult(false, e));
            }
        }
        #endregion


        #region Incoming
        //public ActionResult Incoming()
        //{
        //    return View();
        //}

        //public JsonNetResult ValidIncoming(string barcode)
        //{
        //    try
        //    {
        //        var service = new GoodsStateService();

        //        string errorCode;
        //        GoodsBarModel model;

        //        if (!service.Validate(barcode, FormType.Incoming, this.UserContext.CurrentHospital, this.UserContext.UserId, out model, out errorCode))
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

        //public JsonNetResult CancelIncoming(string barcode)
        //{
        //    try
        //    {
        //        var service = new GoodsStateService();

        //        string errorCode;
        //        GoodsBarModel model;

        //        if (!service.Cancel(barcode, FormType.Incoming, this.UserContext.CurrentHospital, this.UserContext.UserId, out model, out errorCode))
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

        //public ActionResult IncomingConfirm()
        //{
        //    var list = new IncomingFormService().QueryConfirm(this.UserContext.CurrentHospital);

        //    var unitService = new UnitService();
        //    var productService = new ProductService();
        //    ViewBag.Context = new
        //    {
        //        List = list.Select(item =>
        //        {
        //            var unit = unitService.Get(item.ApplyUnitId).Name;
        //            var product = productService.Get(item.ProductId).Name;

        //            return new
        //            {
        //                Id = item.Id,
        //                orderDetailId = item.OrderDetailId,
        //                FormNo = item.OrderFormNo,
        //                Unit = unit,
        //                Product = product,
        //                IncomingCount = item.IncomingCount,
        //                ScanCount = new GoodsStateService().CountValid(item.Id, FormType.Incoming)
        //            };
        //        }),
        //        storerooms = unitService.GetStorerooms(this.UserContext.CurrentHospital).Select(item=>new
        //        {
        //            Id = item.Id,
        //            Name = item.Name
        //        })
        //    };

        //    return View();
        //}

        //public JsonNetResult ConfirmIncoming(string id, string storeroomId)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(id))
        //        {
        //            throw new Exception("The incoming form id is empty.");
        //        }

        //        if (string.IsNullOrEmpty(storeroomId))
        //        {
        //            throw new Exception("The storeroom id of incoming form is empty.");
        //        }

        //        new IncomingFormService().Confirm(id, storeroomId, this.UserContext.UserId);
        //        return JsonNet(new ResponseResult());
        //    }
        //    catch (Exception e)
        //    {
        //        return JsonNet(new ResponseResult(false, e));
        //    }
        //}
        #endregion


        #region Splitting
        //public ActionResult Splitting()
        //{
        //    return View();
        //}

        //public JsonNetResult SplitScan(string barcode)
        //{
        //    var goods = new GoodsService().Get(barcode, this.UserContext.CurrentHospital);
        //    if (goods == null)
        //    {
        //        return JsonNet(new ResponseResult(false, "扫描码找不到货品！"));
        //    }

        //    if (!string.IsNullOrEmpty(goods.ParentId))
        //    {
        //        return JsonNet(new ResponseResult(false, "分装的货品不能再分装！"));
        //    }

        //    var product = new ProductService().Get(goods.ProductId);
        //    if (product == null)
        //    {
        //        return JsonNet(new ResponseResult(false, "产品信息不存在！"));
        //    }

        //    var goodsProduct = new GoodsExtraService().Get(goods.SerialId);
        //    if (goodsProduct == null)
        //    {
        //        return JsonNet(new ResponseResult(false, "货品找不到产品信息！"));
        //    }

        //    if (!goodsProduct.NeedSplit)
        //    {
        //        return JsonNet(new ResponseResult(false, "货品不需要分装！"));
        //    }

        //    if (goods.Status == GoodsStatus.Closed)
        //    {
        //        return JsonNet(new ResponseResult(false, "货品已经分装！"));
        //    }
        //    var vendorName = string.Empty;
        //    if(!string.IsNullOrEmpty(goods.VendorId))
        //    {
        //        var unit = new UnitService().Get(goods.VendorId);
        //        vendorName = unit.Name;
        //    }

        //    return JsonNet(new ResponseResult(true, new
        //    {
        //        Barcode = goods.Barcode,
        //        Name = product.Name,
        //        //RemainingCapacity = goods.RemainingCapacity,
        //        ExpiredDate = goods.ExpiredDate,
        //        SplitCapacity = goodsProduct.SplitCapacity,
        //        MiniSplitNumber = goodsProduct.MiniSplitNumber,
        //        SplitExpiredDate = goods.ExpiredDate,
        //        SplitCopies = goodsProduct.SplitCopies,
        //        SplitUnit = goodsProduct.SplitUnit,
        //        Id = goods.Id,
        //        ProductId = goods.ProductId,
        //        GoodsProductId = goods.SerialId,
        //        VendorName = vendorName
        //    }));
        //}

        //public JsonNetResult SplitGoods(int splitCopies)
        //{
        //    try
        //    {
        //        if(splitCopies <= 0)
        //        {
        //            return JsonNet(new ResponseResult());
        //        }

        //        var subBarcodes = new GoodsStateService().GetBarcodes(splitCopies);
        //        return JsonNet(new ResponseResult(true, subBarcodes));
        //    }
        //    catch(Exception e)
        //    {
        //        return JsonNet(new ResponseResult(e));
        //    }
        //}

        //public ActionResult SplittingConfirm()
        //{
        //    ViewBag.Barcode = new GoodsStateService().GetBarcode();

        //    return View();
        //}

        //public JsonNetResult ConfirmSplitting(string barcode, IList<string> children, string splitUnit, DateTime? expiredDate)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(barcode))
        //        {
        //            throw new Exception("The parent barcode is empty.");
        //        }
        //        if(children == null || children.Count == 0)
        //        {
        //            return JsonNet(new ResponseResult());
        //        }
                
        //        new GoodsService().Save(barcode, this.UserContext.CurrentHospital, children, splitUnit, expiredDate, this.UserContext.UserId);
        //        return JsonNet(new ResponseResult());
        //    }
        //    catch (Exception e)
        //    {
        //        return JsonNet(new ResponseResult(false, e));
        //    }
        //}
        #endregion


        #region Grant
        //public ActionResult Grant()
        //{
        //    return View();
        //}

        //public JsonNetResult QueryGrant(ApplyQueryCondition condition, PagerInfo pager)
        //{
        //    try
        //    {
        //        if (condition.StatusList == null || condition.StatusList.Count == 0)
        //        {
        //            condition.StatusList = new List<int>();
        //            condition.StatusList.Add(ApplyFormStatus.Applied);
        //            condition.StatusList.Add(ApplyFormStatus.Granting);
        //            condition.StatusList.Add(ApplyFormStatus.Granted);
        //        }

        //        condition.UserId = this.UserContext.UserId;
        //        condition.HospitalId = this.UserContext.CurrentHospital;

        //        var list = new ApplyFormService().Query(condition, pager);

        //        return JsonNet(new ResponseResult(true, list, pager));
        //    }
        //    catch (Exception e)
        //    {
        //        return JsonNet(new ResponseResult(false, e));
        //    }
        //}

        //public JsonNetResult Granting(string id)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(id))
        //        {
        //            throw new Exception("The parameter is empty.");
        //        }

        //        new ApplyFormService().Granting(id);
        //        return JsonNet(new ResponseResult());
        //    }
        //    catch (Exception e)
        //    {
        //        return JsonNet(new ResponseResult(e));
        //    }
        //}

        //public ActionResult GrantingEdit(string id)
        //{
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        throw new Exception("The parameter is empty.");
        //    }

        //    var productService = new ProductService();
        //    var entity = new ApplyFormService().Get(id);

        //    var list = new List<dynamic>();
        //    foreach (var item in entity.Details)
        //    {
        //        var product = productService.Get(item.ProductId);
        //        list.Add(new
        //        {
        //            Id = item.Id,
        //            ApplyId = item.ApplyId,
        //            ProductId = item.ProductId,
        //            ProductName = product.Name,
        //            ApplyCount = item.ApplyCount,
        //            GrantCount = item.GrantCount
        //        });
        //    }
        //    ViewBag.Context = new
        //    {
        //        Id = id,
        //        List = list
        //    };

        //    return View();
        //}

        //public JsonNetResult GrantScan(string applyId, string barcode)
        //{
        //    try
        //    {
        //        var goodsService = new GoodsService();
        //        var goods = goodsService.Get(barcode, this.UserContext.CurrentHospital);
        //        if(goods == null)
        //        {
        //            return JsonNet(new ResponseResult(false, "货品不存在！"));
        //        }

        //        var goodsStateService = new GoodsStateService();
        //        string errorCode;
        //        if(!goodsStateService.CanValidate(barcode, FormType.Apply, this.UserContext.CurrentHospital, out errorCode))
        //        {
        //            return JsonNet(new ResponseResult(false, GoodsStateValidateCodes.GetMessage(errorCode)));
        //        }

        //        var goodsExtraService = new GoodsExtraService();
        //        var goodsExtra = goodsExtraService.Get(goods.SerialId);
        //        if(goodsExtra == null)
        //        {
        //            throw new Exception("The goods' information is not whole.");
        //        }
        //        if (goodsExtra.NeedSplit)
        //        {
        //            return JsonNet(new ResponseResult(false, "货品需要拆分，不能直接发放！"));
        //        }

        //        var applyFormService = new ApplyFormService();
        //        var detail = applyFormService.GetDetail(applyId, goods.ProductId);
        //        if (detail == null)
        //        {
        //            return JsonNet(new ResponseResult(false, "扫描货品不是领用的产品！"));
        //        }

        //        applyFormService.GrantScan(barcode, detail, this.UserContext.UserId);

        //        return JsonNet(new ResponseResult(true, goods));
        //    }
        //    catch (Exception e)
        //    {
        //        return JsonNet(new ResponseResult(false, e));
        //    }
        //}

        //public JsonNetResult ConfirmGrant(string applyId)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(applyId))
        //        {
        //            throw new Exception("The parameter is empty.");
        //        }

        //        new ApplyFormService().ConfirmGrant(applyId, this.UserContext.CurrentHospital);
        //        return JsonNet(new ResponseResult(true, new { Granted =  true}));
        //    }
        //    catch (Exception e)
        //    {
        //        return JsonNet(new ResponseResult(false, e));
        //    }
        //}
        #endregion


        #region Return
        public ActionResult Return()
        {
            var vendors = new UnitService().GetVendorsByHospital(this.UserContext.CurrentHospital);
            var statuses = new List<dynamic>();
            statuses.Add(new
            {
                Id = ReturnFormStatus.Applying,
                Name = ReturnFormStatus.GetName(ReturnFormStatus.Applying)
            });
            statuses.Add(new
            {
                Id = ReturnFormStatus.Handling,
                Name = ReturnFormStatus.GetName(ReturnFormStatus.Handling)
            });
            statuses.Add(new
            {
                Id = ReturnFormStatus.Confirmed,
                Name = ReturnFormStatus.GetName(ReturnFormStatus.Confirmed)
            });
            //statuses.Add(new
            //{
            //    Id = ReturnFormStatus.Cancel,
            //    Name = ReturnFormStatus.GetName(ReturnFormStatus.Cancel)
            //});

            ViewBag.Context = new
            {
                Vendors = vendors,
                Statuses = statuses
            };

            return View();
        }

        public ActionResult ReturnEdit(string id)
        {
            var unitService = new UnitService();
            var vendors = unitService.GetVendorsByHospital(this.UserContext.CurrentHospital);

            if (string.IsNullOrEmpty(id))
            {
                ViewBag.Context = new
                {
                    FormNo = IdentityCreatorService.New(IdentityKey.APPLY_FORM, this.UserContext.CurrentHospital),
                    ApplyDate = DateTime.Now,
                    Vendors = vendors
                };
            }
            else
            {
                var entity = new ReturnFormService().Get(id);
                if (entity == null)
                {
                    throw new Exception("The return form does not exist.");
                }

                ViewBag.Context = new
                {
                    Form = entity,
                    Vendors = vendors
                };
            }

            return View();
        }

        public JsonNetResult SaveReturn(ReturnFormEntity form)
        {
            if (form == null)
            {
                return JsonNet(new ResponseResult());
            }

            try
            {
                if (string.IsNullOrEmpty(form.Id))
                {
                    form.FilterId = this.UserContext.UserId;
                    form.HospitalId = this.UserContext.CurrentHospital;
                    form.Status = ReturnFormStatus.Applying;
                    form.CreatedId = this.UserContext.UserId;
                    form.UpdatedId = this.UserContext.UserId;
                }

                new ReturnFormService().Save(form);
                return JsonNet(new ResponseResult());
            }
            catch (Exception e)
            {
                return JsonNet(new ResponseResult(false, e));
            }
        }

        public ActionResult ReturnConfirm(string id)
        {
            ViewBag.Context = new
            {
                Id = id
            };

            return View();
        }

        public JsonNetResult ValidReturn(string barcode, string formId)
        {
            try
            {
                if(string.IsNullOrEmpty(barcode))
                {
                    throw new Exception("The goods barcode is empty.");
                }

                if(string.IsNullOrEmpty(formId))
                {
                    throw new Exception("The return form id is empty.");
                }

                var service = new GoodsStateService();

                GoodsBarModel model;
                string errorCode;
                if (service.Validate(barcode, formId, FormType.Return, this.UserContext.UserId, this.UserContext.CurrentHospital, out model, out errorCode))
                {
                    new ReturnFormService().UpdateStatus(formId, ReturnFormStatus.Handling, this.UserContext.UserId);
                    return JsonNet(new ResponseResult(true, new { Name = model.ProductName }));
                }
                else
                {
                    return JsonNet(new ResponseResult(false, GoodsStateValidateCodes.GetMessage(errorCode)));
                }
            }
            catch (Exception e)
            {
                return JsonNet(new ResponseResult(false, e));
            }
        }

        public JsonNetResult CancelReturn(string barcode, string formId)
        {
            try
            {
                if (string.IsNullOrEmpty(barcode))
                {
                    throw new Exception("The goods barcode is empty.");
                }

                if (string.IsNullOrEmpty(formId))
                {
                    throw new Exception("The return form id is empty.");
                }

                var service = new GoodsStateService();

                GoodsBarModel model;
                string errorCode;
                if (service.Cancel(barcode, formId, FormType.Return, this.UserContext.UserId, this.UserContext.CurrentHospital, out model, out errorCode))
                {
                    return JsonNet(new ResponseResult(true, new { Name = model.ProductName }));
                }
                else
                {
                    return JsonNet(new ResponseResult(false, GoodsStateValidateCodes.GetMessage(errorCode)));
                }
            }
            catch (Exception e)
            {
                return JsonNet(new ResponseResult(false, e));
            }
        }

        public JsonNetResult SaveReturnLogistics(string formId, string logisticsBarcode, string logisticsInfo)
        {
            try
            {
                new ReturnFormService().Confirm(formId, logisticsBarcode, logisticsInfo, this.UserContext.UserId);

                return JsonNet(new ResponseResult());
            }
            catch (Exception e)
            {
                return JsonNet(new ResponseResult(false, e));
            }
        }

        public JsonNetResult QueryReturn(ReturnQueryCondition condition, PagerInfo pager)
        {
            try
            {
                condition.HospitalId = this.UserContext.CurrentHospital;

                var list = new ReturnFormService().Query(condition, pager);

                return JsonNet(new ResponseResult(true, list, pager));
            }
            catch (Exception e)
            {
                return JsonNet(new ResponseResult(false, e));
            }
        }
        #endregion


        #region Checking
        public ActionResult Checking()
        {
            return View();
        }

        public ActionResult QueryChecking(DateRangeCondition condition, PagerInfo pager)
        {
            try
            {
                condition.UserId = this.UserContext.UserId;
                condition.HospitalId = this.UserContext.CurrentHospital;

                var list = new GoodsInventoryService().Query(condition, pager);
                return JsonNet(new ResponseResult(true, list, pager));
            }
            catch (Exception e)
            {
                return JsonNet(new ResponseResult(e));
            }
        }

        public ActionResult CheckingPrint()
        {
            return View();
        }

        public ActionResult LoadPrintInventory(DateRangeCondition condition)
        {
            try
            {
                condition.UserId = this.UserContext.UserId;
                condition.HospitalId = this.UserContext.CurrentHospital;

                var list = new GoodsInventoryService().Query(condition);
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
