using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class UserEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string Account
        {
            get; set;
        }

        public string Password
        {
            get; set;
        }

        public string WeiXinId
        {
            get; set;
        }

        public string UnitId
        {
            get; set;
        }

        public bool IsChangePassword
        {
            get; set;
        }

        public int Title
        {
            get; set;
        }
        
        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.Name = reader["name"].ToString();
            this.Account = reader["account"].ToString();
            this.Password = reader["password"].ToString();
            this.WeiXinId = reader["weixing_id"].ToString();
            this.UnitId = reader["unit_id"].ToString();
            this.IsChangePassword = reader.GetBoolean(reader.GetOrdinal("is_change_pwd"));
            this.Title = reader.GetInt32(reader.GetOrdinal("title"));
        }
    }
}
