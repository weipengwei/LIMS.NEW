using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace LIMS.Entities
{
    public class GoodsInventoryEntity
    {
        public string Id
        {
            get; set;
        }

        public string SerialId
        {
            get;set;
        }

        public string BatchNo
        {
            get;set;
        }

        public string ProductId
        {
            get; set;
        }

        public string StoreroomId
        {
            get;set;
        }

        public DateTime ExpiredDate
        {
            get;set;
        }

        public string HospitalId
        {
            get; set;
        }

        public string VendorId
        {
            get;set;
        }

        public int OriginalCount
        {
            get; set;
        }

        public int SplitCount
        {
            get;set;
        }

        public int UsableCount
        {
            get; set;
        }

        public int ApplyCount
        {
            get; set;
        }

        public int GrantedCount
        {
            get; set;
        }

        public string CreatedId
        {
            get;set;
        }

        public DateTime CreatedTime
        {
            get;set;
        }

        public void Init(IDataReader reader)
        {
            this.Id = reader["id"].ToString();
            this.SerialId = reader["serial_id"].ToString();
            this.BatchNo = reader["batch_no"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.StoreroomId = reader["storeroom_id"].ToString();
            this.ExpiredDate = reader.GetDateTime(reader.GetOrdinal("expired_date"));
            this.HospitalId = reader["hospital_id"].ToString();
            this.VendorId = reader["vendor_id"].ToString();
            this.OriginalCount = reader.GetInt32(reader.GetOrdinal("original_count"));
            this.SplitCount = reader.GetInt32(reader.GetOrdinal("split_count"));
            this.UsableCount = reader.GetInt32(reader.GetOrdinal("usable_count"));
            this.ApplyCount = reader.GetInt32(reader.GetOrdinal("apply_count"));
            this.GrantedCount = reader.GetInt32(reader.GetOrdinal("granted_count"));
            this.CreatedId = reader["created_id"].ToString();
            this.CreatedTime = reader.GetDateTime(reader.GetOrdinal("created_time"));
        }
    }
}
