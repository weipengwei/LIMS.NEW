using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

using LIMS.Util;

namespace LIMS.Entities
{
    public class HospitalProductDonationEntity
    {
        public string Id
        {
            get; set;
        }

        public string HospitalId
        {
            get; set;
        }

        public string UnitId
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

        public decimal TotalCount
        {
            get; set;
        }

        public decimal UsedCount
        {
            get; set;
        }

        public void Init(IDataReader reader)
        {
            this.Id = reader["id"].ToString();
            this.HospitalId = reader["hospital_id"].ToString();
            this.UnitId = reader["unit_id"].ToString();
            this.VendorId = reader["vendor_id"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.TotalCount = reader.GetDecimal(reader.GetOrdinal("total_count"));
            this.UsedCount = reader.GetDecimal(reader.GetOrdinal("used_count"));
        }
    }
}
