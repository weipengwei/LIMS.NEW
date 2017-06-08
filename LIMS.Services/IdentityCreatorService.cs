using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Repositories;

namespace LIMS.Services
{
    public static class IdentityCreatorService
    {
        public static int New(string key)
        {
            return IdentityCreatorRepository.Get(key, 1);
        }

        public static int New(string key, string dimension)
        {
            return IdentityCreatorRepository.Get(key, dimension, 1);
        }

        public static int NewStart(string key, int count)
        {
            return IdentityCreatorRepository.Get(key, count);
        }

        public static int NewStart(string key, string dimension, int count)
        {
            return IdentityCreatorRepository.Get(key, dimension, count);
        }
    }
}
