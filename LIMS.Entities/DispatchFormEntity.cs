using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class DispatchFormEntity
    {
        public string Id
        {
            get; set;
        }

        public string OrderId
        {
            get; set;
        }

        public int OrderFormNo
        {
            get; set;
        }

        public string OrderDetailId
        {
            get; set;
        }

        public string ApplyUnitId
        {
            get; set;
        }

        public string ProductId
        {
            get; set;
        }

        public int DispatchedCount
        {
            get; set;
        }

        public string HospitalId
        {
            get; set;
        }

        public string VendorId
        {
            get; set;
        }

        public string Status
        {
            get; set;
        }

        public string ChangedId
        {
            get; set;
        }

        public DateTime ChangedTime
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
            this.OrderFormNo = reader.GetInt32(reader.GetOrdinal("order_form_no"));
            this.OrderDetailId = reader["order_detail_id"].ToString();
            this.ApplyUnitId = reader["apply_unit_id"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.DispatchedCount = reader.GetInt32(reader.GetOrdinal("dispatched_count"));
            this.HospitalId = reader["hospital_id"].ToString();
            this.VendorId = reader["vendor_id"].ToString();
            this.Status = reader["status"].ToString();
            this.ChangedId = reader["changed_id"].ToString();
            if (reader["changed_time"] != DBNull.Value)
            {
                this.ChangedTime = reader.GetDateTime(reader.GetOrdinal("changed_time"));
            }
            if (!reader["created_id"].Equals(DBNull.Value))
            {
                this.CreatedId = reader["created_id"].ToString();
            }

            if (!reader["created_time"].Equals(DBNull.Value))
            {
                this.CreatedTime = (DateTime)reader["created_time"];
            }
        }
    }
}
