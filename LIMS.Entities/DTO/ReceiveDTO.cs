using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class ReceiveDTO
    {
        public ReceiveFormEntity Receive
        { get; set; }

        public List<ReceiveFormItemEntity> Items
        { get; set; }
    }
}
