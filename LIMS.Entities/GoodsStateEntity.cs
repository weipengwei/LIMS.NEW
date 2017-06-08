using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

using LIMS.Util;

namespace LIMS.Entities
{
    public class GoodsStateEntity
    {
        public string Id
        {
            get; set;
        }

        public string Barcode
        {
            get; set;
        }

        public string ProductId
        {
            get; set;
        }

        public string OrderFormId
        {
            get; set;
        }

        public int OrderFormNo
        {
            get; set;
        }

        public string FormId
        {
            get; set;
        }

        public FormType FormType
        {
            get; set;
        }

        public string StateCreatedUser
        {
            get; set;
        }

        public DateTime? StateCreateTime
        {
            get; set;
        }

        public string StateValidateUser
        {
            get; set;
        }

        public DateTime? StateValidateTime
        {
            get; set;
        }

        public string StateChangedUser
        {
            get; set;
        }

        public DateTime? StateChangedTime
        {
            get; set;
        }

        public string FutureFormId
        {
            get; set;
        }

        public FormType FutureFormType
        {
            get; set;
        }

        public string FutureCreatedUser
        {
            get; set;
        }

        public DateTime? FutureCreatedTime
        {
            get; set;
        }

        public bool FutureValid
        {
            get; set;
        }
        
        public string FutureValidateUser
        {
            get; set;
        }

        public DateTime? FutureValidateTime
        {
            get; set;
        }

        public string VendorId
        {
            get; set;
        }

        public void Init(IDataReader reader)
        {
            this.Id = reader["id"].ToString();
            this.Barcode = reader["barcode"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.OrderFormId = reader["order_form_id"].ToString();
            if(reader["order_form_no"] != DBNull.Value)
            {
                this.OrderFormNo = reader.GetInt32(reader.GetOrdinal("order_form_no"));
            }
            this.FormId = reader["form_id"].ToString();
            
            if(reader["form_type"] != DBNull.Value)
            {
                var value = reader.GetInt32(reader.GetOrdinal("form_type"));
                if(Enum.IsDefined(typeof(FormType), value))
                {
                    this.FormType = (FormType)value;
                }
            }

            this.StateCreatedUser = reader["state_created_user"].ToString();

            if (reader["state_created_time"] != DBNull.Value)
            {
                this.StateCreateTime = reader.GetDateTime(reader.GetOrdinal("state_created_time"));
            }

            this.StateValidateUser = reader["state_validate_user"].ToString();

            if (reader["state_validate_time"] != DBNull.Value)
            {
                this.StateValidateTime = reader.GetDateTime(reader.GetOrdinal("state_validate_time"));
            }

            this.StateChangedUser = reader["state_changed_user"].ToString();

            if(reader["state_changed_time"] != DBNull.Value)
            {
                this.StateChangedTime = reader.GetDateTime(reader.GetOrdinal("state_changed_time"));
            }

            this.FutureFormId = reader["future_form_id"].ToString();

            if(reader["future_form_type"] != DBNull.Value)
            {
                var value = reader.GetInt32(reader.GetOrdinal("future_form_type"));
                if(Enum.IsDefined(typeof(FormType), value))
                {
                    this.FutureFormType = (FormType)value;
                }
            }

            this.FutureCreatedUser = reader["future_created_user"].ToString();
            if(reader["future_created_time"] != DBNull.Value)
            {
                this.FutureCreatedTime = reader.GetDateTime(reader.GetOrdinal("future_created_time"));
            }

            this.FutureValid = reader.GetBoolean(reader.GetOrdinal("future_valid"));

            this.FutureValidateUser = reader["future_validate_user"].ToString();
            if (reader["future_validate_time"] != DBNull.Value)
            {
                this.FutureValidateTime = reader.GetDateTime(reader.GetOrdinal("future_validate_time"));
            }

            this.VendorId = reader["vendor_id"].ToString();
        }
    }
}
