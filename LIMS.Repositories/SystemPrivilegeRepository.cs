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
    public static class SystemPrivilegeRepository
    {
        private const string COLUMN_SQL = @"id, object_id, object_type, fun_key, query, operate, created_id, created_time, updated_id, updated_time";

        /// <summary>
        /// 根据objectId和objectType获取SystemPrivilege
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static IList<SystemPrivilegeEntity> GetByObjectId(string objectId, int objectType = -1)
        {
            var sql = string.Format(@"SELECT {0} FROM system_privilege WHERE object_id = @p_object_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            if (objectType != -1)
            {
                sql += " AND ObjectType=@ObjectType  ";
            }
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_object_id", DbType.String, objectId);
            if (objectType != -1)
            {
                db.AddInParameter(dc, "ObjectType", DbType.Int32, objectType);
            }
            var list = new List<SystemPrivilegeEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    var entity = new SystemPrivilegeEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static void Save(string objectId, IList<SystemPrivilegeEntity> entities)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {

                        Delete(objectId, db, trans);
                        foreach(var item in entities)
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

        private static void Delete(string objectId, Database db, DbTransaction trans)
        {
            var sql = "DELETE system_privilege WHERE object_id = @p_object_id";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_object_id", DbType.String, objectId);

            db.ExecuteNonQuery(dc, trans);
        }

        private static void Add(SystemPrivilegeEntity entity, Database db, DbTransaction trans)
        {
            var sql = string.Format(@"INSERT INTO system_privilege({0})
VALUES(@p_id, @p_object_id, @p_object_type, @p_fun_key, @p_query, @p_operate,
@p_created_id, @p_created_time, @p_updated_id, @p_updated_time)", COLUMN_SQL);

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);
            db.AddInParameter(dc, "p_object_id", DbType.String, entity.ObjectId);
            db.AddInParameter(dc, "p_object_type", DbType.Int32, entity.ObjectType);
            db.AddInParameter(dc, "p_fun_key", DbType.String, entity.FunKey);
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
