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
    public class SystemFunctionService
    {
        public IList<SystemFunctionEntity> GetAll()
        {
            return SystemFunctionRepository.GetAll();
        }

        public IList<SystemFunctionEntity> GetUserFunctions(string unitRootId, string userId)
        {
            return SystemFunctionRepository.GetUserFunctions(unitRootId, userId);
        }

        public SystemFunctionEntity GetSettingFunction()
        {
            return SystemFunctionRepository.GetSettingFunction();
        }
    }
}
