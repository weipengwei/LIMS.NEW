using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Util;

namespace LIMS.Entities
{
    public class FormApproveListEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public FormType FormType
        {
            get; set;
        }

        public string HospitalId
        {
            get; set;
        }

        public ApproverType ApproverType
        {
            get; set;
        }

        public string ApproverId
        {
            get; set;
        }

        public int Sequence
        {
            get; set;
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();

            if (reader["form_type"] != DBNull.Value)
            {
                var value = reader.GetInt32(reader.GetOrdinal("form_type"));
                if (Enum.IsDefined(typeof(FormType), value))
                {
                    this.FormType = (FormType)value;
                }
            }

            this.HospitalId = reader["hospital_id"].ToString();

            if(reader["approver_type"] != DBNull.Value)
            {
                var value = reader.GetInt32(reader.GetOrdinal("approver_type"));
                if (Enum.IsDefined(typeof(ApproverType), value))
                {
                    this.ApproverType = (ApproverType)value;
                }
            }

            this.ApproverId = reader["approver_id"].ToString();
            this.Sequence = reader.GetInt32(reader.GetOrdinal("sequence"));
        }
    }
}
