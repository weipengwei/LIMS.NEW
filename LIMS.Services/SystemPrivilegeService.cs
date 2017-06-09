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
        /// <summary>
        /// 根据objectId和objectType获取SystemPrivilege
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public IList<SystemPrivilegeEntity> GetByObjectId(string objectId, int objectType = -1)
        {
            return SystemPrivilegeRepository.GetByObjectId(objectId, objectType);
        }

        public void Save(string objectId, IList<SystemPrivilegeEntity> entities)
        {
            SystemPrivilegeRepository.Save(objectId, entities);
        }
    }
}
