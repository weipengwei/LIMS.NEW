using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public class SystemFunctionMode
    {
        public string Id
        {
            get; set;
        }

        public string Title
        {
            get; set;
        }

        public bool IsMenu
        {
            get; set;
        }

        public string FunKey
        {
            get; set;
        }

        public string Url
        {
            get; set;
        }

        public bool IsActive
        {
            get; set;
        }

        public int Sequence
        {
            get; set;
        }

        public int DisplayMode
        {
            get; set;
        }

        public IList<SystemFunctionMode> Children
        {
            get; set;
        }
    }
}
