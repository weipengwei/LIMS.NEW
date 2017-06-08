using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class CheckFormCategoryEntity
    {
        public string Id
        {
            get; set;
        }

        public string CheckId
        {
            get; set;
        }

        public string Category
        {
            get; set;
        }

        public void Init(IDataReader reader)
        {
            this.Id = reader["id"].ToString();
            this.CheckId = reader["check_id"].ToString();
            this.Category = reader["category"].ToString();
        }
    }
}
