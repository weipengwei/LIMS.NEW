using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using LIMS.Entities;
using LIMS.Models;

namespace LIMS.Repositories
{
    public static class UserRepository
    {
        private static string COLUMN_SQL = @"id, name, account, password, weixing_id, unit_id, is_change_pwd, title,
created_id, created_time, updated_id, updated_time";

        public static bool TryGetByAccount(string account, out UserEntity entity)
        {
            entity = null;

            var sql = string.Format(@"select {0} from users where account=@p_account", COLUMN_SQL);

            Database db = DatabaseFactory.CreateDatabase();
            DbCommand dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_account", DbType.String, account);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                var read = false;
                while (reader.Read())
                {
                    if (read)
                    {
                        throw new Exception("The account is repeated in users.");
                    }
                    read = true;

                    entity = new UserEntity();
                    entity.Init(reader);
                }

                if (!read)
                {
                    return false;
                }
            }

            return true;
        }

        public static UserEntity GetByAccount(string account, string userId)
        {
            var sql = string.Format(@"select {0} from users where account=@p_account", COLUMN_SQL);

            if (!string.IsNullOrEmpty(userId))
            {
                sql += " and id<>@p_user_id";
            }

            Database db = DatabaseFactory.CreateDatabase();
            DbCommand dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_account", DbType.String, account);
            if (!string.IsNullOrEmpty(userId))
            {
                db.AddInParameter(dc, "p_user_id", DbType.String, userId);
            }

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var entity = new UserEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static UserEntity Get(string id)
        {
            var sql = string.Format(@"select {0} from users where id=@p_id", COLUMN_SQL);

            Database db = DatabaseFactory.CreateDatabase();
            DbCommand dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var user = new UserEntity();
                    user.Init(reader);

                    return user;
                }
            }

            return null;
        }

        public static void Add(UserEntity user)
        {
            var sql = string.Format(@"INSERT INTO users({0})
VALUES(@p_id, @p_name, @p_account, @p_password, @p_weixing_id, @p_unit_id, @p_is_change_pwd, @p_title,
@p_created_id, @p_created_time, @p_updated_id, @p_updated_time)", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, user.Id);
            db.AddInParameter(dc, "p_name", DbType.String, user.Name);
            db.AddInParameter(dc, "p_account", DbType.String, user.Account);
            db.AddInParameter(dc, "p_password", DbType.String, user.Password);
            db.AddInParameter(dc, "p_weixing_id", DbType.String, user.WeiXinId);
            db.AddInParameter(dc, "p_unit_id", DbType.String, user.UnitId);
            db.AddInParameter(dc, "p_is_change_pwd", DbType.Boolean, user.IsChangePassword);
            db.AddInParameter(dc, "p_title", DbType.Int32, user.Title);
            db.AddInParameter(dc, "p_created_id", DbType.String, user.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, user.CreatedTime);
            db.AddInParameter(dc, "p_updated_id", DbType.String, user.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, user.UpdatedTime);

            db.ExecuteNonQuery(dc);
        }

        public static void Update(UserEntity user)
        {
            var sql = string.Format(@"UPDATE users
SET name = @p_name, account = @p_account, {1}
weixing_id = @p_weixing_id, unit_id = @p_unit_id, title=@p_title,
updated_id = @p_updated_id, updated_time = @p_updated_time
WHERE id = @p_id", COLUMN_SQL, string.IsNullOrEmpty(user.Password) ? string.Empty : "password = @p_password,");

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_name", DbType.String, user.Name);
            db.AddInParameter(dc, "p_account", DbType.String, user.Account);
            if(!string.IsNullOrEmpty(user.Password))
            {
                db.AddInParameter(dc, "p_password", DbType.String, user.Password);
            }
            db.AddInParameter(dc, "p_weixing_id", DbType.String, user.WeiXinId);
            db.AddInParameter(dc, "p_unit_id", DbType.String, user.UnitId);
            db.AddInParameter(dc, "p_title", DbType.Int32, user.Title);
            db.AddInParameter(dc, "p_updated_id", DbType.String, user.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, user.UpdatedTime);

            db.AddInParameter(dc, "p_id", DbType.String, user.Id);

            db.ExecuteNonQuery(dc);
        }

        public static IList<UserEntity> Query(string name, string rootId, int count = 20)
        {
            var sql = string.Format(@"select top {1} {0} from users
where 1=1 and name like @p_name and unit_id in (select id from units where root_id=@p_root_id)
order by name", COLUMN_SQL, count);

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_name", DbType.String, "%" + name + "%");
            db.AddInParameter(dc, "p_root_id", DbType.String, rootId);

            var list = new List<UserEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new UserEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<UserEntity> Query(string condition, string rootId, string unitId, PagerInfo pager)
        {
            pager.ComputePageCount(QueryCount(condition, rootId, unitId));

            var list = new List<UserEntity>();


            var orderSql = " ORDER BY ";
            if (pager.OrderFields.Count > 0)
            {
                foreach (var field in pager.OrderFields)
                {
                    orderSql += field.Field + (field.Desc ? " DESC" : "") + ",";
                }
            }
            else
            {
                orderSql += "name";
            }

            var sql = string.Format(@"SELECT {0} FROM users WHERE 1=1{1}", COLUMN_SQL, GetConditionSql(condition, rootId, unitId));

            sql = @"SELECT * FROM
            (
                SELECT ROW_NUMBER() OVER(" + orderSql + @") pid," + COLUMN_SQL + @"
                FROM (" + sql + @") t            
            ) t1 WHERE t1.pid BETWEEN @p_pageNo * @p_pageSize + 1 AND (@p_pageNo + 1) * @p_pageSize ";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_condition", DbType.String, "%" + condition + "%");
            if (string.IsNullOrEmpty(unitId))
            {
                db.AddInParameter(dc, "p_root_id", DbType.String, rootId);
            }
            else
            {
                db.AddInParameter(dc, "p_unit_id", DbType.String, unitId);
            }
            db.AddInParameter(dc, "p_pageNo", DbType.Int32, pager.PageIndex);
            db.AddInParameter(dc, "p_pageSize", DbType.Int32, pager.PageSize);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new UserEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        private static int QueryCount(string condition, string rootId, string unitId)
        {
            var sql = string.Format(@"SELECT COUNT(id) FROM users WHERE 1=1 ", COLUMN_SQL);

            var conditionSql = GetConditionSql(condition, rootId, unitId);
            if (!string.IsNullOrEmpty(conditionSql))
            {
                sql += conditionSql;
            }

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_condition", DbType.String, condition);

            if(string.IsNullOrEmpty(unitId))
            {
                db.AddInParameter(dc, "p_root_id", DbType.String, rootId);
            }
            else
            {
                db.AddInParameter(dc, "p_unit_id", DbType.String, unitId);
            }

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                reader.Read();

                return reader.GetInt32(0);
            }
        }

        private static string GetConditionSql(string condition, string rootId, string unitId)
        {
            var conditionSql = " ";
            if (!string.IsNullOrEmpty(condition))
            {
                conditionSql = " AND (name LIKE @p_condition OR account LIKE @p_condition)";
            }

            if(!string.IsNullOrEmpty(unitId))
            {
                conditionSql += " AND unit_id = @p_unit_id";
            }
            else
            {
                conditionSql += " AND (unit_id IN (SELECT id FROM units WHERE root_id = @p_root_id) or unit_id = @p_root_id)";
            }

            return conditionSql;
        }


        public static IList<UserEntity> GetManagers(string unitId)
        {
            var sql = string.Format("select {0} from users where unit_id = @p_unit_id and title = 1", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_unit_id", DbType.String, unitId);

            var list = new List<UserEntity>();
            using (IDataReader reader = db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var user = new UserEntity();
                    user.Init(reader);

                    list.Add(user);
                }
            }

            return list;
        }
    }
}
