using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Entities;
using LIMS.Repositories;

namespace LIMS.Services
{
    public class VendorHospitalService
    {
        public IList<VendorHospitalEntity> GetByVendor(string vendorId)
        {
            return VendorHospitalsRepository.GetByVendor(vendorId);
        }

        public void Save(string vendorId, IList<VendorHospitalEntity> entities)
        {
            VendorHospitalsRepository.Save(vendorId, entities);
        }
    }
}
