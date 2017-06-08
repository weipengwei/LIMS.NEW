using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class CheckFormProductValidationEntity
    {
        public string Id
        {
            get; set;
        }

        public string CheckId
        {
            get; set;
        }

        public string ProductId
        {
            get; set;
        }

        public string Barcode
        {
            get; set;
        }

        public string InvStoreroomId
        {
            get; set;
        }

        public bool? InvIsStandard
        {
            get; set;
        }

        public int? InvCount
        {
            get; set;
        }

        public bool? CheckIsStandard
        {
            get; set;
        }

        public int? CheckCount
        {
            get; set;
        }

        public string Status
        {
            get; set;
        }

        public string AdjustType
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
            this.Id = reader["id"].ToString();
            this.CheckId = reader["check_id"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.Barcode = reader["barcode"].ToString();
            this.InvStoreroomId = reader["inv_storeroom_id"].ToString();
            if (reader["inv_is_standard"] != DBNull.Value)
            {
                this.InvIsStandard = reader.GetBoolean(reader.GetOrdinal("inv_is_standard"));
            }
            if (reader["inv_count"] != DBNull.Value)
            {
                this.InvCount = reader.GetInt32(reader.GetOrdinal("inv_count"));
            }
            if (reader["check_is_standard"] != DBNull.Value)
            {
                this.CheckIsStandard = reader.GetBoolean(reader.GetOrdinal("check_is_standard"));
            }
            if (reader["check_count"] != DBNull.Value)
            {
                this.CheckCount = reader.GetInt32(reader.GetOrdinal("check_count"));
            }
            this.Status = reader["status"].ToString();
            this.AdjustType = reader["adjust_type"].ToString();
            this.CreatedId = reader["created_id"].ToString();
            this.CreatedTime = reader.GetDateTime(reader.GetOrdinal("created_time"));
        }
    }
}
