using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using LIMS.Entities;
using LIMS.Util;

namespace LIMS.Repositories
{
    public static class FormApproveListRepository
    {
        private const string COLUMN_SQL = @"id, form_type, hospital_id, approver_type, approver_id, sequence, 
created_id, created_time, updated_id, updated_time";

        public static FormApproveListEntity Get(string id)
        {
            var sql = string.Format("select {0} from form_approve_list where id = @p_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);

            using (var reader = db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var entity = new FormApproveListEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static IList<FormApproveListEntity> Get(FormType formType)
        {
            var sql = string.Format("select {0} from form_approve_list where form_type = @p_form_type order by sequence", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_form_type", DbType.Int32, (int)formType);

            var list = new List<FormApproveListEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new FormApproveListEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static void Delete(string id)
        {
            var sql = "delete from form_approve_list where id = @p_id";

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);

            db.ExecuteNonQuery(dc);
        }

        public static void Save(FormApproveListEntity entity)
        {
            var sql = string.Format(@"insert into form_approve_list({0})
values(@p_id, @p_form_type, @p_hospital_id, @p_approver_type, @p_approver_id, @p_sequence,
        @p_created_id, @p_created_time, @p_updated_id, @p_updated_time)", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);
            db.AddInParameter(dc, "p_form_type", DbType.Int32, (int)entity.FormType);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, entity.HospitalId);
            db.AddInParameter(dc, "p_approver_type", DbType.Int32, (int)entity.ApproverType);
            db.AddInParameter(dc, "p_approver_id", DbType.String, entity.ApproverId);
            db.AddInParameter(dc, "p_sequence", DbType.Int32, entity.Sequence);
            db.AddInParameter(dc, "p_created_id", DbType.String, entity.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, entity.CreatedTime);
            db.AddInParameter(dc, "p_updated_id", DbType.String, entity.CreatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, entity.CreatedTime);

            db.ExecuteNonQuery(dc);
        }

        public static void Move(string current, string exchange, string userId)
        {
            var currentEntity = Get(current);
            var exchangeEntity = Get(exchange);

            if(currentEntity == null)
            {
                return;
            }

            if(exchangeEntity == null)
            {
                return;
            }
            
            var db = DatabaseFactory.CreateDatabase();
            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        Move(current, exchangeEntity.Sequence, userId, db, trans);
                        Move(exchange, currentEntity.Sequence, userId, db, trans);

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

        private static void Move(string id, int sequence, string userId, Database db, DbTransaction trans)
        {
            var sql = "update form_approve_list set sequence=@p_sequence, updated_id=@p_user_id, updated_time=@p_updated_time where id=@p_id";
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);
            db.AddInParameter(dc, "p_sequence", DbType.Int32, sequence);
            db.AddInParameter(dc, "p_user_id", DbType.String, userId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, DateTime.Now);

            db.ExecuteNonQuery(dc, trans);
        }
    }
}
