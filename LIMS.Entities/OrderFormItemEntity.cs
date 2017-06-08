using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIMS.Util;

namespace LIMS.Entities
{
    public class OrderFormItemEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string OrderId
        {
            get; set;
        }

        public int FormNo
        {
            get; set;
        }

        public string HospitalId
        {
            get; set;
        }

        public string ProductId
        {
            get; set;
        }

        public string OrderPerson
        {
            get; set;
        }

        public int Count
        {
            get; set;
        }

        public int DonateCount
        {
            get; set;
        }

        public decimal Price
        {
            get; set;
        }

        public decimal ExpectPrice
        {
            get; set;
        }

        public decimal Sum
        {
            get; set;
        }

        public DateTime? ExpiredDate
        {
            get; set;
        }

        public DateTime? ExpectDate
        {
            get; set;
        }

        public string Contact
        {
            get; set;
        }
        
        public string Status
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
        
        public override void Init(IDataReader reader)
        {
            this.Id = reader["id"].ToString();
            this.OrderId = reader["order_id"].ToString();
            this.FormNo = reader.GetInt32(reader.GetOrdinal("form_no"));
            this.HospitalId = reader["hospital_id"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.OrderPerson = reader["order_person"].ToString();
            this.Count = reader.GetInt32(reader.GetOrdinal("count"));
            this.DonateCount = reader.GetInt32(reader.GetOrdinal("donate_count"));
            this.Price = reader.GetDecimal(reader.GetOrdinal("price"));
            if (reader["expect_price"] != DBNull.Value)
            {
                this.ExpectPrice = reader.GetDecimal(reader.GetOrdinal("expect_price"));
            }
            this.Sum = reader.GetDecimal(reader.GetOrdinal("sum"));            
            if (reader["expired_date"] != DBNull.Value)
            {
                this.ExpiredDate = reader.GetDateTime(reader.GetOrdinal("expired_date"));
            }
            if (reader["expect_date"] != DBNull.Value)
            {
                this.ExpectDate = reader.GetDateTime(reader.GetOrdinal("expect_date"));
            }
            this.Contact = reader["contact"].ToString();
            this.Status = reader["status"].ToString();
            this.NeedAudit = reader.GetBoolean(reader.GetOrdinal("need_audit"));
            this.NeedCheck = reader.GetBoolean(reader.GetOrdinal("need_check"));
            this.NeedSplit = reader.GetBoolean(reader.GetOrdinal("need_split"));
            this.SplitCopies = reader.GetInt32(reader.GetOrdinal("split_copies"));
            this.SplitUnit = reader["split_unit"].ToString();
            this.SplitCapacity = reader["split_capacity"].ToString();
            this.SplitPackageCount = reader.GetInt32(reader.GetOrdinal("split_package_count"));
            this.ValidDays = reader.GetDecimal(reader.GetOrdinal("valid_days"));
        }
    }
}
