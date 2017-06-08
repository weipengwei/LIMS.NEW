using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using LIMS.Entities;

namespace LIMS.Repositories
{
    public static class UserPrivilegeRepository
    {
        private const string COLUMN_SQL = @"id, user_id, unit_root_id, unit_id, query, operate,created_id, created_time, updated_id, updated_time";
        
        public static IList<UserPrivilegeEntity> Query(string userId, string rootId)

        {
            var sql = string.Format("SELECT {0} FROM user_privilege WHERE user_id = @p_user_id AND unit_root_id=@p_unit_root_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_user_id", DbType.String, userId);
            db.AddInParameter(dc, "p_unit_root_id", DbType.String, rootId);

            var list = new List<UserPrivilegeEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new UserPrivilegeEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static void Save(string userId, string rootId, IList<UserPrivilegeEntity> entities)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {

                        Delete(userId, rootId, db, trans);
                        foreach (var item in entities)
                        {
                            Add(item, db, trans);
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        private static void Delete(string userId, string rootId, Database db, DbTransaction trans)
        {
            var sql = "DELETE user_privilege WHERE user_id = @p_user_id and unit_root_id=@p_unit_root_id";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_user_id", DbType.String, userId);
            db.AddInParameter(dc, "p_unit_root_id", DbType.String, rootId);

            db.ExecuteNonQuery(dc, trans);
        }

        private static void Add(UserPrivilegeEntity entity, Database db, DbTransaction trans)
        {
            var sql = string.Format(@"INSERT INTO user_privilege({0})
VALUES(@p_id, @p_user_id, @p_unit_root_id, @p_unit_id, @p_query, @p_operate,
@p_created_id, @p_created_time, @p_updated_id, @p_updated_time)", COLUMN_SQL);

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);
            db.AddInParameter(dc, "p_user_id", DbType.String, entity.UserId);
            db.AddInParameter(dc, "p_unit_root_id", DbType.String, entity.UnitRootId);
            db.AddInParameter(dc, "p_unit_id", DbType.String, entity.UnitId);
            db.AddInParameter(dc, "p_query", DbType.Boolean, entity.Query);
            db.AddInParameter(dc, "p_operate", DbType.Boolean, entity.Operate);
            db.AddInParameter(dc, "p_created_id", DbType.String, entity.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, entity.CreatedTime);
            db.AddInParameter(dc, "p_updated_id", DbType.String, entity.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, entity.UpdatedTime);

            db.ExecuteNonQuery(dc, trans);
        }
    }
}
