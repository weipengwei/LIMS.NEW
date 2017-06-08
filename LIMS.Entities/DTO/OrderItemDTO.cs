using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class OrderItemDTO
    {
        public OrderFormItemEntity Item
        { get; set; }

        public string StatusName
        { get; set; }

        public ProductEntity Product
        { get; set; }

        public List<DispatchDTO> Dispatches
        { get; set; }

        public List<ReceiveDTO> Receives
        { get; set; }

        /// <summary>
        /// Sum of lines to dispatch
        /// </summary>
        public int QtyToDispatch
        { get; set; }

        /// <summary>
        /// Sum of dispatched lines
        /// </summary>
        public int DispatchedQty
        { get; set; }

        /// <summary>
        /// Sum of received lines
        /// </summary>
        public int ReceivedQty
        { get; set; }

        public int DispatchingQty
        {
            get
            {
                return QtyToDispatch - DispatchedQty;
            }
        }

        public int ReceivingQty
        {
            get
            {
                return DispatchedQty - ReceivedQty;
            }
        }
    }
}
