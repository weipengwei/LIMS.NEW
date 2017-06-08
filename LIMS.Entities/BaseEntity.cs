using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            
        }

        public string CreatedId
        {
            get; set;
        }

        private DateTime m_CreatedTime = DateTime.Now;
        public DateTime CreatedTime
        {
            get
            {
                return this.m_CreatedTime;
            }
            set
            {
                this.m_CreatedTime = value;
            }
        }

        public string UpdatedId
        {
            get; set;
        }

        private DateTime m_UpdatedTime = DateTime.Now;
        public DateTime UpdatedTime
        {
            get
            {
                return this.m_UpdatedTime;
            }
            set
            {
                this.m_UpdatedTime = value;
            }
        }

        public virtual void Init(IDataReader reader)
        {
            if (!reader["created_id"].Equals(DBNull.Value))
            {
                this.CreatedId = reader["created_id"].ToString();
            }

            if (!reader["created_time"].Equals(DBNull.Value))
            {
                this.CreatedTime = (DateTime)reader["created_time"];
            }

            if (!reader["updated_id"].Equals(DBNull.Value))
            {
                this.UpdatedId = reader["updated_id"].ToString();
            }

            if (!reader["updated_time"].Equals(DBNull.Value))
            {
                this.UpdatedTime = (DateTime)reader["updated_time"];
            }
        }

        protected bool IsNotNull(object value)
        {
            return value != DBNull.Value;
        }
    }
}
