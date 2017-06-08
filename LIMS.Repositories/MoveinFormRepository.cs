using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

using LIMS.Entities;
using LIMS.Util;

namespace LIMS.Repositories
{
    public static class MoveinFormRepository
    {
        private const string COLUMN_SQL = @"id, moveout_id, moveout_form_no, scan_over, confirm_user_id, confirm_datetime,
created_id, created_time, updated_id, updated_time";

        public static IList<MoveFormEntity> QueryMoveForms(string hospitalId)
        {
            var sql = @"select a.id, b.form_no, b.new_storeroom_id from movein_form a, moveout_form b where a.moveout_id=b.id and a.scan_over=0 and b.hospital_id=@p_hospital_id";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            var list = new List<MoveFormEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    var entity = new MoveFormEntity();

                    entity.Id = reader["id"].ToString();
                    entity.FormNo = reader.GetInt32(reader.GetOrdinal("form_no"));
                    entity.ToStoreroomId = reader["new_storeroom_id"].ToString();

                    list.Add(entity);
                }
            }

            return list;
        }

        public static void Save(MoveinFormEntity entity, Database db, DbTransaction trans)
        {
            var sql = string.Format(@"insert into movein_form({0})
values(@p_id, @p_moveout_id, @p_moveout_form_no, @p_scan_over, @p_confirm_user_id, @p_confirm_datetime,
        @p_created_id, @p_created_time, @p_updated_id, @p_updated_time)", COLUMN_SQL);
            
            entity.Id = Guid.NewGuid().ToString();
            
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);
            db.AddInParameter(dc, "p_moveout_id", DbType.String, entity.MoveoutId);
            db.AddInParameter(dc, "p_moveout_form_no", DbType.Int32, entity.MoveoutFormNo);
            db.AddInParameter(dc, "p_scan_over", DbType.Boolean, entity.ScanOver);
            db.AddInParameter(dc, "p_confirm_user_id", DbType.String, DBNull.Value);
            db.AddInParameter(dc, "p_confirm_datetime", DbType.DateTime, DBNull.Value);
            db.AddInParameter(dc, "p_created_id", DbType.String, entity.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, entity.CreatedTime);
            db.AddInParameter(dc, "p_updated_id", DbType.String, entity.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, entity.UpdatedTime);

            db.ExecuteNonQuery(dc, trans);
        }

        public static MoveinFormEntity Get(string id)
        {
            var sql = string.Format("select {0} from movein_form where id=@p_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);

            using (var reader = db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var entity = new MoveinFormEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static void Confirm(string id, string userId, string hospitalId)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("The id of movein form is empty.");
            }
            var movein = Get(id);
            if (movein == null)
            {
                throw new Exception(string.Format("The id({0}) does not exist.", id));
            }
            var moveout = MoveoutFormRepository.Get(movein.MoveoutId);
            if (movein == null)
            {
                throw new Exception(string.Format("The id({0}) of moveout form does not exist.", id));
            }

            var db = DatabaseFactory.CreateDatabase();
            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        Confirm(id, userId, db, trans);
                        GoodsStateRepository.ChangeState(id, FormType.MoveIn, userId, db, trans);
                        GoodsRepsitory.MoveStoreroom(moveout.NewStoreroomId, movein.Id, hospitalId, userId, db, trans);

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

        private static void Confirm(string id, string userId, Database db, DbTransaction trans)
        {
            var sql = @"update movein_form set scan_over=@p_scan_over, confirm_user_id=@p_user_id, confirm_datetime=@p_datetime, 
updated_id=@p_user_id, updated_time=@p_datetime where id=@p_id";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);
            db.AddInParameter(dc, "p_scan_over", DbType.Boolean, true);
            db.AddInParameter(dc, "p_user_id", DbType.String, userId);
            db.AddInParameter(dc, "p_datetime", DbType.DateTime, DateTime.Now);

            db.ExecuteNonQuery(dc, trans);
        }
    }
}
