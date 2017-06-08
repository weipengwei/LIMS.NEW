using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIMS.Entities;
using LIMS.Models;
using LIMS.Repositories;

namespace LIMS.Services
{
    public class GoodsInventoryService
    {
        //public GoodsInventoryEntity Get(string productId, string hospitalId)
        //{
        //    return GoodsInventoryRepository.Get(productId, hospitalId);
        //}

        //public IList<GoodsInventoryEntity> Get(IList<string> productIds, string hospitalId)
        //{
        //    return GoodsInventoryRepository.Get(productIds, hospitalId);
        //}

        public int CountLeft(string productId, string storeroomId, string hospitalId)
        {
            return GoodsInventoryRepository.CountLeft(productId, storeroomId, hospitalId);
        }

        public int SumLeft(string applyId, string productId, string hospitalId)
        {
            return GoodsInventoryRepository.SumLeft(applyId, productId, hospitalId);
        }

        public GoodsInventoryEntity QueryEarlyExpiredDateInventory(string productId, DateTime expiredDate, string hospitalId)
        {
            return GoodsInventoryRepository.QueryEarlyExpiredDateInventory(productId, expiredDate, hospitalId);
        }

        public GoodsInventoryEntity QueryEarlyExpiredDateInventory(string productId, string storeroomId, DateTime expiredDate, string hospitalId)
        {
            return GoodsInventoryRepository.QueryEarlyExpiredDateInventory(productId, storeroomId, expiredDate, hospitalId);
        }

        public IList<GoodsInventoryRuntimeEntity> GetRuntime(string applyId, string hospitalId)
        {
            return GoodsInventoryRepository.GetRuntime(applyId, hospitalId);
        }

        public IList<ProductInventoryEntity> QueryProductInventory(ProductInventoryCondition condition)
        {
            return GoodsInventoryRepository.QueryProductInventory(condition);
        }




        public IList<GoodsInventoryEntity> Query(DateRangeCondition condition, PagerInfo pageInfo)
        {
            return GoodsInventoryRepository.Query(condition, pageInfo);
        }

        public IList<GoodsInventoryEntity> Query(DateRangeCondition condition)
        {
            return GoodsInventoryRepository.Query(condition);
        }
    }
}
