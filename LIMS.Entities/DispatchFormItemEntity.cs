using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class DispatchFormItemEntity
    {
        public string Id
        {
            get; set;
        }

        public string DispatchId
        {
            get; set;
        }

        public string SerialId
        {
            get; set;
        }

        public int Count
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

        public string LogisticsCode
        {
            get; set;
        }

        public string LogisticsContent
        {
            get; set;
        }
        public bool IsConfirmed
        {
            get; set;
        }

        public string ConfirmedId
        {
            get; set;
        }

        public DateTime ConfirmedTime
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
            this.DispatchId = reader["dispatch_id"].ToString();
            this.SerialId = reader["serial_id"].ToString();
            this.Count = reader.GetInt32(reader.GetOrdinal("count"));
            this.BatchNo = reader["batch_no"].ToString();
            this.ExpiredDate = reader.GetDateTime(reader.GetOrdinal("expired_date"));
            this.LogisticsCode = reader["logistics_code"].ToString();
            this.LogisticsContent = reader["logistics_content"].ToString();
            this.IsConfirmed = reader.GetBoolean(reader.GetOrdinal("is_confirmed"));
            this.ConfirmedId = reader["confirmed_id"].ToString();
            if (reader["confirmed_time"] != DBNull.Value)
            {
                this.ConfirmedTime = reader.GetDateTime(reader.GetOrdinal("confirmed_time"));
            }
            if (!reader["created_id"].Equals(DBNull.Value))
            {
                this.CreatedId = reader["created_id"].ToString();
            }

            if (!reader["created_time"].Equals(DBNull.Value))
            {
                this.CreatedTime = (DateTime)reader["created_time"];
            }
        }
    }
}
