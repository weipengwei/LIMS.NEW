using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class MoveinFormEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string MoveoutId
        {
            get; set;
        }

        public int MoveoutFormNo
        {
            get; set;
        }

        public bool ScanOver
        {
            get; set;
        }

        public string ConfirmUserId
        {
            get; set;
        }

        public DateTime ConfirmDateTime
        {
            get; set;
        }
        
        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.MoveoutId = reader["moveout_id"].ToString();
            this.MoveoutFormNo = reader.GetInt32(reader.GetOrdinal("moveout_form_no"));
            this.ScanOver = reader.GetBoolean(reader.GetOrdinal("scan_over"));
            this.ConfirmUserId = reader["confirm_user_id"].ToString();

            if(reader["confirm_datetime"] != DBNull.Value)
            {
                this.ConfirmDateTime = reader.GetDateTime(reader.GetOrdinal("confirm_datetime"));
            }
        }
    }
}
