using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class GoodsExtraEntity
    {
        public string Id
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

        public string SplitCapacity
        {
            get; set;
        }

        public int MiniSplitNumber
        {
            get; set;
        }

        public decimal ValidDays
        {
            get; set;
        }

        public DateTime? ExpiredDate
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

        public void Init(IDataReader reader)
        {
            this.Id = reader["id"].ToString();
            this.NeedCheck = reader.GetBoolean(reader.GetOrdinal("need_check"));
            this.NeedSplit = reader.GetBoolean(reader.GetOrdinal("need_split"));
            this.SplitCapacity = reader["split_capacity"].ToString();
            this.MiniSplitNumber = reader.GetInt32(reader.GetOrdinal("mini_split_number"));
            this.ValidDays = reader.GetDecimal(reader.GetOrdinal("valid_days"));

            if (reader["expired_date"] != DBNull.Value)
            {
                this.ExpiredDate = reader.GetDateTime(reader.GetOrdinal("expired_date"));
            }

            if (reader["split_copies"] != DBNull.Value)
            {
                this.SplitCopies = reader.GetInt32(reader.GetOrdinal("split_copies"));
            }

            this.SplitUnit = reader["split_unit"].ToString();
        }
    }
}
