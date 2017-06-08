using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Models;
using LIMS.Entities;
using LIMS.Repositories;
using LIMS.Util;

namespace LIMS.Services
{
    public class GoodsService
    {
        public GoodsEntity Get(string barcode, string hospitalId)
        {
            return GoodsRepsitory.Get(barcode, hospitalId);
        }

        public IList<GoodsEntity> GetByParent(string parentId, string hospitalId)
        {
            return GoodsRepsitory.GetByParent(parentId, hospitalId);
        }

        public GoodsEntity GetOneBySerial(string serialId, string hospitalId)
        {
            return GoodsRepsitory.GetOneBySerial(serialId, hospitalId);
        }

        public int SumRuntime(string barcode, string hospitalId)
        {
            return GoodsRepsitory.SumRuntime(barcode, hospitalId);
        }

        public int SumRuntime(string barcode, string applyId, string hospitalId)
        {
            return GoodsRepsitory.SumRuntime(barcode, applyId, hospitalId);
        }

        public IList<GoodsRuntimeEntity> GetRuntime(string applyId)
        {
            return GoodsRepsitory.GetRuntime(applyId);
        }

        //public void Save(string barcode, string hospital, IList<string> children, string splitUnit, DateTime? expiredDate, string userId)
        //{
        //    GoodsRepsitory.Split(barcode, hospital, children, splitUnit, expiredDate, userId);
        //}

        public GoodsRuntimeEntity GetRuntime(string barcode, string hospitalId)
        {
            return GoodsRepsitory.GetRuntime(barcode, hospitalId);
        }

        public IList<GoodsEntity> Split(GoodsEntity goods, GoodsSerialEntity goodsSerial, string userId)
        {
            return GoodsRepsitory.Split(goods, goodsSerial, userId);
        }

        //public IList<ProductInventoryEntity> QueryProductInventory(ProductInventoryCondition condition)
        //{
        //    var list = GoodsRepsitory.QueryProductInventory(condition);

        //    var unitService = new UnitService();
        //    var productService = new ProductService();
        //    foreach(var item in list)
        //    {
        //        var storeroom = unitService.Get(item.StoreroomId);
        //        if(storeroom == null)
        //        {
        //            item.StoreroomName = "Unknow";
        //        }
        //        else
        //        {
        //            item.StoreroomName = storeroom.Name;
        //        }

        //        var product = productService.Get(item.ProductId);
        //        if(product == null)
        //        {
        //            item.ProductName = "Unknow";
        //        }
        //        else
        //        {
        //            item.ProductName = product.Name;
        //        }
        //    }

        //    return list;
        //}
    }
}
