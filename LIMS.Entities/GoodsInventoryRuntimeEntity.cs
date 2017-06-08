using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class GoodsInventoryRuntimeEntity
    {
        public string Id
        {
            get; set;
        }

        public string HospitalId
        {
            get; set;
        }

        public string ApplyId
        {
            get; set;
        }

        public string ProductId
        {
            get; set;
        }

        public int ApplyCount
        {
            get; set;
        }

        public int GrantedCount
        {
            get; set;
        }

        public void Init(IDataReader reader)
        {
            this.Id = reader["id"].ToString();
            this.HospitalId = reader["hospital_id"].ToString();
            this.ApplyId = reader["apply_id"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.ApplyCount = reader.GetInt32(reader.GetOrdinal("apply_count"));
            this.GrantedCount = reader.GetInt32(reader.GetOrdinal("granted_count"));
        }
    }
}
