using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Entities;
using LIMS.Repositories;
using LIMS.Models;

namespace LIMS.Services
{
    public class ReturnFormService
    {
        public IList<ReturnFormEntity> Query(ReturnQueryCondition condition, PagerInfo pager)
        {
            return ReturnFormRepository.Query(condition, pager);
        }

        public void Save(ReturnFormEntity entity)
        {
            ReturnFormRepository.Save(entity);
        }

        public void UpdateStatus(string id, int status, string userId)
        {
            ReturnFormRepository.UpdateStatus(id, status, userId);
        }

        public ReturnFormEntity Get(string id)
        {
            return ReturnFormRepository.Get(id);
        }

        public void Confirm(string id, string logisticsBarcode, string logisticsInfo, string userId)
        {
            ReturnFormRepository.Confirm(id, logisticsBarcode, logisticsInfo, userId);
        }
    }
}
