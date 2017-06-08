using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Models;
using LIMS.Entities;
using LIMS.Repositories;

namespace LIMS.Services
{
    public class ReceiptInfoService
    {
        public void Save(ReceiptInfoEntity entity)
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString();
                ReceiptInfoRepository.Add(entity);
            }
            else
            {
                ReceiptInfoRepository.Update(entity);
            }
        }

        public IList<ReceiptInfoModel> GetByHospital(string hospitalId)
        {
            var result =  ReceiptInfoRepository.GetByHospital(hospitalId);

            var list = new List<ReceiptInfoModel>();
            foreach(var item in result)
            {
                list.Add(new ReceiptInfoModel
                {
                    Id = item.Id,
                    HospitalId = item.HospitalId,
                    Title = item.Title,
                    Tax = item.Tax
                });
            }

            return list;
        }

        public ReceiptInfoModel Get(string id)
        {
            var result = ReceiptInfoRepository.Get(id);

            return new ReceiptInfoModel
            {
                Id = result.Id,
                Title = result.Title,
                Tax = result.Tax
            };
        }
    }
}
