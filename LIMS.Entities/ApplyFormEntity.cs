using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using LIMS.Util;

namespace LIMS.Entities
{
    public class ApplyFormEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string FillerId
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

        public string ApplyUnitId
        {
            get; set;
        }

        public DateTime ApplyDate
        {
            get; set;
        }

        public string HospitalId
        {
            get; set;
        }

        public string StoreroomId
        {
            get; set;
        }

        public string Status
        {
            get; set;
        }

        public bool ShowSplit
        {
            get; set;
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.FormNo = reader.GetInt32(reader.GetOrdinal("form_no"));
            this.FillerId = reader["filler_id"].ToString();
            this.Applyer = reader["applyer"].ToString();
            this.ApplyUnitId = reader["apply_unit_id"].ToString();
            this.ApplyDate = reader.GetDateTime(reader.GetOrdinal("apply_date"));
            this.HospitalId = reader["hospital_id"].ToString();
            this.StoreroomId = reader["storeroom_id"].ToString();
            this.Status = reader["status"].ToString();
            this.ShowSplit = reader.GetBoolean(reader.GetOrdinal("show_split"));
        }
    }

    public class ApplyFormDetailEntity
    {
        public string Id
        {
            get; set;
        }

        public string ApplyId
        {
            get; set;
        }

        public int FormNo
        {
            get; set;
        }

        public string ProductId
        {
            get; set;
        }

        public string ProductName
        {
            get; set;
        }

        public int ApplyCount
        {
            get; set;
        }

        public int GrantCount
        {
            get; set;
        }

        public void Init(IDataReader reader)
        {
            this.Id = reader["id"].ToString();
            this.ApplyId = reader["apply_id"].ToString();
            this.FormNo = reader.GetInt32(reader.GetOrdinal("form_no"));
            this.ProductId = reader["product_id"].ToString();
            this.ApplyCount = reader.GetInt32(reader.GetOrdinal("apply_count"));
            this.GrantCount = reader.GetInt32(reader.GetOrdinal("grant_count"));
        }
    }
}
