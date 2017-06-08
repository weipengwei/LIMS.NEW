using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class UserPrivilegeEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string UserId
        {
            get; set;
        }

        public string UnitRootId
        {
            get; set;
        }

        public string UnitId
        {
            get; set;
        }

        public bool Query
        {
            get; set;
        }

        public bool Operate
        {
            get; set;
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.UserId = reader["user_id"].ToString();
            this.UnitRootId = reader["unit_root_id"].ToString();
            this.UnitId = reader["unit_id"].ToString();
            this.Query = reader.GetBoolean(reader.GetOrdinal("query"));
            this.Operate = reader.GetBoolean(reader.GetOrdinal("operate"));
        }
    }
}
