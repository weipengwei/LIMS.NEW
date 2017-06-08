using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class GoodsSerialBarcodeEntity
    {
        public string Id
        {
            get; set;
        }

        public string SerialId
        {
            get; set;
        }

        public string Barcode
        {
            get; set;
        }

        public bool IsPrinted
        {
            get; set;
        }

        public bool Out
        {
            get; set;
        }

        public void Init(IDataReader reader)
        {
            this.Id = reader["id"].ToString();
            this.SerialId = reader["serial_id"].ToString();
            this.Barcode = reader["barcode"].ToString();
            this.IsPrinted = reader.GetBoolean(reader.GetOrdinal("is_printed"));
            this.Out = reader.GetBoolean(reader.GetOrdinal("out"));
        }
    }
}
