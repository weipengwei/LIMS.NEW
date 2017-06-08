using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIMS.Repositories;
using LIMS.Entities;

namespace LIMS.Services
{
    public class UserPrivilegeService
    {
        public IList<UserPrivilegeEntity> Query(string userId, string rootId)
        {
            return UserPrivilegeRepository.Query(userId, rootId);
        }

        public void Save(string userId, string rootId, IList<UserPrivilegeEntity> entities)
        {
            UserPrivilegeRepository.Save(userId, rootId, entities);
        }
    }
}
