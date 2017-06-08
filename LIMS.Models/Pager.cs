using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public sealed class PagerInfo
    {
        private const int DEFAULT_PAGE_SIZE = 50;

        public int PageIndex
        {
            get; set;
        }

        private int m_PageSize = DEFAULT_PAGE_SIZE;
        public int PageSize
        {
            get
            {
                return this.m_PageSize;
            }
            set
            {
                this.m_PageSize = value == 0 ? DEFAULT_PAGE_SIZE : value;
            }
        }

        public int PageCount
        {
            get; private set;
        }

        private IList<OrderField> m_OrderFields = new List<OrderField>();
        public IList<OrderField> OrderFields
        {
            get
            {
                return m_OrderFields;
            }
        }

        public void ComputePageCount(int recordCount)
        {
            var pageSize = this.PageSize <= 0 ? DEFAULT_PAGE_SIZE : this.PageSize;

            var count = recordCount / pageSize;
            if(recordCount % pageSize > 0)
            {
                count++;
            }

            this.PageCount = count;
        }
    }

    public sealed class OrderField
    {
        public string Field
        {
            get; set;
        }

        public bool Desc
        {
            get; set;
        }
    }
}
