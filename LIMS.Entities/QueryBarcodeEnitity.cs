using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using LIMS.Util;

namespace LIMS.Entities
{
    public class QueryBarcodeEntity
    {

        public string Id
        {
            get; set;
        }

        public string Barcode
        {
            get; set;
        }

        public int OrderFormNo
        {
            get; set;
        }
        public string OrderPersonID
        {
            get; set;
        }
        public string OrderPersonName
        {
            get; set;
        }
        public DateTime OrderTime
        {
            get; set;
        }

        public string HospitalId
        {
            get; set;
        }

        public string HospitalName
        {
            get; set;
        }

        public string VendorID
        {
            get; set;
        }

        public string VendorName
        {
            get; set;
        }
        public string ProductId
        {
            get; set;
        }

        public string ProductName
        {
            get; set;
        }

        public int Status
        {
            get; set;
        }

        public string StatusName
        {
            get
            {
                return BarcodeStatusName.GetName((FormType)this.Status);
            }
        }

        public IList<BarcodeStatusEntity> StatusDetails
        {
            get; set;
        }

        public void Init(IDataReader reader)
        {
            this.Id = reader["id"].ToString();
            this.Barcode= reader["barcode"].ToString();
            this.HospitalId = reader["hospital_id"].ToString();
            this.HospitalName = reader["hospital_name"].ToString();
            this.OrderFormNo = reader.GetInt32(reader.GetOrdinal("order_form_no"));
            this.OrderPersonID = reader.GetOrdinal("order_person_id").ToString();
            this.OrderPersonName = reader.GetOrdinal("order_person_name").ToString();
            this.OrderTime = reader.GetDateTime(reader.GetOrdinal("apply_time")); 
            this.ProductId = reader["product_id"].ToString();
            this.ProductName = reader["product_name"].ToString();
            this.Status = reader.GetInt32(reader.GetOrdinal("status"));
            this.VendorID = reader["vendor_id"].ToString();
            this.VendorName = reader["vendor_name"].ToString();
        }
    }

    public class BarcodeStatusEntity
    {
        public string Id
        {
            get; set;
        }

        public string FormTypeName
        {
            get
            {
                return BarcodeStatusName.GetName((FormType)this.FormTypeID);
            }
        }

        public string OperatorID
        {
            get; set;
        }

        public string OperatorName
        {
            get; set;
        }

        public DateTime StatusTime
        {
            get; set;
        }

        public string FormID
        {
            get; set;
        }

        public int FormTypeID
        {
            get; set;
        }

        public void Init(IDataReader reader)
        {
            //this.Id = reader["id"].ToString();
            this.FormID = reader["form_no"].ToString()=="0"?"": reader["form_no"].ToString();
            this.FormTypeID = reader.GetInt32(reader.GetOrdinal("form_type_id"));
            this.OperatorID = reader["operator_id"].ToString();
            this.OperatorName = reader["operator_name"].ToString();
            this.StatusTime = reader.GetDateTime(reader.GetOrdinal("status_time"));
        }
    }
}
