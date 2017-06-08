using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Models
{
    public class MenuModel
    {
        public string Id
        {
            get; set;
        }

        public string Title
        {
            get; set;
        }

        public string Url
        {
            get; set;
        }

        public IList<MenuModel> SubMenus
        {
            get; set;
        }
    }
}
