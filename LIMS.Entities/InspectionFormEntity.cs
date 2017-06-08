using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace LIMS.Entities
{
    public class InspectionFormEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public int ReceiveCount
        {
            get; set;
        }

        public int InspectionCount
        {
            get; set;
        }

        public string ReceiveForm
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

        public string HospitalId
        {
            get; set;
        }

        public string ApplyUnitId
        {
            get; set;
        }

        public string VendorId
        {
            get; set;
        }

        public string ProductId
        {
            get; set;
        }

        public bool ScanOver
        {
            get; set;
        }

        public string ConfirmUserId
        {
            get; set;
        }

        public DateTime ConfirmDateTime
        {
            get; set;
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.ReceiveCount = reader.GetInt32(reader.GetOrdinal("receive_count"));
            this.InspectionCount = reader.GetInt32(reader.GetOrdinal("inspection_count"));
            this.ReceiveForm = reader["receive_form"].ToString();
            this.OrderId = reader["order_id"].ToString();
            this.OrderFormNo = reader.GetInt32(reader.GetOrdinal("order_form_no"));
            this.OrderDetailId = reader["order_detail_id"].ToString();
            this.HospitalId = reader["hospital_id"].ToString();
            this.ApplyUnitId = reader["apply_unit_id"].ToString();
            this.VendorId = reader["vendor_id"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.ScanOver = reader.GetBoolean(reader.GetOrdinal("scan_over"));

            if (reader["confirm_user_id"] != DBNull.Value)
            {
                this.ConfirmUserId = reader["confirm_user_id"].ToString();
            }
            if (reader["confirm_datetime"] != DBNull.Value)
            {
                this.ConfirmDateTime = reader.GetDateTime(reader.GetOrdinal("confirm_datetime"));
            }
        }
    }
}
