using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class GoodsSerialFormEntity
    {
        public string SerialId
        {
            get; set;
        }

        public string FormId
        {
            get; set;
        }

        public string FormKind
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

        public void Init(IDataReader reader)
        {
            this.SerialId = reader["serial_id"].ToString();
            this.FormId = reader["form_id"].ToString();
            this.FormKind = reader["form_kind"].ToString();
            this.CreatedId = reader["created_id"].ToString();
            this.CreatedTime = reader.GetDateTime(reader.GetOrdinal("created_time"));
        }
    }
}
