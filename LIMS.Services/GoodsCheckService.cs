using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Entities;
using LIMS.Repositories;

namespace LIMS.Services
{
    public class GoodsCheckService
    {
        public IList<GoodsCheckEntity> Get(string hospitalId)
        {
            return GoodsCheckRepository.Get(hospitalId);
        }

        public IList<GoodsCheckEntity> GetByHospitals(IList<string> hospitalIds)
        {
            return GoodsCheckRepository.GetByHospitals(hospitalIds);
        }

        public bool Exist(string checkId, string hospitalId)
        {
            return GoodsCheckRepository.Exist(checkId, hospitalId);
        }

        public void Save(GoodsCheckEntity entity)
        {
            GoodsCheckRepository.Save(entity);
        }

        public void Close(string id, string userId)
        {
            GoodsCheckRepository.Close(id, userId);
        }
    }
}