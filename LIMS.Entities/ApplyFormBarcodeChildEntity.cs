using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class ApplyFormBarcodeChildEntity
    {
        public Guid Id
        {
            get; set;
        }

        public string ApplyId
        {
            get; set;
        }

        public string Barcode
        {
            get; set;
        }

        public string ChildBarcode
        {
            get; set;
        }

        public DateTime ExpiredDate
        {
            get; set;
        }

        public bool IsPrinted
        {
            get; set;
        }

        public string CreatedId
        {
            get; set;
        }

        public DateTime CreatedTime
        {
            get; set;
        }

        public void Init(IDataReader reader)
        {
            this.Id = reader.GetGuid(reader.GetOrdinal("id"));
            this.ApplyId = reader["apply_id"].ToString();
            this.Barcode = reader["barcode"].ToString();
            this.ChildBarcode = reader["child_barcode"].ToString();
            this.ExpiredDate = reader.GetDateTime(reader.GetOrdinal("expired_date"));
            this.IsPrinted = reader.GetBoolean(reader.GetOrdinal("is_printed"));
            this.CreatedId = reader["created_id"].ToString();
            this.CreatedTime = reader.GetDateTime(reader.GetOrdinal("created_time"));
        }
    }
}
