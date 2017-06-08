using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class MoveoutFormEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string HospitalId
        {
            get; set;
        }

        public int FormNo
        {
            get; set;
        }

        public string Applyer
        {
            get; set;
        }

        public DateTime ApplyTime
        {
            get; set;
        }

        public string NewStoreroomId
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
            this.HospitalId = reader["hospital_id"].ToString();
            this.FormNo = reader.GetInt32(reader.GetOrdinal("form_no"));
            this.Applyer = reader["applyer"].ToString();
            this.ApplyTime = reader.GetDateTime(reader.GetOrdinal("apply_time"));
            this.NewStoreroomId = reader["new_storeroom_id"].ToString();
            this.ScanOver = reader.GetBoolean(reader.GetOrdinal("scan_over"));
            this.ConfirmUserId = reader["confirm_user_id"].ToString();

            if(reader["confirm_datetime"] != DBNull.Value)
            {
                this.ConfirmDateTime = reader.GetDateTime(reader.GetOrdinal("confirm_datetime"));
            }
        }
    }
}
