using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

namespace LIMS.Entities
{
    public class GoodsCheckEntity
    {
        public string Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string BelongTo
        {
            get; set;
        }

        public string HospitalId
        {
            get; set;
        }

        public string Status
        {
            get; set;
        }

        public string CreatedId
        {
            get; set;
        }

        public DateTime CreatedTime
        {
            get; set;
        }

        public string CloserId
        {
            get; set;
        }

        public DateTime ClosedTime
        {
            get; set;
        }

        public void Init(IDataReader reader)
        {
            this.Id = reader["id"].ToString();
            this.Name = reader["name"].ToString();
            this.BelongTo = reader["belong_to"].ToString();
            this.HospitalId = reader["hospital_id"].ToString();
            this.Status = reader["status"].ToString();
            this.CreatedId = reader["created_id"].ToString();
            this.CreatedTime = reader.GetDateTime(reader.GetOrdinal("created_time"));
            this.CloserId = reader["closer_id"].ToString();

            if(reader["closed_time"] != DBNull.Value)
            {
                this.ClosedTime = reader.GetDateTime(reader.GetOrdinal("closed_time"));
            }
        }
    }
}
