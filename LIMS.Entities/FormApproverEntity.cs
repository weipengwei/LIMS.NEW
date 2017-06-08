using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class FormApproverEntity
    {
        public string Id
        {
            get; set;
        }

        public string HospitalId
        {
            get; set;
        }

        public int FormType
        {
            get; set;
        }

        public string FormId
        {
            get; set;
        }

        public int FormNo
        {
            get; set;
        }

        public string ApproverId
        {
            get; set;
        }

        public DateTime? ApprovedTime
        {
            get; set;
        }

        public int Status
        {
            get; set;
        }

        public int Sequence
        {
            get; set;
        }

        public string Remark
        {
            get; set;
        }

        public void Init(IDataReader reader)
        {
            this.Id = reader["id"].ToString();
            this.HospitalId = reader["hospital_id"].ToString();
            this.FormType = reader.GetInt32(reader.GetOrdinal("form_type"));
            this.FormId = reader["form_id"].ToString();
            this.FormNo = reader.GetInt32(reader.GetOrdinal("form_no"));
            this.ApproverId = reader["approver_id"].ToString();
            this.Status = reader.GetInt32(reader.GetOrdinal("status"));
            this.Sequence = reader.GetInt32(reader.GetOrdinal("sequence"));
            this.Remark = reader["remark"].ToString();

            if(reader["approved_time"] != DBNull.Value)
            {
                this.ApprovedTime = reader.GetDateTime(reader.GetOrdinal("approved_time"));
            }
        }
    }
}
