using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

namespace LIMS.Entities
{
    public class GoodsCheckDataEntity
    {
        public long Id
        {
            get; set;
        }

        public string Barcode
        {
            get; set;
        }

        public string CheckId
        {
            get; set;
        }

        public string CheckUser
        {
            get; set;
        }

        public DateTime CheckDateTime
        {
            get; set;
        }

        public string Status
        {
            get; set;
        }
        
        public void Init(IDataReader reader)
        {
            this.Id = reader.GetInt64(reader.GetOrdinal("id"));
            this.Barcode = reader["barcode"].ToString();
            this.CheckId = reader["check_id"].ToString();
            this.CheckUser = reader["check_user"].ToString();
            this.CheckDateTime = reader.GetDateTime(reader.GetOrdinal("check_datetime"));
            this.Status = reader["status"].ToString();
        }
    }
}
