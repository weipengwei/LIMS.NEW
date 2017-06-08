using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class DonateProductItemEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string ParentId
        {
            get; set;
        }

        public string ProductId
        {
            get; set;
        }

        public decimal Count
        {
            get; set;
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.ParentId = reader["parent_id"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.Count = reader.GetDecimal(reader.GetOrdinal("count"));
        }
    }
}
