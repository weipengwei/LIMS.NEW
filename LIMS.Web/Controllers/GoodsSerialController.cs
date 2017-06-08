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

namespace LIMS.Web.Controllers
{
    [RequiredLogon]
    public class GoodsSerialController : BaseController
    {
        [Authorization("Dispatch")]
        public ActionResult Barcodes(string serialId)
        {
            return View(new { SerialId = serialId });
        }

        [Authorization("Dispatch")]
        public JsonNetResult QueryBarcodes(string serialId)
        {
            if (string.IsNullOrEmpty(serialId))
            {
                return JsonNet(new ResponseResult());
            }

            try
            {
                var goodsSerialService = new GoodsSerialService();

                var goodsSerial = goodsSerialService.Get(serialId);
                if(goodsSerial == null)
                {
                    return JsonNet(new ResponseResult(true, "发送的货品不存在！"));
                }
                
                var product = new ProductService().Get(goodsSerial.ProductId);
                if(product == null)
                {
                    return JsonNet(new ResponseResult(true, "发送的货品没有对应的产品信息！"));
                }

                var vendor = new UnitService().Get(goodsSerial.VendorId);
                if(vendor == null)
                {
                    return JsonNet(new ResponseResult(true, "发送的货品没有对应的供应商信息！"));
                }

                var barcodes = goodsSerialService.GetBarcodes(serialId);
                var result = new
                {
                    SerialNo = goodsSerial.SerialNo,
                    Product = product.Name,
                    Vendor = vendor.Name,
                    Count = goodsSerial.DispatchedCount,
                    Barcodes = barcodes.Select(item => new
                    {
                        Barcode = item.Barcode,
                        IsPrinted = item.IsPrinted
                    })
                };

                return JsonNet(new ResponseResult(true, result));
            }
            catch(Exception e)
            {
                return JsonNet(new ResponseResult(e));
            }
        }

        [Authorization("Dispatch")]
        public JsonNetResult GetBarcodesByRoot(string serialId)
        {
            if (string.IsNullOrEmpty(serialId))
            {
                return JsonNet(new ResponseResult());
            }

            try
            {
                var goodsSerialService = new GoodsSerialService();

                var goodsSerial = goodsSerialService.Get(serialId);
                if (goodsSerial == null)
                {
                    return JsonNet(new ResponseResult(true, "发送的货品不存在！"));
                }

                var product = new ProductService().Get(goodsSerial.ProductId);
                if (product == null)
                {
                    return JsonNet(new ResponseResult(true, "发送的货品没有对应的产品信息！"));
                }

                var vendor = new UnitService().Get(goodsSerial.VendorId);
                if (vendor == null)
                {
                    return JsonNet(new ResponseResult(true, "发送的货品没有对应的供应商信息！"));
                }

                var barcodes = goodsSerialService.GetBarcodesByRoot(serialId);
                var result = new
                {
                    SerialNo = goodsSerial.SerialNo,
                    Product = product.Name,
                    Vendor = vendor.Name,
                    Count = goodsSerial.DispatchedCount,
                    Barcodes = barcodes.Select(item => new
                    {
                        Barcode = item.Barcode,
                        IsPrinted = item.IsPrinted
                    })
                };

                return JsonNet(new ResponseResult(true, result));
            }
            catch (Exception e)
            {
                return JsonNet(new ResponseResult(e));
            }
        }

        [Authorization("Dispatch", "Receive", "Incoming","ReceiveIncoming")]
        public JsonNetResult LoadByBarcode(string serialId, string barcode, string formKind)
        {
            if (string.IsNullOrEmpty(barcode))
            {
                return JsonNet(new ResponseResult());
            }

            try
            {
                var goodsSerial = new GoodsSerialService().GetByBarcode(barcode);
                if (goodsSerial==null || (!string.IsNullOrEmpty(serialId) && goodsSerial.Id!=serialId))
                {
                    return JsonNet(new ResponseResult(true, "条码不存在！"));
                }
                if (goodsSerial.IsClosed)
                {
                    return JsonNet(new ResponseResult(true, "条码对应的发货已经关闭（院方已入库或退货）！"));
                }

                if (!this.UserContext.HospitalOrVendor && string.Compare(goodsSerial.VendorId, this.UserContext.RootUnitId, true) != 0)
                {
                    return JsonNet(new ResponseResult(true, "找不到条码对应的发货信息！"));
                }

                if (string.Compare(goodsSerial.HospitalId, this.UserContext.CurrentHospital, true) != 0)
                {
                    return JsonNet(new ResponseResult(true, "条码对应的货品不属于当前院方！"));
                }

                var formService = new GoodsSerialFormService();
                var currentForm = formService.GetBySerialId(goodsSerial.Id);
                if (currentForm == null)
                {
                    return JsonNet(new ResponseResult(true, "找不到扫码对应的发货信息！"));
                }
                else if (string.Compare(currentForm.FormKind, formKind, true) != 0)
                {
                    if(string.Compare(currentForm.FormKind, FormKind.Incoming, true) == 0)
                    {
                        var incoming = new IncomingFormService().Get(currentForm.FormId);
                        if (incoming == null)
                        {
                            throw new Exception("已在入库阶段，但是没有入库单！");
                        }

                        if (incoming.IsConfirmed)
                        {
                            return JsonNet(new ResponseResult(true, "货品已经入库！"));
                        }
                    }

                    return JsonNet(new ResponseResult(true, formService.GetFormMessage(currentForm)));
                }
                else if (string.Compare(currentForm.FormKind, FormKind.Incoming, true) == 0)
                {
                    var incoming = new IncomingFormService().Get(currentForm.FormId);
                    if (incoming == null)
                    {
                        throw new Exception("已在入库阶段，但是没有入库单！");
                    }

                    if (incoming.IsConfirmed)
                    {
                        return JsonNet(new ResponseResult(true, "货品已经入库！"));
                    }
                }

                var product = new ProductService().Get(goodsSerial.ProductId);
                if (product == null)
                {
                    return JsonNet(new ResponseResult(true, "条码对应的货品没有产品信息！"));
                }

                var vendor = new UnitService().Get(goodsSerial.VendorId);
                if (vendor == null)
                {
                    return JsonNet(new ResponseResult(true, "条码对应的货品没有供应商信息！"));
                }

                return JsonNet(new ResponseResult(true, new
                {
                    FormId = currentForm.FormId,
                    SerialId = goodsSerial.Id,
                    SerialNo = goodsSerial.SerialNo,
                    Product = product.Name,
                    Vendor = vendor.Name,
                    Count = goodsSerial.DispatchedCount,
                    BatchNo = goodsSerial.BatchNo,
                    ExpiredDate = goodsSerial.ExpiredDate,
                    LogisticsCode = goodsSerial.LogisticsCode,
                    LogisticsContent = goodsSerial.LogisticsContent
                }));
            }
            catch (Exception e)
            {
                return JsonNet(new ResponseResult(e));
            }
        }

        [Authorization("Dispatch")]
        public JsonNetResult UpdatePrint(string serialId, IList<string> barcodes)
        {
            if(string.IsNullOrEmpty(serialId) || barcodes == null || barcodes.Count == 0)
            {
                return JsonNet(new ResponseResult());
            }

            try
            {
                new GoodsSerialService().UpdatePrint(serialId, barcodes);

                return JsonNet(new ResponseResult());
            }
            catch(Exception e)
            {
                return JsonNet(new ResponseResult(e));
            }
        }
    }
}
