using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class HospitalProductEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string HospitalId
        {
            get; set;
        }

        public string UnitId
        {
            get; set;
        }

        public string ProductId
        {
            get; set;
        }

        public string Category
        {
            get; set;
        }

        public string Alias
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

        public int MiniSplitNumber
        {
            get; set;
        }

        public int GrantUnitCount
        {
            get;set;
        }

        public int OrderUnitCount
        {
            get; set;
        }

        public int DonateCount
        {
            get; set;
        }

        public int DonateBase
        {
            get; set;
        }

        public decimal ValidDays
        {
            get; set;
        }

        public int ArrivalDays
        {
            get; set;
        }

        public decimal Price
        {
            get; set;
        }

        public decimal PackagePrice
        {
            get; set;
        }

        public string ContactId
        {
            get; set;
        }

        public bool IsActive
        {
            get; set;
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.HospitalId = reader["hospital_id"].ToString();
            this.UnitId = reader["unit_id"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.Alias = reader["alias"].ToString();
            this.Category = reader["category"].ToString();
            this.NeedCheck = reader.GetBoolean(reader.GetOrdinal("need_check"));
            this.NeedSplit = reader.GetBoolean(reader.GetOrdinal("need_split"));

            if (this.IsNotNull(reader["split_copies"]))
            {
                this.SplitCopies = reader.GetInt32(reader.GetOrdinal("split_copies"));
            }

            this.SplitUnit = reader["split_unit"].ToString();
            this.SplitCapacity = reader["split_capacity"].ToString();

            if (this.IsNotNull(reader["mini_split_number"]))
            {
                this.MiniSplitNumber = reader.GetInt32(reader.GetOrdinal("mini_split_number"));
            }

            if (this.IsNotNull(reader["donate_count"]))
            {
                this.DonateCount = reader.GetInt32(reader.GetOrdinal("donate_count"));
            }

            if (this.IsNotNull(reader["donate_base"]))
            {
                this.DonateBase = reader.GetInt32(reader.GetOrdinal("donate_base"));
            }

            if (this.IsNotNull(reader["valid_days"]))
            {
                this.ValidDays = reader.GetDecimal(reader.GetOrdinal("valid_days"));
            }

            if (this.IsNotNull(reader["arrival_days"]))
            {
                this.ArrivalDays = reader.GetInt32(reader.GetOrdinal("arrival_days"));
            }

            if (this.IsNotNull(reader["price"]))
            {
                this.Price = reader.GetDecimal(reader.GetOrdinal("price"));
            }

            if (this.IsNotNull(reader["package_price"]))
            {
                this.PackagePrice = reader.GetDecimal(reader.GetOrdinal("package_price"));
            }

            if (this.IsNotNull(reader["grant_unit_count"]))
            {
                this.GrantUnitCount = reader.GetInt32(reader.GetOrdinal("grant_unit_count"));
            }
            else
                this.GrantUnitCount = 1;

            if (this.IsNotNull(reader["order_unit_count"]))
            {
                this.OrderUnitCount = reader.GetInt32(reader.GetOrdinal("order_unit_count"));
            }
            else
                this.OrderUnitCount = 1;

            this.ContactId = reader["contact_id"].ToString();
            this.IsActive = reader.GetBoolean(reader.GetOrdinal("is_active"));
        }
    }
}
