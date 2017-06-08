using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace LIMS.Util
{
    public class ConfigHelper
    {
        private static ConfigHelper m_Instance;

        static ConfigHelper()
        {
            m_Instance = new ConfigHelper();
        }

        private ConfigHelper()
        {
        }

        public static ConfigHelper Instance
        {
            get
            {
                return m_Instance;
            }
        }

        private bool? m_MergeReceiveIncoming;
        public bool MergeReceiveIncoming
        {
            get
            {
                if (!m_MergeReceiveIncoming.HasValue)
                {
                    var value = ConfigurationManager.AppSettings["MergeReceiveIncoming"];
                    if (!string.IsNullOrEmpty(value) && string.Compare(value, "true", true) == 0)
                    {
                        this.m_MergeReceiveIncoming = true;
                    }
                    else
                    {
                        this.m_MergeReceiveIncoming = false;
                    }
                }

                return this.m_MergeReceiveIncoming.Value;
            }
        }
    }
}
