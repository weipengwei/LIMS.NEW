using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class ReceiveFormItemEntity
    {
        public string Id
        {
            get; set;
        }

        public string ReceiveId
        {
            get; set;
        }

        public string SerialId
        {
            get; set;
        }

        public int ReceivedCount
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

        public void Init(IDataReader reader)
        {
            this.Id = reader["id"].ToString();
            this.ReceiveId = reader["receive_id"].ToString();
            this.SerialId = reader["serial_id"].ToString();
            this.ReceivedCount = reader.GetInt32(reader.GetOrdinal("received_count"));
            this.ConfirmedId = reader["confirmed_id"].ToString();
            this.ConfirmedTime = reader.GetDateTime(reader.GetOrdinal("confirmed_time"));
        }
    }
}
