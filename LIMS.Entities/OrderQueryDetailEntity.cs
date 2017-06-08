using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Util;

namespace LIMS.Entities
{
    public class OrderQueryDetailEntity : OrderFormItemEntity
    {
        public string UnitName
        {
            get; set;
        }

        public string ProductName
        {
            get; set;
        }

        public DateTime RegistedDate
        {
            get; set;
        }

        public string StatusName
        {
            get; set;
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.UnitName = reader["unit_name"].ToString();
            this.ProductName = reader["product_name"].ToString();
            this.RegistedDate = reader.GetDateTime(reader.GetOrdinal("registed_time"));
            this.StatusName = OrderFormItemStatus.GetName(this.Status);
        }
    }
}
