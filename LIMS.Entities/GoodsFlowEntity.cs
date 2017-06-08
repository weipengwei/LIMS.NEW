using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class GoodsFlowEntity
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

        public int OrderFormNo
        {
            get; set;
        }

        public string DispatchForm
        {
            get; set;
        }

        public bool DispatchValid
        {
            get; set;
        }

        public DateTime? DispatchTime
        {
            get; set;
        }

        public string DispatchUser
        {
            get; set;
        }
        
        public string ReceiveForm
        {
            get; set;
        }

        public bool ReceiveValid
        {
            get; set;
        }

        public DateTime? ReceiveTime
        {
            get; set;
        }

        public string ReceiveUser
        {
            get; set;
        }

        public string InspectionForm
        {
            get; set;
        }

        public bool InspectionValid
        {
            get; set;
        }

        public DateTime? InspectionTime
        {
            get; set;
        }

        public string InspectionUser
        {
            get; set;
        }

        public string IncomingForm
        {
            get; set;
        }

        public bool IncomingValid
        {
            get; set;
        }

        public DateTime? IncomingTime
        {
            get; set;
        }

        public string IncomingUser
        {
            get; set;
        }

        public string ReturnForm
        {
            get; set;
        }

        public bool ReturnValid
        {
            get; set;
        }

        public DateTime? ReturnTime
        {
            get; set;
        }

        public string ReturnUser
        {
            get; set;
        }


        public virtual void Init(IDataReader reader)
        {
            this.Id = reader["id"].ToString();
            this.Barcode = reader["barcode"].ToString();
            this.ProductId = reader["product_id"].ToString();
            this.OrderFormNo = reader.GetInt32(reader.GetOrdinal("order_form_no"));
            this.DispatchForm = reader["dispatch_form"].ToString();
            this.DispatchValid = reader.GetBoolean(reader.GetOrdinal("dispatch_valid"));
            if(reader["dispatch_time"] != DBNull.Value)
            {
                this.DispatchTime = reader.GetDateTime(reader.GetOrdinal("dispatch_time"));
            }
            this.DispatchUser = reader["dispatch_user"].ToString();

            if(reader["receive_form"] != DBNull.Value)
            {
                this.ReceiveForm = reader["receive_form"].ToString();
            }
            if (reader["receive_valid"] != DBNull.Value)
            {
                this.ReceiveValid = reader.GetBoolean(reader.GetOrdinal("receive_valid"));
            }
            if (reader["receive_time"] != DBNull.Value)
            {
                this.ReceiveTime = reader.GetDateTime(reader.GetOrdinal("receive_time"));
            }
            if (reader["receive_user"] != DBNull.Value)
            {
                this.ReceiveUser = reader["receive_user"].ToString();
            }

            if (reader["inspection_form"] != DBNull.Value)
            {
                this.InspectionForm = reader["inspection_form"].ToString();
            }
            if (reader["inspection_valid"] != DBNull.Value)
            {
                this.InspectionValid = reader.GetBoolean(reader.GetOrdinal("inspection_valid"));
            }
            if (reader["inspection_time"] != DBNull.Value)
            {
                this.InspectionTime = reader.GetDateTime(reader.GetOrdinal("inspection_time"));
            }
            if (reader["inspection_user"] != DBNull.Value)
            {
                this.InspectionUser = reader["inspection_user"].ToString();
            }

            if (reader["incoming_form"] != DBNull.Value)
            {
                this.IncomingForm = reader["incoming_form"].ToString();
            }
            if (reader["incoming_valid"] != DBNull.Value)
            {
                this.IncomingValid = reader.GetBoolean(reader.GetOrdinal("incoming_valid"));
            }
            if (reader["incoming_time"] != DBNull.Value)
            {
                this.IncomingTime = reader.GetDateTime(reader.GetOrdinal("incoming_time"));
            }
            if (reader["incoming_user"] != DBNull.Value)
            {
                this.IncomingUser = reader["incoming_user"].ToString();
            }

            if (reader["return_form"] != DBNull.Value)
            {
                this.ReturnForm = reader["return_form"].ToString();
            }
            if (reader["return_valid"] != DBNull.Value)
            {
                this.ReturnValid = reader.GetBoolean(reader.GetOrdinal("return_valid"));
            }
            if (reader["return_time"] != DBNull.Value)
            {
                this.ReturnTime = reader.GetDateTime(reader.GetOrdinal("return_time"));
            }
            if (reader["return_user"] != DBNull.Value)
            {
                this.ReturnUser = reader["return_user"].ToString();
            }
        }
    }
}
