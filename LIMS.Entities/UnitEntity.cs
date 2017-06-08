using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIMS.Util;

namespace LIMS.Entities
{
    public class UnitEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public string ShortCode
        {
            get; set;
        }

        public string ContactId
        {
            get; set;
        }

        public string DefaultReceiptId
        {
            get; set;
        }

        public string DefaultReceiptTitle
        {
            get; set;
        }

        public UnitType Type
        {
            get; set;
        }

        public string ParentId
        {
            get; set;
        }

        public string RootId
        {
            get; set;
        }
        
        public UnitBusinessType BusinessType
        {
            get; set;
        }

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.Name = reader["name"].ToString();
            this.Description = reader["description"].ToString();
            this.ShortCode = reader["short_code"].ToString();
            this.ContactId = reader["contact_id"].ToString();
            this.Type = (UnitType)(reader.GetInt32(reader.GetOrdinal("type")));
            this.DefaultReceiptId = reader["default_receipt_id"].ToString();
            this.ParentId = reader["parent_id"].ToString();
            this.RootId = reader["root_id"].ToString();
            this.BusinessType = (UnitBusinessType)(reader.GetInt32(reader.GetOrdinal("business_type")));
        }
    }
}
