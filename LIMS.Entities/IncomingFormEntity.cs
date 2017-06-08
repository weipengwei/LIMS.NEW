using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace LIMS.Entities
{
    public class IncomingFormEntity
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

        public string SerialId
        {
            get; set;
        }

        public string ProductId
        {
            get; set;
        }

        public int IncomingCount
        {
            get; set;
        }

        public string StoreroomId
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

        public bool IsConfirmed
        {
            get; set;
        }

        public string ConfirmedId
        {
            get; set;
        }

        public DateTime ConfirmedTime
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
            this.SerialId = reader["serial_id"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.IncomingCount = reader.GetInt32(reader.GetOrdinal("incoming_count"));
            this.StoreroomId = reader["storeroom_id"].ToString();
            this.HospitalId = reader["hospital_id"].ToString();
            this.VendorId = reader["vendor_id"].ToString();
            this.IsConfirmed = reader.GetBoolean(reader.GetOrdinal("is_confirmed"));
            this.ConfirmedId = reader["confirmed_id"].ToString();
            if (reader["confirmed_time"] != DBNull.Value)
            {
                this.ConfirmedTime = reader.GetDateTime(reader.GetOrdinal("confirmed_time"));
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
