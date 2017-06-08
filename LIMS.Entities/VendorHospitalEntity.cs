using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class VendorHospitalEntity : BaseEntity
    {
        public string Id
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

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.VendorId = reader["vendor_id"].ToString();
            this.HospitalId = reader["hospital_id"].ToString();
        }
    }
}
