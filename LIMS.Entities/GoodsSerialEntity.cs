using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class GoodsSerialEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string SerialNo
        {
            get; set;
        }

        public string ParentId
        {
            get;set;
        }

        public string ProductId
        {
            get; set;
        }

        public int DispatchedCount
        {
            get; set;
        }

        public DateTime? ExpiredDate
        {
            get; set;
        }

        public string HospitalId
        {
            get; set;
        }

        public string VendorId
        {
            get; set;
        }

        public bool NeedAudit
        {
            get; set;
        }

        public bool NeedCheck
        {
            get; set;
        }

        public bool NeedSplit
        {
            get; set;
        }

        public int SplitCopies
        {
            get; set;
        }

        public string SplitUnit
        {
            get; set;
        }

        public string SplitCapacity
        {
            get; set;
        }

        public int SplitPackageCount
        {
            get; set;
        }

        public decimal ValidDays
        {
            get; set;
        }

        public string BatchNo
        {
            get; set;
        }

        public string LogisticsCode
        {
            get; set;
        }

        public string LogisticsContent
        {
            get; set;
        }

        public bool IsClosed
        {
            get; set;
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.SerialNo = reader["serial_no"].ToString();
            this.ParentId = reader["parent_id"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.DispatchedCount = reader.GetInt32(reader.GetOrdinal("dispatched_count"));
            this.HospitalId = reader["hospital_id"].ToString();
            this.VendorId = reader["vendor_id"].ToString();
            this.NeedAudit = reader.GetBoolean(reader.GetOrdinal("need_audit"));
            this.NeedCheck = reader.GetBoolean(reader.GetOrdinal("need_check"));
            this.NeedSplit = reader.GetBoolean(reader.GetOrdinal("need_split"));
            this.SplitCopies = reader.GetInt32(reader.GetOrdinal("split_copies"));
            this.SplitUnit = reader["split_unit"].ToString();
            this.SplitCapacity = reader["split_capacity"].ToString();
            this.SplitPackageCount = reader.GetInt32(reader.GetOrdinal("split_package_count"));
            this.ValidDays = reader.GetDecimal(reader.GetOrdinal("valid_days"));
            if (reader["expired_date"] != DBNull.Value)
            {
                this.ExpiredDate = reader.GetDateTime(reader.GetOrdinal("expired_date"));
            }
            this.BatchNo = reader["batch_no"].ToString();
            this.LogisticsCode = reader["logistics_code"].ToString();
            this.LogisticsContent = reader["logistics_content"].ToString();
            this.IsClosed = reader.GetBoolean(reader.GetOrdinal("is_closed"));
        }
    }
}
