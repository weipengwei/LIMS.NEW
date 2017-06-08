using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Repositories;
using LIMS.Entities;

namespace LIMS.Services
{
    public class VendorProductService
    {
        public IList<VendorProductEntity> Query(string vendorUnitId)
        {
            return VendorProductRepository.Query(vendorUnitId);
        }

        public IList<VendorProductEntity> QueryByVendor(string vendorId)
        {
            return VendorProductRepository.QueryByVendor(vendorId);
        }

        public VendorProductEntity Get(string unitId, string productId)
        {
            return VendorProductRepository.Get(unitId, productId);
        }

        public void Save(VendorProductEntity entity)
        {
            VendorProductRepository.Save(entity);
        }
    }
}
