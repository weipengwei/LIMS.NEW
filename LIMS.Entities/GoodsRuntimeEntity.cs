using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class GoodsRuntimeEntity
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

        public string SerialId
        {
            get;set;
        }

        public string Barcode
        {
            get; set;
        }

        public int Count
        {
            get; set;
        }

        public string ProductId
        {
            get; set;
        }

        public void Init(IDataReader reader)
        {
            this.Id = reader["id"].ToString();
            this.HospitalId = reader["hospital_id"].ToString();
            this.ApplyId = reader["apply_id"].ToString();
            this.SerialId = reader["serial_id"].ToString();
            this.Barcode = reader["barcode"].ToString();
            this.Count = reader.GetInt32(reader.GetOrdinal("count"));
            this.ProductId = reader["product_id"].ToString();
        }
    }
}
