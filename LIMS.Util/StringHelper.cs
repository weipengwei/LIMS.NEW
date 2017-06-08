using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Util
{
    public sealed class StringHelper
    {
        public static string Barcode(int seed)
        {
            return (10000000000000 + seed).ToString().Substring(1);
        }
        
        public static void GenerInParameters(string namePrefix, int count, out IList<string> paramNames, out string paramSql)
        {
            paramNames = new List<string>();
            var temp = new StringBuilder();

            for (var i = 0; i < count; i++)
            {
                temp.Append("@");
                temp.Append(namePrefix);
                temp.Append(i);
                temp.Append(",");

                paramNames.Add(namePrefix + i.ToString());
            }

            paramSql = temp.ToString().TrimEnd(',');
        }
    }
}
