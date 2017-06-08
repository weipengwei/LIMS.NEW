using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class ContactInfoEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string VesteeId
        {
            get; set;
        }

        public string ContactPerson
        {
            get; set;
        }

        public string Address
        {
            get; set;
        }

        public string ContactWay1
        {
            get; set;
        }

        public string ContactWay2
        {
            get; set;
        }

        public string ContactWay3
        {
            get; set;
        }

        public string ContactWay4
        {
            get; set;
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.VesteeId = reader["vestee_id"].ToString();
            this.ContactPerson = reader["contact_person"].ToString();
            this.Address = reader["address"].ToString();
            this.ContactWay1 = reader["contact_way_1"].ToString();
            this.ContactWay2 = reader["contact_way_2"].ToString();
            this.ContactWay3 = reader["contact_way_3"].ToString();
            this.ContactWay4 = reader["contact_way_4"].ToString();
        }
    }
}
