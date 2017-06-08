using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class ReceiptInfoEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string HospitalId
        {
            get; set;
        }

        public string Title
        {
            get; set;
        }

        public decimal Tax
        {
            get; set;
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.HospitalId = reader["hospital_id"].ToString();
            this.Title = reader["title"].ToString();
            this.Tax = reader.GetDecimal(reader.GetOrdinal("tax"));
        }
    }
}
