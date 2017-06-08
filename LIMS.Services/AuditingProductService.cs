using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Entities;
using LIMS.Repositories;

namespace LIMS.Services
{
    public class AuditingProductService
    {
        public IList<AuditingProductEntity> QueryLatestVesion(string hospitalId, string vendorId)
        {
            var list = AuditingProductRepository.Query(hospitalId, vendorId);
            var dic = new Dictionary<string, AuditingProductEntity>();

            foreach(var item in list)
            {
                if(dic.Keys.Contains(item.ProductId))
                {
                    continue;
                }

                dic[item.ProductId] = item;
            }

            return dic.Values.ToList();
        }

        public IDictionary<string, AuditingProductEntity> QueryValidVersion(string hospitalId, string vendorId, DateTime orderApplyTime)
        {
            var list = AuditingProductRepository.Query(hospitalId, vendorId, orderApplyTime);
            var dic = new Dictionary<string, AuditingProductEntity>();

            foreach(var item in list)
            {
                if (dic.Keys.Contains(item.ProductId))
                {
                    continue;
                }

                dic[item.ProductId] = item;
            }

            return dic;
        }

        public void Save(AuditingProductEntity entity)
        {
            AuditingProductRepository.Save(entity);
        }
    }
}
