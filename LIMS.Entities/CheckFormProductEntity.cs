using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class CheckFormProductEntity
    {
        public string Id
        {
            get; set;
        }

        public string CheckId
        {
            get; set;
        }

        public string Category
        {
            get; set;
        }

        public string ProductId
        {
            get; set;
        }

        public int InvStandardCount
        {
            get; set;
        }

        public int InvUsingPackageCount
        {
            get; set;
        }

        public int? CheckStandardCount
        {
            get; set;
        }

        public int? CheckUsingPackageCount
        {
            get; set;
        }

        public string Status
        {
            get; set;
        }

        public string OperatorId
        {
            get; set;
        }

        public DateTime OperatedTime
        {
            get; set;
        }

        public bool CheckOver
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
            this.Category = reader["category"].ToString();
            if (reader["inv_standard_count"] != DBNull.Value)
            {
                this.InvStandardCount = reader.GetInt32(reader.GetOrdinal("inv_standard_count"));
            }
            if (reader["inv_using_package_count"] != DBNull.Value)
            {
                this.InvUsingPackageCount = reader.GetInt32(reader.GetOrdinal("inv_using_package_count"));
            }
            if (reader["check_standard_count"] != DBNull.Value)
            {
                this.CheckStandardCount = reader.GetInt32(reader.GetOrdinal("check_standard_count"));
            }
            if (reader["check_using_package_count"] != DBNull.Value)
            {
                this.CheckUsingPackageCount = reader.GetInt32(reader.GetOrdinal("check_using_package_count"));
            }
            this.Status = reader["status"].ToString();
            this.OperatorId = reader["operator_id"].ToString();
            if (reader["operated_time"] != DBNull.Value)
            {
                this.OperatedTime = reader.GetDateTime(reader.GetOrdinal("operated_time"));
            }
            this.CheckOver = reader.GetBoolean(reader.GetOrdinal("check_over"));
            this.CreatedId = reader["created_id"].ToString();
            this.CreatedTime = reader.GetDateTime(reader.GetOrdinal("created_time"));
        }
    }
}
