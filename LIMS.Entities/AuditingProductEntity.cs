using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class AuditingProductEntity : BaseEntity
    {
        public string Id
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

        public string ProductId
        {
            get; set;
        }

        public bool IsAudit
        {
            get; set;
        }

        public int Version
        {
            get; set;
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.HospitalId = reader["hospital_id"].ToString();
            this.VendorId = reader["vendor_id"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.IsAudit = reader.GetBoolean(reader.GetOrdinal("is_audit"));
            this.Version = reader.GetInt32(reader.GetOrdinal("version"));
        }
    }
}
