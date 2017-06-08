using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Repositories;
using LIMS.Entities;

namespace LIMS.Services
{
    public class DonateProductService
    {
        public void Get(string hospitalId, string unitId, string vendorId, string productId,
            out DonateProductEntity donation, out IList<DonateProductItemEntity> items)
        {
            DonateProductRepository.Get(hospitalId, unitId, vendorId, productId, out donation, out items);
        }
    }
}
