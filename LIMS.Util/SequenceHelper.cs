using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Util
{
    public static class SequenceHelper
    {
        public static int TimeSequence()
        {
            return (int)DateTime.Now.Subtract(new DateTime(2016, 1, 1)).Seconds;
        }
    }
}
