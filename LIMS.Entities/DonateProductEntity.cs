using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class DonateProductEntity : BaseEntity
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

        public decimal BaseCount
        {
            get; set;
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.HospitalId = reader["hospital_id"].ToString();
            this.UnitId = reader["unit_id"].ToString();
            this.VendorId = reader["vendor_id"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.BaseCount = reader.GetDecimal(reader.GetOrdinal("base_count"));
        }
    }
}
