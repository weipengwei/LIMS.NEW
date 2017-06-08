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
using LIMS.Models;

namespace LIMS.Repositories
{
    public static class ReturnFormRepository
    {
        private const string COLUMN_SQL = "id, form_no, filter_id, vendor_id, hospital_id, apply_date, status, logistics_barcode, logistics_info, created_id, created_time, updated_id, updated_time";

        #region Query
        public static IList<ReturnFormEntity> Query(ReturnQueryCondition condition, PagerInfo pager)
        {
            pager.ComputePageCount(QueryCount(condition));

            var list = new List<ReturnFormEntity>();


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

            var sql = string.Format(@"SELECT {0} FROM return_form WHERE {1}", COLUMN_SQL, GetConditionSql(condition));

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
                    var entity = new ReturnFormEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        private static int QueryCount(ReturnQueryCondition condition)
        {
            var sql = "SELECT COUNT(id) FROM return_form WHERE ";

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

        private static string GetConditionSql(ReturnQueryCondition condition)
        {
            var conditionSql = @" 1=1 and hospital_id = @p_hospital_id";

            int formNo;
            if (!string.IsNullOrEmpty(condition.Content) && int.TryParse(condition.Content, out formNo))
            {
                conditionSql += " and form_no = @p_form_no";
            }

            if (condition.Status != ReturnFormStatus.None)
            {
                conditionSql += " and status = @p_status";
            }

            return conditionSql;
        }

        private static void AddParameter(DbCommand dc, Database db, ReturnQueryCondition condition)
        {
            db.AddInParameter(dc, "p_hospital_id", DbType.String, condition.HospitalId);

            int formNo;
            if (!string.IsNullOrEmpty(condition.Content) && int.TryParse(condition.Content, out formNo))
            {
                db.AddInParameter(dc, "p_form_no", DbType.Int32, formNo);
            }

            if (condition.Status != ReturnFormStatus.None)
            {
                db.AddInParameter(dc, "p_status", DbType.Int32, condition.Status);
            }
        }
        #endregion

        #region Save
        public static void Save(ReturnFormEntity entity)
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

        private static void Add(ReturnFormEntity entity)
        {
            var sql = string.Format(@"insert into return_form({0})
values(
@p_id, @p_form_no, @p_filter_id, @p_vendor_id, @p_hospital_id, @p_apply_date, @p_status, '', '',
@p_created_id, @p_created_time, @p_updated_id, @p_updated_time
)", COLUMN_SQL);

            entity.Id = Guid.NewGuid().ToString();

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);
            db.AddInParameter(dc, "p_form_no", DbType.Int32, entity.FormNo);
            db.AddInParameter(dc, "p_filter_id", DbType.String, entity.FilterId);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, entity.VendorId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, entity.HospitalId);
            db.AddInParameter(dc, "p_apply_date", DbType.DateTime, entity.ApplyDate);
            db.AddInParameter(dc, "p_status", DbType.Int32, entity.Status);
            db.AddInParameter(dc, "p_created_id", DbType.String, entity.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, entity.CreatedTime);
            db.AddInParameter(dc, "p_updated_id", DbType.String, entity.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, entity.UpdatedTime);

            db.ExecuteNonQuery(dc);
        }

        private static void Update(ReturnFormEntity entity)
        {
            var sql = string.Format(@"update return_form
set filter_id = @p_filter_id, vendor_id = @p_vendor_id, status = @p_status, updated_id = @p_updated_id, updated_time = @p_updated_time
where id = @p_id", COLUMN_SQL);

            entity.Id = Guid.NewGuid().ToString();

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);
            db.AddInParameter(dc, "p_filter_id", DbType.String, entity.FilterId);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, entity.VendorId);
            db.AddInParameter(dc, "p_status", DbType.Int32, entity.Status);
            db.AddInParameter(dc, "p_updated_id", DbType.String, entity.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, entity.UpdatedTime);

            db.ExecuteNonQuery(dc);
        }
        #endregion

        public static ReturnFormEntity Get(string id)
        {
            var sql = string.Format("select {0} from return_form where id = @p_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);

            using (var reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    var entity = new ReturnFormEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static void UpdateStatus(string id, int status, string userId)
        {

            var sql = string.Format(@"update return_form
set status = @p_status, updated_id = @p_updated_id, updated_time = @p_updated_time
where id = @p_id", COLUMN_SQL);
            
            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);
            db.AddInParameter(dc, "p_status", DbType.Int32, status);
            db.AddInParameter(dc, "p_updated_id", DbType.String, userId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, DateTime.Now);

            db.ExecuteNonQuery(dc);
        }

        public static void Confirm(string id, string logisticsBarcode, string logisticsInfo, string userId)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        if(!Confirm(id, logisticsBarcode, logisticsInfo, db, trans))
                        {
                            throw new Exception("The return form id does not exist.");
                        }
                        GoodsStateRepository.ChangeState(id, FormType.Return, userId, db, trans);
                        
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

        private static bool Confirm(string id, string logisticsBarcode, string logisticsInfo, Database db, DbTransaction trans)
        {
            var sql = string.Format(@"update return_form
set status = @p_status, logistics_barcode = @p_logistics_barcode, logistics_info = @p_logistics_info
where id = @p_id", COLUMN_SQL);

            db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);
            db.AddInParameter(dc, "p_status", DbType.Int32, ReturnFormStatus.Confirmed);
            db.AddInParameter(dc, "p_logistics_barcode", DbType.String, logisticsBarcode);
            db.AddInParameter(dc, "p_logistics_info", DbType.String, logisticsInfo);

            return db.ExecuteNonQuery(dc, trans) > 0;
        }
    }
}
