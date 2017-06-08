using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class OrderFormEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string FillerId
        {
            get; set;
        }

        public DateTime ApplyTime
        {
            get; set;
        }

        public int FormNo
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

        public string VendorName
        {
            get; set;
        }

        public string ReceiptId
        {
            get; set;
        }

        public string Status
        {
            get; set;
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.FillerId = reader["filler_id"].ToString();
            this.ApplyTime = reader.GetDateTime(reader.GetOrdinal("apply_time"));
            this.FormNo = reader.GetInt32(reader.GetOrdinal("form_no"));
            this.HospitalId = reader["hospital_id"].ToString();
            this.ApplyUnitId = reader["apply_unit_id"].ToString();
            this.VendorId = reader["vendor_id"].ToString();
            this.ReceiptId = reader["receipt_id"].ToString();
            this.Status = reader["status"].ToString();
        }
    }
}
