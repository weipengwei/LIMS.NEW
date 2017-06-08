using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace LIMS.Entities
{
    public class IdentityCreatorEntity
    {
        public string IdentityKey
        {
            get; set;
        }

        public string Dimension
        {
            get; set;
        }

        public int Seed
        {
            get; set;
        }

        public int Step
        {
            get; set;
        }

        public void Init(IDataReader reader)
        {
            this.IdentityKey = reader["identity_key"].ToString();
            this.Dimension = reader["dimension"].ToString();
            this.Seed = reader.GetInt32(reader.GetOrdinal("seed"));
            this.Step = reader.GetInt32(reader.GetOrdinal("step"));
        }
    }
}
