using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class MoveFormEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string HospitalId
        {
            get; set;
        }

        public int FormNo
        {
            get; set;
        }

        public string ToStoreroomId
        {
            get; set;
        }

        public string ToStoreroomName
        { get; set; }

        public string FromStoreroomId
        {
            get; set;
        }

        public string FromStoreroomName
        { get; set; }

        public string RequestedBy
        { get; set; }

        public string RequestedByName
        { get; set; }

        public DateTime RequestedTime
        { get; set; }

        public string Status
        { get; set; }

        public string StatusName
        { get; set; }

        public string MoveoutBy
        { get; set; }

        public string MoveoutByName
        { get; set; }

        public DateTime? MoveoutTime
        { get; set; }

        public string MoveinBy
        { get; set; }

        public string MoveinByName
        { get; set; }

        public DateTime? MoveinTime
        { get; set; }

        public List<MoveFormItemEntity> Items
        { get; set; }

        public bool IsReadOnly
        { get; set; }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.FormNo = reader.GetInt32(reader.GetOrdinal("form_no"));
            this.HospitalId = reader["hospital_id"].ToString();
            this.RequestedBy = reader["requested_by"].ToString();
            this.RequestedTime = reader.GetDateTime(reader.GetOrdinal("requested_time"));
            this.FromStoreroomId = reader["from_storeroom"].ToString();
            this.ToStoreroomId = reader["to_storeroom"].ToString();
            this.Status = reader["status"].ToString();

            if (!reader["moveout_by"].Equals(DBNull.Value))
                this.MoveoutBy = reader["moveout_by"].ToString();

            if (!reader["moveout_time"].Equals(DBNull.Value))
                this.MoveoutTime = (DateTime)reader["moveout_time"];

            if (!reader["movein_by"].Equals(DBNull.Value))
                this.MoveinBy = reader["movein_by"].ToString();

            if (!reader["movein_time"].Equals(DBNull.Value))
                this.MoveinTime = (DateTime)reader["movein_time"];
        }
    }
}
