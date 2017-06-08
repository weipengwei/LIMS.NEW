using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class SystemPrivilegeEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string ObjectId
        {
            get; set;
        }

        public int ObjectType
        {
            get; set;
        }

        public string FunKey
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
            this.ObjectId = reader["object_id"].ToString();
            this.ObjectType = reader.GetInt32(reader.GetOrdinal("object_type"));
            this.FunKey = reader["fun_key"].ToString();
            this.Query = reader.GetBoolean(reader.GetOrdinal("query"));
            this.Operate = reader.GetBoolean(reader.GetOrdinal("operate"));
        }
    }
}
