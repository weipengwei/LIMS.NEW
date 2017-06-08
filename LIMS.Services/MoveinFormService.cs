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
    public class MoveinFormService
    {
        public void Confirm(string id, string userId, string hospitalId)
        {
            MoveinFormRepository.Confirm(id, userId, hospitalId);
        }

        public IList<MoveFormEntity> QueryMoveForms(string hospitalId)
        {
            return MoveinFormRepository.QueryMoveForms(hospitalId);
        }
    }
}
