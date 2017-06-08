using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class ReturnFormEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public int FormNo
        {
            get; set;
        }

        public string FilterId
        {
            get; set;
        }

        public string VendorId
        {
            get; set;
        }

        public string HospitalId
        {
            get; set;
        }

        public DateTime ApplyDate
        {
            get; set;
        }

        public int Status
        {
            get; set;
        }

        public string LogisticsBarcode
        {
            get; set;
        }

        public string LogisticsInfo
        {
            get; set;
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.FormNo = reader.GetInt32(reader.GetOrdinal("form_no"));
            this.FilterId = reader["filter_id"].ToString();
            this.VendorId = reader["vendor_id"].ToString();
            this.HospitalId = reader["hospital_id"].ToString();
            this.ApplyDate = reader.GetDateTime(reader.GetOrdinal("apply_date"));
            this.Status = reader.GetInt32(reader.GetOrdinal("status"));
            this.LogisticsBarcode = reader["logistics_barcode"].ToString();
            this.LogisticsInfo = reader["logistics_info"].ToString();
        }
    }
}
