using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class VendorProductEntity : BaseEntity
    {
        public string Id
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

        public string Alias
        {
            get; set;
        }

        public string RegistrationId
        {
            get; set;
        }

        public DateTime? ExpirationDate
        {
            get; set;
        }

        public string UnitId
        {
            get; set;
        }

        public bool IsActive
        {
            get; set;
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.VendorId = reader["vendor_id"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.Alias = reader["alias"].ToString();
            this.RegistrationId = reader["registration_id"].ToString();
            if(reader["expiration_date"] != DBNull.Value)
            {
                this.ExpirationDate = reader.GetDateTime(reader.GetOrdinal("expiration_date"));
            }
            this.UnitId = reader["unit_id"].ToString();
            this.IsActive = reader.GetBoolean(reader.GetOrdinal("is_active"));
        }
    }
}
