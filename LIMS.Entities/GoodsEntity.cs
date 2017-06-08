using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Util;

namespace LIMS.Entities
{
    public class GoodsEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string Barcode
        {
            get; set;
        }

        public string SerialId
        {
            get; set;
        }

        public string ParentId
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

        public string ProductId
        {
            get; set;
        }

        public string BatchNo
        {
            get; set;
        }

        public DateTime ExpiredDate
        {
            get; set;
        }

        public string StoreroomId
        {
            get; set;
        }

        public string PackageCapacity
        {
            get; set;
        }

        public int PackageCount
        {
            get; set;
        }

        public string MeasuringUnit
        {
            get; set;
        }

        public GoodsStatus Status
        {
            get; set;
        }

        public int GrantedCount
        {
            get; set;
        }

        public string SplitUserId
        {
            get; set;
        }

        public DateTime? SplitDate
        {
            get; set;
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.Name = reader["name"].ToString();
            this.Barcode = reader["barcode"].ToString();
            this.SerialId = reader["serial_id"].ToString();
            this.ParentId = reader["parent_id"].ToString();
            this.HospitalId = reader["hospital_id"].ToString();
            this.VendorId = reader["vendor_id"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.BatchNo = reader["batch_no"].ToString();
            this.ExpiredDate = reader.GetDateTime(reader.GetOrdinal("expired_date"));
            this.StoreroomId = reader["storeroom_id"].ToString();

            if (reader["package_capacity"] != DBNull.Value)
            {
                this.PackageCapacity = reader["package_capacity"].ToString();
            }

            if (reader["package_count"] != DBNull.Value)
            {
                this.PackageCount = reader.GetInt32(reader.GetOrdinal("package_count"));
            }
            
            this.MeasuringUnit = reader["measuring_unit"].ToString();
            
            var status = reader.GetInt32(reader.GetOrdinal("status"));
            if (Enum.IsDefined(typeof(GoodsStatus), status))
            {
                this.Status = (GoodsStatus)status;
            }
            else
            {
                throw new Exception(string.Format("The status of goods({0}) is invalid.", this.Barcode));
            }
            this.GrantedCount = reader.GetInt32(reader.GetOrdinal("granted_count"));

            this.SplitUserId = reader["split_user_id"].ToString();
            if (reader["split_date"] != DBNull.Value)
            {
                this.SplitDate = reader.GetDateTime(reader.GetOrdinal("split_date"));
            }
        }
    }
}
