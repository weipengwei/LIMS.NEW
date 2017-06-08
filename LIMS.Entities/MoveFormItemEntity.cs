using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class MoveFormItemEntity
    {
        public string Id
        { get; set; }

        public string MoveId
        { get; set; }

        public int FormNo
        { get; set; }

        public string ProductId
        { get; set; }

        public string ProductName
        { get; set; }

        public int Count
        { get; set; }

        public int MovableCount
        { get; set; }

        public void Init(IDataReader reader)
        {
            this.Id = reader["id"].ToString();
            this.MoveId = reader["move_id"].ToString();
            this.FormNo = reader.GetInt32(reader.GetOrdinal("form_no"));
            this.ProductId = reader["product_id"].ToString();
            this.Count = reader.GetInt32(reader.GetOrdinal("count"));
        }
    }
}
