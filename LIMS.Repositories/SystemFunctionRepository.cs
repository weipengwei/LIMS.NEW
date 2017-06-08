using LIMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;

using LIMS.Util;

namespace LIMS.Repositories
{
    public static class SystemFunctionRepository
    {
        public static IList<SystemFunctionEntity> GetAll()
        {
            var list = new List<SystemFunctionEntity>();

            var sql = @"SELECT * FROM system_functions WHERE is_active=1 ORDER BY sequence";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    var entity = new SystemFunctionEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<SystemFunctionEntity> GetUserFunctions(string unitRootId, string userId)
        {
            var sql = @"
select fun.id, fun.title, fun.is_menu, fun.fun_key, fun.url, fun.parent_id, fun.is_unit, fun.is_active, fun.sequence, fun.display_mode,  
		fun.created_id, fun.created_time, fun.updated_id, fun.updated_time 
from system_functions fun
join (select distinct b.fun_key from user_privilege a
join system_privilege b on a.unit_id = b.object_id and b.object_type=0 and b.operate=1
where a.unit_root_id=@p_unit_root_id and a.user_id=@p_user_id and a.operate=1) privilege
on fun.fun_key = privilege.fun_key and fun.is_active=1 order by sequence";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_unit_root_id", DbType.String, unitRootId);
            db.AddInParameter(dc, "p_user_id", DbType.String, userId);

            var funs = new Dictionary<string, IList<SystemFunctionEntity>>();
            using (var reader = db.ExecuteReader(dc))
            {
                IList<SystemFunctionEntity> list;
                while(reader.Read())
                {
                    var fun = new SystemFunctionEntity();
                    fun.Init(reader);

                    if(funs.TryGetValue(fun.ParentId, out list))
                    {
                        list.Add(fun);
                    }
                    else
                    {
                        list = new List<SystemFunctionEntity>();
                        list.Add(fun);

                        funs[fun.ParentId] = list;
                    }
                }
            }
            
            if(funs.Count == 0)
            {
                return new List<SystemFunctionEntity>();
            }

            var mainFuns = Query(funs.Keys.ToList());
            foreach(var item in mainFuns)
            {
                var subs = funs[item.Id];
                foreach(var sub in subs)
                {
                    item.SubFunctions.Add(sub);
                }
            }

            return mainFuns;
        }

        public static IList<SystemFunctionEntity> Query(IList<string> ids)
        {
            var sql = string.Format("select * from system_functions where id in ('{0}') order by sequence", string.Join("','", ids));

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            var list = new List<SystemFunctionEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    var fun = new SystemFunctionEntity();
                    fun.Init(reader);

                    list.Add(fun);
                }
            }

            return list;
        }

        public static SystemFunctionEntity GetSettingFunction()
        {
            var sql = "select * from system_functions where id = @id or parent_id = @id order by sequence";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "id", DbType.String, Constant.ADMIN_MENU_ID);

            var list = new List<SystemFunctionEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var fun = new SystemFunctionEntity();
                    fun.Init(reader);

                    list.Add(fun);
                }
            }

            var setting = list.FirstOrDefault(item => string.Compare(item.Id, Constant.ADMIN_MENU_ID, true) == 0);
            if(setting == null)
            {
                throw new Exception("The functions of admin is empty.");
            }

            setting.SubFunctions = list.Where(item => string.Compare(item.ParentId, Constant.ADMIN_MENU_ID, true) == 0).ToList();

            return setting;
        }
    }
}
