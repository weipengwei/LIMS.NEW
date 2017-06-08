using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class CheckFormEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string StoreroomId
        {
            get; set;
        }

        public string HospitalId
        {
            get; set;
        }

        public string Status
        {
            get; set;
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.Name = reader["name"].ToString();
            this.StoreroomId = reader["storeroom_id"].ToString();
            this.HospitalId = reader["hospital_id"].ToString();
            this.Status = reader["status"].ToString();
        }
    }
}
