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
    public class HospitalProductDonationService
    {
        public IList<HospitalProductDonationEntity> GetDonationByProducts(string hospitalId, string unitId, string vendorId, IList<string> products)
        {
            return HospitalProductDonationRepository.GetDonationByProducts(hospitalId, unitId, vendorId, products);
        }
    }
}
