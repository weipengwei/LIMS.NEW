using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Util
{
    public class InvalidException : Exception
    {
        public string Alert { private set; get; }

        public InvalidException(string message): base(message)
        {
            this.Alert = message;
        }
    }
}
