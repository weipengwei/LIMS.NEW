using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIMS.Entities;
using LIMS.Repositories;

namespace LIMS.Services
{
    public class SystemPrivilegeService
    {
        public IList<SystemPrivilegeEntity> GetByObjectId(string objectId)
        {
            return SystemPrivilegeRepository.GetByObjectId(objectId);
        }

        public void Save(string objectId, IList<SystemPrivilegeEntity> entities)
        {
            SystemPrivilegeRepository.Save(objectId, entities);
        }
    }
}
