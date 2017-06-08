using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class OrderDTO
    {
        public OrderFormEntity Order
        { get; set; }

        public List<OrderItemDTO> Items
        { get; set; }

        public string Requestor
        { get; set; }

        public string StatusName
        { get; set; }
    }
}
