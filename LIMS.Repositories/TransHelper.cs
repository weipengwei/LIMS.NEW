using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace LIMS.Repositories
{
    internal class TransHelper
    {
        public static string UpdateLock(DbTransaction trans)
        {
            return trans == null ? "" : " WITH (HOLDLOCK, UPDLOCK) ";
        }
    }
}
