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
    public static class InspectionFormRepository
    {
        private const string COLUMN_SQL = @"id,receive_count,inspection_count,receive_form,order_id,order_form_no,order_detail_id,
hospital_id,apply_unit_id,vendor_id,product_id,scan_over,confirm_user_id,confirm_datetime,
created_id,created_time,updated_id,updated_time";

        public static void Save(InspectionFormEntity entity, Database db, DbTransaction trans)
        {
            var sql = string.Format(@"insert into inspection_form({0})
values(
@p_id, @p_receive_count, @p_receive_count, @p_receive_form, @p_order_id, @p_order_form_no, @p_order_detail_id, @p_hospital_id,
@p_apply_unit_id, @p_vendor_id, @p_product_id, @p_scan_over, @p_confirm_user_id,@p_confirm_datetime,
@p_created_id, @p_created_time, @p_updated_id, @p_updated_time
)", COLUMN_SQL);

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);
            db.AddInParameter(dc, "p_receive_count", DbType.Int32, entity.ReceiveCount);
            db.AddInParameter(dc, "p_inspection_count", DbType.Int32, entity.InspectionCount);
            db.AddInParameter(dc, "p_receive_form", DbType.String, entity.ReceiveForm);
            db.AddInParameter(dc, "p_order_id", DbType.String, entity.OrderId);
            db.AddInParameter(dc, "p_order_form_no", DbType.Int32, entity.OrderFormNo);
            db.AddInParameter(dc, "p_order_detail_id", DbType.String, entity.OrderDetailId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, entity.HospitalId);
            db.AddInParameter(dc, "p_apply_unit_id", DbType.String, entity.ApplyUnitId);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, entity.VendorId);
            db.AddInParameter(dc, "p_product_id", DbType.String, entity.ProductId);
            db.AddInParameter(dc, "p_scan_over", DbType.Boolean, entity.ScanOver);
            db.AddInParameter(dc, "p_confirm_user_id", DbType.String, entity.ConfirmUserId);
            db.AddInParameter(dc, "p_confirm_datetime", DbType.DateTime, DBNull.Value);
            db.AddInParameter(dc, "p_created_id", DbType.String, entity.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, entity.CreatedTime);
            db.AddInParameter(dc, "p_updated_id", DbType.String, entity.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, entity.UpdatedTime);

            db.ExecuteNonQuery(dc, trans);
        }

        public static IList<InspectionFormEntity> QueryConfirm(string hospitalId)
        {
            var sql = string.Format(@"select {0} from inspection_form where scan_over = 0 and hospital_id=@p_hospital_id
order by order_form_no", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            var list = new List<InspectionFormEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new InspectionFormEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }


        public static InspectionFormEntity Get(string id)
        {
            return Get(id, null, null);
        }

        private static InspectionFormEntity Get(string id, Database db, DbTransaction trans)
        {
            var sql = string.Format("select {0} from inspection_form where id=@p_id", COLUMN_SQL);

            if (db == null)
            {
                db = DatabaseFactory.CreateDatabase();
            }

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);

            InspectionFormEntity entity = null;
            using (var reader = (trans == null ? db.ExecuteReader(dc) : db.ExecuteReader(dc, trans)))
            {
                while (reader.Read())
                {
                    entity = new InspectionFormEntity();
                    entity.Init(reader);

                    break;
                }
            }

            return entity;
        }


        #region Confirm
        public static void Confirm(string id, string userId)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var entity = Get(id, db, trans);
                        if (entity == null)
                        {
                            throw new Exception("The inspection form does not exist.");
                        }

                        var scanCount = GoodsStateRepository.CountValid(id, FormType.Inspection, db, trans);

                        UpdateConfirmStatus(id, scanCount, userId, db, trans);
                        
                        var needAudit = OrderFormRepository.GetItem(entity.OrderDetailId).NeedAudit;
                        
                        string formId;
                        FormType formType;
                        if (needAudit)
                        {
                            formId = CreateInspectionAudit();
                            formType = FormType.InspectionAudit;
                        }
                        else
                        {
                            formId = SaveIncomingForm(entity, scanCount, userId, db, trans);
                            formType = FormType.Incoming;
                        }

                        GoodsStateRepository.ChangeState(id, FormType.Inspection, formId, formType, userId, db, trans);
                        
                        //UpdateOrderStatus(entity.OrderDetailId, needAudit, db, trans);

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

        private static void UpdateConfirmStatus(string id, int scanCount, string userId, Database db, DbTransaction trans)
        {

            var sql = @"update inspection_form set scan_over=1, inspection_count=@p_inspection_count, 
confirm_user_id=@p_confirm_user_id, confirm_datetime = @p_confirm_datetime
where id=@p_id";
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);
            db.AddInParameter(dc, "p_inspection_count", DbType.String, scanCount);
            db.AddInParameter(dc, "p_confirm_user_id", DbType.String, userId);
            db.AddInParameter(dc, "p_confirm_datetime", DbType.String, DateTime.Now);

            db.ExecuteNonQuery(dc, trans);
        }

        private static void UpdateOrderStatus(string orderItemId, bool needAudit, Database db, DbTransaction trans)
        {
            var sql = "select sum(inspection_count) from inspection_form where order_detail_id=@p_order_detail_id";
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_order_detail_id", DbType.String, orderItemId);

            var count = 0;
            using (var reader = db.ExecuteReader(dc, trans))
            {
                reader.Read();

                count = Convert.ToInt32(reader[0]);
            }

            var detailEntity = OrderFormRepository.GetItem(orderItemId);
            if (detailEntity != null && detailEntity.Count == count)
            {
                if(needAudit)
                {
                    OrderFormRepository.UpdateItemStatus(orderItemId, OrderFormItemStatus.Auditing, db, trans);
                }
                else
                {
                    OrderFormRepository.UpdateItemStatus(orderItemId, OrderFormItemStatus.Dispatching, db, trans);
                }
            }
        }

        private static string CreateInspectionAudit()
        {
            //TODO:
            return string.Empty;
        }

        private static string SaveIncomingForm(InspectionFormEntity entity, int scanCount, string userId, Database db, DbTransaction trans)
        {
            var incoming = new IncomingFormEntity
            {
                Id = Guid.NewGuid().ToString(),
                IncomingCount = scanCount,
                OrderId = entity.OrderId,
                OrderFormNo = entity.OrderFormNo,
                OrderDetailId = entity.OrderDetailId,
                HospitalId = entity.HospitalId,
                ApplyUnitId = entity.ApplyUnitId,
                VendorId = entity.VendorId,
                ProductId = entity.ProductId,
                IsConfirmed = entity.ScanOver,
                CreatedId = userId,
                CreatedTime = DateTime.Now
            };

            IncomingFormRepository.Create(incoming, db, trans);

            return incoming.Id;
        }
        #endregion
    }
}
