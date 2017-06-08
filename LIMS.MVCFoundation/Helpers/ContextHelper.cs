using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using LIMS.MVCFoundation.Core;

namespace LIMS.MVCFoundation.Helpers
{
    public static class ContextHelper
    {
        public static CustomPrincipal CurrentPrincipal
        {
            get
            {
                return Thread.CurrentPrincipal as CustomPrincipal;
            }
        }
    }
}
