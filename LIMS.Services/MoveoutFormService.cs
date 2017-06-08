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
    public class MoveoutFormService
    {
        public IList<MoveoutFormEntity> Query(DateRangeCondition condition, PagerInfo pager)
        {
            return MoveoutFormRepository.Query(condition, pager);
        }

        public MoveoutFormEntity Get(string id)
        {
            return MoveoutFormRepository.Get(id);
        }

        public void Save(MoveoutFormEntity entity)
        {
            MoveoutFormRepository.Save(entity);
        }

        public void Confirm(string id, string userId)
        {
            MoveoutFormRepository.Confirm(id, userId);
        }

        public bool Cancel(string id, string userId)
        {
            return MoveoutFormRepository.Cancel(id, userId);
        }
    }
}
