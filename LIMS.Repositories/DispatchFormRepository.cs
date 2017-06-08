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
using LIMS.Util;

namespace LIMS.Repositories
{
    public static class DispatchFormRepository
    {
        private const string COLUMN_SQL = @"
id,order_id,order_form_no,order_detail_id,apply_unit_id,product_id,dispatched_count,hospital_id,vendor_id,
status,changed_id,changed_time,created_id,created_time
";

        #region Get form
        private static DispatchFormEntity Get(string id, Database db, DbTransaction trans)
        {
            var sql = string.Format("select {0} from dispatch_form {1} where id=@p_id", COLUMN_SQL, TransHelper.UpdateLock(trans));

            if (db == null)
            {
                db = DatabaseFactory.CreateDatabase();
            }

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);

            DispatchFormEntity entity = null;
            using (var reader = (trans == null ? db.ExecuteReader(dc) : db.ExecuteReader(dc, trans)))
            {
                while (reader.Read())
                {
                    entity = new DispatchFormEntity();
                    entity.Init(reader);

                    break;
                }
            }

            return entity;
        }
        #endregion

        #region Load
        public static DispatchFormEntity Get(string id)
        {
            var sql = string.Format("select {0} from dispatch_form where id=@p_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);
            
            using (var reader = db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var entity = new DispatchFormEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static IList<DispatchFormEntity> GetDispatches(int formNo,string hospitalID)
        {
            var sql = string.Format("select {0} from dispatch_form where order_form_no=@p_form_no and hospital_id=@p_hospital_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_form_no", DbType.Int32, formNo);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalID);

            var list = new List<DispatchFormEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new DispatchFormEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<DispatchFormItemEntity> GetItems(int formNo, string hospitalID)
        {
            var sql = @"
select a.*
from dispatch_form_items a inner join dispatch_form b on a.dispatch_id = b.id
where b.order_form_no =@p_form_no and b.hospital_id=@p_hospital_id
order by a.created_time desc";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_form_no", DbType.Int32, formNo);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalID);

            var list = new List<DispatchFormItemEntity>();
            using (var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new DispatchFormItemEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static DispatchFormItemEntity GetItemBySerialId(string serialId)
        {
            var sql = @"
select 
    id,dispatch_id,serial_id,count,batch_no,expired_date,logistics_code,logistics_content,
    is_confirmed,confirmed_id,confirmed_time,created_id,created_time
from dispatch_form_items where serial_id=@p_serial_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_serial_id", DbType.String, serialId);
            using (var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    var entity = new DispatchFormItemEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static IList<DispatchFormItemEntity> GetItems(string dispatchId)
        {
            var sql = @"
select 
    id,dispatch_id,serial_id,count,batch_no,expired_date,logistics_code,logistics_content,
    is_confirmed,confirmed_id,confirmed_time,created_id,created_time
from dispatch_form_items where dispatch_id=@p_dispatch_id order by created_time desc";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_dispatch_id", DbType.String, dispatchId);

            var list = new List<DispatchFormItemEntity>();
            using (var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new DispatchFormItemEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }
        #endregion


        #region Save Form
        public static void Create(IList<DispatchFormEntity> entities)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        foreach(var entity in entities)
                        {
                            entity.Id = Guid.NewGuid().ToString();

                            Create(entity, db, trans);

                            UpdateOrderStatus(entity, db, trans);
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

        private static void Create(DispatchFormEntity form, Database db, DbTransaction trans)
        {
            var sql = string.Format(@"INSERT INTO dispatch_form
(
id,order_id,order_form_no,order_detail_id,apply_unit_id,product_id,
dispatched_count,hospital_id,vendor_id,status,changed_id,changed_time,created_id,created_time
)
VALUES(
@p_id,@p_order_id,@p_order_form_no,@p_order_detail_id,@p_apply_unit_id,@p_product_id,
@p_dispatched_count,@p_hospital_id,@p_vendor_id,@p_status,@p_changed_id,@p_changed_time,@p_created_id,@p_created_time
)", COLUMN_SQL);

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, form.Id);
            db.AddInParameter(dc, "p_order_id", DbType.String, form.OrderId);
            db.AddInParameter(dc, "p_order_form_no", DbType.Int32, form.OrderFormNo);
            db.AddInParameter(dc, "p_order_detail_id", DbType.String, form.OrderDetailId);
            db.AddInParameter(dc, "p_apply_unit_id", DbType.String, form.ApplyUnitId);
            db.AddInParameter(dc, "p_product_id", DbType.String, form.ProductId);
            db.AddInParameter(dc, "p_dispatched_count", DbType.Int32, form.DispatchedCount);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, form.HospitalId);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, form.VendorId);
            db.AddInParameter(dc, "p_status", DbType.String, form.Status);
            db.AddInParameter(dc, "p_changed_id", DbType.String, form.ChangedId);
            db.AddInParameter(dc, "p_changed_time", DbType.DateTime, form.ChangedTime);
            db.AddInParameter(dc, "p_created_id", DbType.String, form.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, form.CreatedTime);

            db.ExecuteNonQuery(dc, trans);
        }

        private static void UpdateOrderStatus(DispatchFormEntity form, Database db, DbTransaction trans)
        {
            OrderFormRepository.UpdateStatus(form.OrderId, form.OrderDetailId, OrderFormItemStatus.Dispatching, db, trans);
        }
        #endregion


        #region Create a form item and a goods serial
        public static void CreateItem(DispatchFormEntity form, DispatchFormItemEntity formItem)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var count = SumItems(form.Id);

                        if(form.DispatchedCount == count + formItem.Count)
                        {
                            UpdateStatus(form.Id, DispatchFormStatus.Dispatching, formItem.CreatedId, formItem.CreatedTime, db, trans);
                        }
                        else if(count == 0)
                        {
                            UpdateStatus(form.Id, DispatchFormStatus.Dispatching, formItem.CreatedId, formItem.CreatedTime, db, trans);
                        }
                        else if (formItem.Count + count > form.DispatchedCount)
                        {
                            throw new Exception("The count of dispatch form is over.");
                        }

                        formItem.Id = Guid.NewGuid().ToString();

                        formItem.SerialId = DispatchGoods(form, formItem, db, trans);

                        CreateItem(formItem, db, trans);

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

        public static int SumItems(string dispatchId)
        {
            var sql = "select sum(count) from dispatch_form_items where dispatch_id=@p_dispatch_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_dispatch_id", DbType.String, dispatchId);

            using (var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    return reader[0] == DBNull.Value ? 0 : reader.GetInt32(0);
                }
                else
                {
                    return 0;
                }
            }
        }

        private static void UpdateStatus(string id, string status, string userId, DateTime time, Database db, DbTransaction trans)
        {
            var sql = "update dispatch_form set status=@p_status,changed_id=@p_changed_id,changed_time=@p_changed_time where id=@p_id";
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_status", DbType.String, status);
            db.AddInParameter(cmd, "p_changed_id", DbType.String, userId);
            db.AddInParameter(cmd, "p_changed_time", DbType.String, time);
            db.AddInParameter(cmd, "p_id", DbType.String, id);

            db.ExecuteNonQuery(cmd, trans);
        }

        private static void CreateItem(DispatchFormItemEntity formItem, Database db, DbTransaction trans)
        {
            var sql = @"insert into dispatch_form_items(
id,dispatch_id,serial_id,count,batch_no,expired_date,is_confirmed,created_id,created_time
)
values(
@p_id,@p_dispatch_id,@p_serial_id,@p_count,@p_batch_no,@p_expired_date,@p_is_confirmed,@p_created_id,@p_created_time
)";

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, formItem.Id);
            db.AddInParameter(cmd, "p_dispatch_id", DbType.String, formItem.DispatchId);
            db.AddInParameter(cmd, "p_serial_id", DbType.String, formItem.SerialId);
            db.AddInParameter(cmd, "p_count", DbType.Int32, formItem.Count);
            db.AddInParameter(cmd, "p_batch_no", DbType.String, formItem.BatchNo);
            db.AddInParameter(cmd, "p_expired_date", DbType.DateTime, formItem.ExpiredDate);
            db.AddInParameter(cmd, "p_is_confirmed", DbType.Boolean, formItem.IsConfirmed);
            db.AddInParameter(cmd, "p_created_id", DbType.String, formItem.CreatedId);
            db.AddInParameter(cmd, "p_created_time", DbType.DateTime, formItem.CreatedTime);

            db.ExecuteNonQuery(cmd, trans);
        }

        private static string DispatchGoods(DispatchFormEntity form, DispatchFormItemEntity formItem, Database db, DbTransaction trans)
        {
            var orderDetail = OrderFormRepository.GetItem(form.OrderDetailId);

            var goodsSerial = new GoodsSerialEntity
            {
                ProductId = form.ProductId,
                DispatchedCount = formItem.Count,
                HospitalId = form.HospitalId,
                VendorId = form.VendorId,
                NeedAudit = orderDetail.NeedAudit,
                NeedCheck = orderDetail.NeedCheck,
                NeedSplit = orderDetail.NeedSplit,
                SplitCopies = orderDetail.SplitCopies,
                SplitUnit = orderDetail.SplitUnit,
                SplitCapacity = orderDetail.SplitCapacity,
                SplitPackageCount = orderDetail.SplitPackageCount,
                ValidDays = orderDetail.ValidDays,
                BatchNo = formItem.BatchNo,
                ExpiredDate = formItem.ExpiredDate,
                IsClosed = false,
                CreatedId = form.CreatedId,
                CreatedTime = DateTime.Now,
                UpdatedId = form.CreatedId,
                UpdatedTime = DateTime.Now
            };
            GoodsSerialRepository.Create(goodsSerial, db, trans);

            GoodsSerialFormRepository.Create(new GoodsSerialFormEntity
            {
                SerialId = goodsSerial.Id,
                FormId = form.Id,
                FormKind = FormKind.DispatchItem,
                CreatedId = form.CreatedId,
                CreatedTime = DateTime.Now,
            }, new GoodsSerialFormEntity
            {
                SerialId = goodsSerial.Id,
                FormId = form.OrderDetailId,
                FormKind = FormKind.OrderDetail,
                CreatedId = form.CreatedId,
                CreatedTime = DateTime.Now,
            }, db, trans);

            return goodsSerial.Id;
        }
        #endregion


        #region Query Dispatch Form
        public static IList<DispatchFormEntity> Query(IList<string> orderDetailIds)
        {
            var list = new List<DispatchFormEntity>();
            if(orderDetailIds == null || orderDetailIds.Count == 0)
            {
                return list;
            }

            IList<string> paramNames;
            string paramSql;
            LIMS.Util.StringHelper.GenerInParameters("p_order_detail_id", orderDetailIds.Count, out paramNames, out paramSql);

            var sql = string.Format(@"select {0} from dispatch_form where order_detail_id in ({1}) and status in ('{2}','{3}','{4}')", COLUMN_SQL, paramSql, DispatchFormStatus.Waiting, DispatchFormStatus.Confirmed, DispatchFormStatus.Dispatching);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            for (var i = 0; i < orderDetailIds.Count; i++)
            {
                db.AddInParameter(dc, paramNames[i], DbType.String, orderDetailIds[i]);
            }

            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new DispatchFormEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<DispatchFormEntity> Query(DispatchQueryCondition condition, PagerInfo pager)
        {
            var baseSql = @"select {0} from dispatch_form where 1=1 {1}";
            var conditionSql = GetConditionSql(condition);

            var db = DatabaseFactory.CreateDatabase();

            pager.ComputePageCount(QueryCount(baseSql, conditionSql, condition, db));

            var orderSql = " order by ";
            if (pager.OrderFields.Count > 0)
            {
                foreach (var field in pager.OrderFields)
                {
                    orderSql += field.Field + (field.Desc ? " DESC" : "") + ",";
                }
            }
            else
            {
                orderSql += " order_form_no desc,product_id,created_time desc,status ";
            }

            var sql = string.Format(baseSql, COLUMN_SQL, conditionSql);
            sql = @"SELECT * FROM
            (
                SELECT ROW_NUMBER() OVER(" + orderSql + @") pid," + COLUMN_SQL + @"
                FROM (" + sql + @") t            
            ) t1 WHERE t1.pid BETWEEN @p_pageNo * @p_pageSize + 1 AND (@p_pageNo + 1) * @p_pageSize ";
            
            var cmd = db.GetSqlStringCommand(sql);
            AddParameters(condition, db, cmd);
            db.AddInParameter(cmd, "p_pageNo", DbType.Int32, pager.PageIndex);
            db.AddInParameter(cmd, "p_pageSize", DbType.Int32, pager.PageSize);
            
            var list = new List<DispatchFormEntity>();
            using (var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new DispatchFormEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        private static int QueryCount(string baseSql, string conditionSql, DispatchQueryCondition condition, Database db)
        {
            var sql = string.Format(baseSql, "count(*)", conditionSql);
            var cmd = db.GetSqlStringCommand(sql);
            AddParameters(condition, db, cmd);

            using (IDataReader reader = db.ExecuteReader(cmd))
            {
                reader.Read();

                return reader.GetInt32(0);
            }
        }

        private static string GetConditionSql(DispatchQueryCondition condition)
        {
            var sql = " and vendor_id=@p_vendor_id and hospital_id=@p_hospital_id ";
            sql += @" and product_id in (select b.product_id from user_privilege a join vendor_products b on a.unit_id=b.unit_id where user_id=@p_user_id and operate=1) ";
            
            if (condition.FormNo.HasValue)
            {
                sql += " and order_form_no=@p_form_no";
            }

            if (!string.IsNullOrEmpty(condition.Status))
            {
                if (string.Compare(condition.Status, DispatchFormStatus.QueryDispatchable, true) == 0)
                {
                    sql += " and (status=@p_waiting or status=@p_dispatching)";
                }
                else
                {
                    sql += " and status=@p_status";
                }
            }

            return sql;
        }

        private static void AddParameters(DispatchQueryCondition condition, Database db, DbCommand cmd)
        {
            db.AddInParameter(cmd, "p_vendor_id", DbType.String, condition.VendorId);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, condition.HospitalId);
            db.AddInParameter(cmd, "p_user_id", DbType.String, condition.UserId);

            if(condition.FormNo.HasValue)
            {
                db.AddInParameter(cmd, "p_form_no", DbType.String, condition.FormNo);
            }

            if (!string.IsNullOrEmpty(condition.Status))
            {
                if(string.Compare(condition.Status, DispatchFormStatus.QueryDispatchable, true) == 0)
                {
                    db.AddInParameter(cmd, "p_waiting", DbType.String, DispatchFormStatus.Waiting);
                    db.AddInParameter(cmd, "p_dispatching", DbType.String, DispatchFormStatus.Dispatching);
                }
                else
                {
                    db.AddInParameter(cmd, "p_status", DbType.String, condition.Status);
                }
            }
        }
        #endregion


        #region Confirm to Pass
        public static void Pass(DispatchFormItemEntity formItem)
        {
            var form = Get(formItem.DispatchId);
            if(form == null)
            {
                throw new Exception("The dispatch form does not exist.");
            }

            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        UpdateItem(formItem, db, trans);
                        UpdateDispatchStatus(form.Id, db, trans);
                        UpdateGoodsSerial(formItem, db, trans);
                        CreateReceiveForm(form, formItem, db, trans);

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

        private static void UpdateItem(DispatchFormItemEntity form, Database db, DbTransaction trans)
        {
            var sql = @"update dispatch_form_items set 
logistics_code=@p_logistics_code,logistics_content=@p_logistics_content,
is_confirmed=@p_is_confirmed,confirmed_id=@p_confirmed_id,confirmed_time=@p_confirmed_time
where id=@p_id";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, form.Id);
            db.AddInParameter(dc, "p_logistics_code", DbType.String, form.LogisticsCode);
            db.AddInParameter(dc, "p_logistics_content", DbType.String, form.LogisticsContent);
            db.AddInParameter(dc, "p_is_confirmed", DbType.Boolean, form.IsConfirmed);
            db.AddInParameter(dc, "p_confirmed_id", DbType.String, form.ConfirmedId);
            db.AddInParameter(dc, "p_confirmed_time", DbType.DateTime, form.ConfirmedTime);

            db.ExecuteNonQuery(dc, trans);
        }

        private static void UpdateGoodsSerial(DispatchFormItemEntity form, Database db, DbTransaction trans)
        {
            GoodsSerialRepository.UpdateSerialInfo(new GoodsSerialEntity
            {
                Id = form.SerialId,
                LogisticsCode = form.LogisticsCode,
                LogisticsContent = form.LogisticsContent,
                UpdatedId = form.ConfirmedId,
                UpdatedTime = form.ConfirmedTime
            }, db, trans);
        }

        private static void CreateReceiveForm(DispatchFormEntity form, DispatchFormItemEntity formItem, Database db, DbTransaction trans)
        {
            var receiveForm = new ReceiveFormEntity
            {
                OrderId = form.OrderId,
                OrderFormNo = form.OrderFormNo,
                OrderDetailId = form.OrderDetailId,
                ApplyUnitId = form.ApplyUnitId,
                SerialId = formItem.SerialId,
                ProductId = form.ProductId,
                ReceivedCount = formItem.Count ,
                HospitalId = form.HospitalId,
                VendorId = form.VendorId,
                CreatedId = form.ChangedId,
                CreatedTime = form.ChangedTime
            };

            ReceiveFormRepository.Create(receiveForm, db, trans);
        }

        private static void UpdateDispatchStatus(string formId, Database db, DbTransaction trans)
        {
            var cmd = db.GetStoredProcCommand("dbo.sp_update_dispatch_status", formId);

            db.ExecuteNonQuery(cmd, trans);
        }
        #endregion


        public static void Cancel(string id, string userId)
        {
            var sql = "update dispatch_form set status=@p_status,changed_id=@p_changed_id,changed_time=@p_changed_time where id=@p_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_id", DbType.String, id);
            db.AddInParameter(cmd, "p_status", DbType.String, DispatchFormStatus.Cancelled);
            db.AddInParameter(cmd, "p_changed_id", DbType.String, userId);
            db.AddInParameter(cmd, "p_changed_time", DbType.String, DateTime.Now);

            db.ExecuteNonQuery(cmd);
        }
    }
}
