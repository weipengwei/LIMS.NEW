using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class OrderFormDonateItemEntity
    {
        public string Id
        {
            get; set;
        }

        public string OrderId
        {
            get; set;
        }

        public string ItemId
        {
            get; set;
        }

        public string HospitalId
        {
            get; set;
        }

        public string ProductId
        {
            get; set;
        }

        public int Count
        {
            get; set;
        }

        public string CreatedId
        {
            get; set;
        }

        public DateTime CreatedTime
        {
            get; set;
        }

        public void Init(IDataReader reader)
        {
            this.Id = reader["id"].ToString();
            this.OrderId = reader["order_id"].ToString();
            this.ItemId = reader["item_id"].ToString();
            this.HospitalId = reader["hospital_id"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.Count = reader.GetInt32(reader.GetOrdinal("count"));
            this.CreatedId = reader["created_id"].ToString();
            this.CreatedTime = reader.GetDateTime(reader.GetOrdinal("created_time"));
        }
    }
}
