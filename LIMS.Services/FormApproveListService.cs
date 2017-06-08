using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Util;
using LIMS.Entities;
using LIMS.Repositories;

namespace LIMS.Services
{
    public class FormApproveListService
    {
        public IList<FormApproveListEntity> Get(FormType formType)
        {
            return FormApproveListRepository.Get(formType);
        }

        public void Delete(string id)
        {
            FormApproveListRepository.Delete(id);
        }

        public void Save(FormApproveListEntity entity)
        {
            FormApproveListRepository.Save(entity);
        }

        public void Move(string current, string exchange, string userId)
        {
            FormApproveListRepository.Move(current, exchange, userId);
        }
    }
}
