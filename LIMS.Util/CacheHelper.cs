using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace LIMS.Util
{
    public static class CacheHelper
    {
        private static object ms_Lock = new object();

        public static T Get<T>(string key, Func<T> load) where T : class
        {
            return Get<T>(key, new TimeSpan(24, 0, 0), load);
        }

        public static T Get<T>(string key, TimeSpan expiredTime, Func<T> load) where T : class
        {
            var data = HttpRuntime.Cache.Get(key) as T;
            if(data == null)
            {
                lock (ms_Lock)
                {
                    data = HttpRuntime.Cache.Get(key) as T;
                    if(data == null)
                    {
                        data = load();
                        HttpRuntime.Cache.Insert(key, data, null, Cache.NoAbsoluteExpiration, expiredTime);
                    }
                }
            }

            return data;
        }
    }
}
