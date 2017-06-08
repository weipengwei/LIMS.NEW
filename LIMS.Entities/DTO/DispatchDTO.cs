using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class DispatchDTO
    {
        public DispatchFormEntity Dispatch
        { get; set; }

        public List<DispatchFormItemEntity> Items
        { get; set; }
    }
}
