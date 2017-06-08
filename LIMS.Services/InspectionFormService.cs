using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Entities;
using LIMS.Repositories;

namespace LIMS.Services
{
    public class InspectionFormService
    {
        public IList<InspectionFormEntity> QueryConfirm(string hospitalId)
        {
            return InspectionFormRepository.QueryConfirm(hospitalId);
        }

        public void Confirm(string id, string userId)
        {
            InspectionFormRepository.Confirm(id, userId);
        }
    }
}
