using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Entities;
using LIMS.Repositories;

namespace LIMS.Services
{
    public class HospitalProductService
    {
        public HospitalProductEntity Get(string unitId, string productId)
        {
            return HospitalProductRepository.Get(unitId, productId);
        }

        public IList<HospitalProductEntity> GetByUnit(string unitId)
        {
            return HospitalProductRepository.GetByUnit(unitId);
        }

        public IList<string> GetCategories(string hospitalId)
        {
            return HospitalProductRepository.GetCategories(hospitalId);
        }

        public void Save(HospitalProductEntity entity)
        {
            HospitalProductRepository.Save(entity);
        }
    }
}
