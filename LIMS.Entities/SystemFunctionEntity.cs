using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using LIMS.Util;

namespace LIMS.Entities
{

    public class SystemFunctionEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string Title
        {
            get; set;
        }

        public bool IsMenu
        {
            get; set;
        }

        public string FunKey
        {
            get; set;
        }

        public string Url
        {
            get; set;
        }

        public string ParentId
        {
            get; set;
        }

        public bool IsActive
        {
            get; set;
        }

        public int Sequence
        {
            get; set;
        }

        public DisplayModeType DisplayMode
        {
            get; set;
        }

        private IList<SystemFunctionEntity> m_SubFunctions = new List<SystemFunctionEntity>();
        public IList<SystemFunctionEntity> SubFunctions
        {
            get
            {
                return this.m_SubFunctions;
            }
            set
            {
                this.m_SubFunctions = value;
            }
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.Title = reader["title"].ToString();
            this.IsMenu = reader.GetBoolean(reader.GetOrdinal("is_menu"));
            this.FunKey = reader["fun_key"].ToString();
            this.Url = reader["url"].ToString();
            this.ParentId = reader["parent_id"].ToString();
            this.IsActive = reader.GetBoolean(reader.GetOrdinal("is_active"));
            this.Sequence = reader.GetInt32(reader.GetOrdinal("sequence"));
        }
    }
}
