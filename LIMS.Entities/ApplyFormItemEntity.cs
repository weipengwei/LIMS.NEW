using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class ApplyFormItemEntity
    {
        public string Id
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

        public int Count
        {
            get; set;
        }

        public void Init(IDataReader reader)
        {
            this.Id = reader["id"].ToString();
            this.ApplyId = reader["apply_id"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.Count = reader.GetInt32(reader.GetOrdinal("count"));
        }
    }
}
