using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using LIMS.Models;
using LIMS.Entities;
using LIMS.Util;

namespace LIMS.Repositories
{
    public static class MoveoutFormRepository
    {
        private const string COLUMN_SQL = @"id, hospital_id, form_no, applyer, apply_time, new_storeroom_id, scan_over, 
confirm_user_id, confirm_datetime, created_id, created_time, updated_id, updated_time";

        #region Query
        public static IList<MoveoutFormEntity> Query(DateRangeCondition condition, PagerInfo pager)
        {
            pager.ComputePageCount(QueryCount(condition));

            var list = new List<MoveoutFormEntity>();


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
                orderSql += "form_no DESC";
            }

            var sql = string.Format(@"SELECT {0} FROM moveout_form WHERE is_active = 1 and {1}", COLUMN_SQL, GetConditionSql(condition));

            sql = @"SELECT * FROM
            (
                SELECT ROW_NUMBER() OVER(" + orderSql + @") pid," + COLUMN_SQL + @"
                FROM (" + sql + @") t            
            ) t1 WHERE t1.pid BETWEEN @p_pageNo * @p_pageSize + 1 AND (@p_pageNo + 1) * @p_pageSize ";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            AddParameter(dc, db, condition);

            db.AddInParameter(dc, "p_pageNo", DbType.Int32, pager.PageIndex);
            db.AddInParameter(dc, "p_pageSize", DbType.Int32, pager.PageSize);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new MoveoutFormEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        private static int QueryCount(DateRangeCondition condition)
        {
            var sql = "SELECT COUNT(id) FROM moveout_form WHERE ";

            sql += GetConditionSql(condition);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            AddParameter(dc, db, condition);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                reader.Read();

                return reader.GetInt32(0);
            }
        }

        private static string GetConditionSql(DateRangeCondition condition)
        {
            var conditionSql = @" 1=1 and new_storeroom_id in (
select unit_id from user_privilege 
where user_id = @p_user_id and unit_root_id = @p_hospital_id and operate=1)";

            int formNo;
            if (!string.IsNullOrEmpty(condition.Content) && int.TryParse(condition.Content, out formNo))
            {
                conditionSql += " and form_no = @p_form_no";
            }

            if (condition.BeginDate.HasValue)
            {
                conditionSql += " AND apply_time >= @p_begin_date";
            }

            if (condition.EndDate.HasValue)
            {
                conditionSql += " AND apply_time <= @p_end_date";
            }

            return conditionSql;
        }

        private static void AddParameter(DbCommand dc, Database db, DateRangeCondition condition)
        {
            db.AddInParameter(dc, "p_user_id", DbType.String, condition.UserId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, condition.HospitalId);

            int formNo;
            if (!string.IsNullOrEmpty(condition.Content) && int.TryParse(condition.Content, out formNo))
            {
                db.AddInParameter(dc, "p_form_no", DbType.Int32, formNo);
            }

            if (condition.BeginDate.HasValue)
            {
                db.AddInParameter(dc, "p_begin_date", DbType.DateTime, condition.BeginDate);
            }

            if (condition.EndDate.HasValue)
            {
                db.AddInParameter(dc, "p_end_date", DbType.DateTime, condition.EndDate);
            }
        }
        #endregion

        #region Save
        public static void Save(MoveoutFormEntity entity)
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                Add(entity);
            }
            else
            {
                Update(entity);
            }
        }

        private static void Add(MoveoutFormEntity entity)
        {
            var sql = string.Format(@"insert into moveout_form({0})
values(@p_id, @p_hospital_id, @p_form_no, @p_applyer, @p_apply_time, @p_new_storeroom_id, @p_scan_over,
    @p_confirm_user_id, @p_confirm_datetime, @p_created_id, @p_created_time, @p_updated_id, @p_updated_time)", COLUMN_SQL);

            entity.Id = Guid.NewGuid().ToString();

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, entity.HospitalId);
            db.AddInParameter(dc, "p_form_no", DbType.Int32, entity.FormNo);
            db.AddInParameter(dc, "p_applyer", DbType.String, entity.Applyer);
            db.AddInParameter(dc, "p_apply_time", DbType.DateTime, entity.ApplyTime);
            db.AddInParameter(dc, "p_new_storeroom_id", DbType.String, entity.NewStoreroomId);
            db.AddInParameter(dc, "p_scan_over", DbType.Boolean, entity.ScanOver);
            db.AddInParameter(dc, "p_confirm_user_id", DbType.String, DBNull.Value);
            db.AddInParameter(dc, "p_confirm_datetime", DbType.DateTime, DBNull.Value);
            db.AddInParameter(dc, "p_created_id", DbType.String, entity.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, entity.CreatedTime);
            db.AddInParameter(dc, "p_updated_id", DbType.String, entity.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, entity.UpdatedTime);

            db.ExecuteNonQuery(dc);
        }

        private static void Update(MoveoutFormEntity entity)
        {
            var sql = @"update moveout_form set new_storeroom_id=@p_new_storeroom_id, 
updated_id=@p_updated_id, updated_time=@p_updated_time where id=@p_id";
            
            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);
            db.AddInParameter(dc, "p_new_storeroom_id", DbType.String, entity.NewStoreroomId);
            db.AddInParameter(dc, "p_updated_id", DbType.String, entity.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, entity.UpdatedTime);

            db.ExecuteNonQuery(dc);
        }
        #endregion

        public static MoveoutFormEntity Get(string id)
        {
            var sql = string.Format("select {0} from moveout_form where is_active=1 and id=@p_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);

            using (var reader = db.ExecuteReader(dc))
            {
                if(reader.Read())
                {
                    var entity = new MoveoutFormEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static void Confirm(string id, string userId)
        {
            if(string.IsNullOrEmpty(id))
            {
                throw new Exception("The id of moveout form is empty.");
            }
            var moveout = Get(id);
            if(moveout == null)
            {
                throw new Exception(string.Format("The id({0}) does not exist.", id));
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
                        var moveinId = CreateMoveinForm(moveout, userId, db, trans);

                        GoodsStateRepository.ChangeState(id, FormType.MoveOut, moveinId, FormType.MoveIn, userId, db, trans);

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
            var sql = @"update moveout_form set scan_over=@p_scan_over, confirm_user_id=@p_user_id, confirm_datetime=@p_datetime, 
updated_id=@p_user_id, updated_time=@p_datetime where id=@p_id";
            
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);
            db.AddInParameter(dc, "p_scan_over", DbType.Boolean, true);
            db.AddInParameter(dc, "p_user_id", DbType.String, userId);
            db.AddInParameter(dc, "p_datetime", DbType.DateTime, DateTime.Now);

            db.ExecuteNonQuery(dc, trans);
        }

        private static string CreateMoveinForm(MoveoutFormEntity moveout, string userId, Database db, DbTransaction trans)
        {
            var now = DateTime.Now;

            var movein = new MoveinFormEntity
            {
                MoveoutFormNo = moveout.FormNo,
                MoveoutId = moveout.Id,
                ScanOver = false,
                CreatedId = userId,
                CreatedTime = now,
                UpdatedId = userId,
                UpdatedTime = now
            };

            MoveinFormRepository.Save(movein, db, trans);

            return movein.Id;
        }


        public static bool Cancel(string id, string userId)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("The id of moveout form is empty.");
            }

            var sql = @"update moveout_form set is_active=@p_is_active, updated_id=@p_updated_id, updated_time=@p_updated_time where id=@p_id and is_active = 1";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);
            db.AddInParameter(dc, "p_is_active", DbType.Boolean, false);
            db.AddInParameter(dc, "p_updated_id", DbType.String, userId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, DateTime.Now);

            return db.ExecuteNonQuery(dc) > 0;
        }
    }
}
