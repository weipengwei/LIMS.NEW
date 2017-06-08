using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

using LIMS.Util;
using LIMS.Entities;

namespace LIMS.Repositories
{
    public static class ReceiveFormRepository
    {
        private const string COLUMN_SQL = @"
id,order_id,order_form_no,order_detail_id,apply_unit_id,serial_id,product_id,received_count,
hospital_id,vendor_id,is_confirmed,confirmed_id,confirmed_time,created_id,created_time";

        #region Get
        public static ReceiveFormEntity Get(string id)
        {
            return Get(id, null, null);
        }

        public static IList<ReceiveFormEntity> GetReceives(int formNo,string hospitalID)
        {
            var sql = string.Format("select {0} from receive_form where order_form_no=@p_form_no and hospital_id=@p_hospital_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_form_no", DbType.Int32, formNo);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalID);

            var list = new List<ReceiveFormEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new ReceiveFormEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<ReceiveFormItemEntity> GetItems(int formNo, string hospitalID)
        {
            var sql = @"
select a.*
from receive_form_items a inner join receive_form b on a.receive_id = b.id
where b.order_form_no =@p_form_no and b.hospital_id=@p_hospital_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_form_no", DbType.Int32, formNo);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalID);

            var list = new List<ReceiveFormItemEntity>();
            using (var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new ReceiveFormItemEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        private static ReceiveFormEntity Get(string id, Database db, DbTransaction trans)
        {
            var sql = string.Format("select {0} from receive_form where id=@p_id", COLUMN_SQL);

            if (db == null)
            {
                db = DatabaseFactory.CreateDatabase();
            }

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);

            ReceiveFormEntity entity = null;
            using (var reader = (trans == null ? db.ExecuteReader(dc) : db.ExecuteReader(dc, trans)))
            {
                while (reader.Read())
                {
                    entity = new ReceiveFormEntity();
                    entity.Init(reader);

                    break;
                }
            }

            return entity;
        }
        #endregion


        public static ReceiveFormEntity GetBySerialId(string serialId)
        {
            var sql = string.Format("select {0} from receive_form where serial_id=@p_serial_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_serial_id", DbType.String, serialId);
            using (var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    var entity = new ReceiveFormEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        internal static void Create(ReceiveFormEntity entity, Database db, DbTransaction trans)
        {
            var sql = @"insert into receive_form
(
id,order_id,order_form_no,order_detail_id,apply_unit_id,serial_id,product_id,received_count,hospital_id,vendor_id,is_confirmed,created_id,created_time
)
values(
@p_id,@p_order_id,@p_order_form_no,@p_order_detail_id,@p_apply_unit_id,@p_serial_id,@p_product_id,@p_received_count,
@p_hospital_id,@p_vendor_id,@p_is_confirmed,@p_created_id,@p_created_time
)";

            var dc = db.GetSqlStringCommand(sql);

            entity.Id = Guid.NewGuid().ToString();

            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);
            db.AddInParameter(dc, "p_order_id", DbType.String, entity.OrderId);
            db.AddInParameter(dc, "p_order_form_no", DbType.Int32, entity.OrderFormNo);
            db.AddInParameter(dc, "p_order_detail_id", DbType.String, entity.OrderDetailId);
            db.AddInParameter(dc, "p_apply_unit_id", DbType.String, entity.ApplyUnitId);
            db.AddInParameter(dc, "p_serial_id", DbType.String, entity.SerialId);
            db.AddInParameter(dc, "p_product_id", DbType.String, entity.ProductId);
            db.AddInParameter(dc, "p_received_count", DbType.Int32, entity.ReceivedCount);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, entity.HospitalId);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, entity.VendorId);
            db.AddInParameter(dc, "p_is_confirmed", DbType.Boolean, entity.IsConfirmed);
            db.AddInParameter(dc, "p_created_id", DbType.String, entity.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, entity.CreatedTime);

            db.ExecuteNonQuery(dc, trans);

            GoodsSerialFormRepository.FlowNextForm(new GoodsSerialFormEntity
            {
                SerialId = entity.SerialId,
                FormId = entity.Id,
                FormKind = FormKind.Receive,
                CreatedId = entity.CreatedId,
                CreatedTime = entity.CreatedTime
            }, db, trans);
        }

        #region Pass
        public static void Pass(ReceiveFormEntity form, Database db, DbTransaction trans)
        {
            Update(form, db, trans);

            //TODO: need to add inspection form
            CreateIncomingForm(form, db, trans);
        }
        public static void Pass(ReceiveFormEntity form)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        Update(form, db, trans);

                        //TODO: need to add inspection form
                        CreateIncomingForm(form, db, trans);

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

        private static void Update(ReceiveFormEntity form, Database db, DbTransaction trans)
        {
            var sql = @"update receive_form set is_confirmed=@p_is_confirmed,confirmed_id=@p_confirmed_id,confirmed_time=@p_confirmed_time where id=@p_id";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, form.Id);
            db.AddInParameter(dc, "p_is_confirmed", DbType.String, form.IsConfirmed);
            db.AddInParameter(dc, "p_confirmed_id", DbType.String, form.ConfirmedId);
            db.AddInParameter(dc, "p_confirmed_time", DbType.DateTime, form.ConfirmedTime.Equals(DBNull.Value) ? DateTime.Now : form.ConfirmedTime);


            db.ExecuteNonQuery(dc, trans);
        }
        
        //private static string CreateInspectionForm(ReceiveFormEntity entity, Database db, DbTransaction trans)
        //{
        //    var inspection = new InspectionFormEntity()
        //    {
        //        Id = Guid.NewGuid().ToString(),
        //        ReceiveCount = scanCount,
        //        InspectionCount = 0,
        //        ReceiveForm = entity.Id,
        //        OrderId = entity.OrderId,
        //        OrderFormNo = entity.OrderFormNo,
        //        OrderDetailId = entity.OrderDetailId,
        //        HospitalId = entity.HospitalId,
        //        ApplyUnitId = entity.ApplyUnitId,
        //        VendorId = entity.VendorId,
        //        ProductId = entity.ProductId,
        //        ScanOver = false,
        //        CreatedId = userId,
        //        CreatedTime = DateTime.Now,
        //        UpdatedId = userId,
        //        UpdatedTime = DateTime.Now
        //    };

        //    InspectionFormRepository.Save(inspection, db, trans);

        //    return inspection.Id;
        //}

        private static string CreateIncomingForm(ReceiveFormEntity form, Database db, DbTransaction trans)
        {
            var incoming = new IncomingFormEntity
            {
                OrderId = form.OrderId,
                OrderFormNo = form.OrderFormNo,
                OrderDetailId = form.OrderDetailId,
                ApplyUnitId = form.ApplyUnitId,
                SerialId = form.SerialId,
                ProductId = form.ProductId,
                IncomingCount = form.ReceivedCount - SumPassed(form.Id),
                HospitalId = form.HospitalId,
                VendorId = form.VendorId,
                IsConfirmed = false,
                CreatedId = form.ConfirmedId,
                CreatedTime = form.ConfirmedTime
            };

            IncomingFormRepository.Create(incoming, db, trans);

            return incoming.Id;
        }
        #endregion



        #region Create 
        public static string PartPass(ReceiveFormEntity form, IList<string> barcodes, string userId , Database db, DbTransaction trans)
        {
            var serialId = form.SerialId;
            var passedCount = SumPassed(form.Id);
            if (form.ReceivedCount == passedCount)
            {
                return serialId;
            }

            if (form.ReceivedCount == passedCount + barcodes.Count)
            {
                Pass(form);
                return serialId;
            }

            var goodsSerial = GoodsSerialRepository.Get(form.SerialId);

            var formItem = new ReceiveFormItemEntity
            {
                Id = Guid.NewGuid().ToString(),
                ReceiveId = form.Id,
                ReceivedCount = barcodes.Count,
                ConfirmedId = userId,
                ConfirmedTime = DateTime.Now
            };

            formItem.SerialId = CreateGoodsSerial(goodsSerial, formItem, barcodes, db, trans);
            CreateItem(formItem, db, trans);

            //TODO: need to add inspection form
            CreateIncomingForm(form, formItem, db, trans);

            serialId = formItem.SerialId;
            return serialId;
        }
        public static string PartPass(ReceiveFormEntity form, IList<string> barcodes, string userId)
        {
            var serialId = form.SerialId;
            var passedCount = SumPassed(form.Id);
            if(form.ReceivedCount == passedCount)
            {
                return serialId;
            }

            if(form.ReceivedCount == passedCount + barcodes.Count)
            {
                Pass(form);
                return serialId;
            }

            var goodsSerial = GoodsSerialRepository.Get(form.SerialId);

            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var formItem = new ReceiveFormItemEntity
                        {
                            Id = Guid.NewGuid().ToString(),
                            ReceiveId = form.Id,
                            ReceivedCount = barcodes.Count,
                            ConfirmedId = userId,
                            ConfirmedTime = DateTime.Now
                        };

                        formItem.SerialId = CreateGoodsSerial(goodsSerial, formItem, barcodes, db, trans);
                        CreateItem(formItem, db, trans);

                        //TODO: need to add inspection form
                        CreateIncomingForm(form, formItem, db, trans);

                        trans.Commit();
                        serialId = formItem.SerialId;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            return serialId;
        }

        public static int SumPassed(string receiveId)
        {
            var sql = "select sum(received_count) from receive_form_items where receive_id=@p_receive_id";
            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_receive_id", DbType.String, receiveId);

            using(var reader = db.ExecuteReader(cmd))
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

        private static void CreateItem(ReceiveFormItemEntity item, Database db, DbTransaction trans)
        {
            var sql = @"insert into receive_form_items(
id,receive_id,serial_id,received_count,confirmed_id,confirmed_time
)
values(
@p_id,@p_receive_id,@p_serial_id,@p_received_count,@p_confirmed_id,@p_confirmed_time
)";

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, item.Id);
            db.AddInParameter(cmd, "p_receive_id", DbType.String, item.ReceiveId);
            db.AddInParameter(cmd, "p_serial_id", DbType.String, item.SerialId);
            db.AddInParameter(cmd, "p_received_count", DbType.Int32, item.ReceivedCount);
            db.AddInParameter(cmd, "p_confirmed_id", DbType.String, item.ConfirmedId);
            db.AddInParameter(cmd, "p_confirmed_time", DbType.DateTime, item.ConfirmedTime);

            db.ExecuteNonQuery(cmd, trans);
        }

        private static string CreateGoodsSerial(GoodsSerialEntity parent, ReceiveFormItemEntity formItem, IList<string> barcodes, Database db, DbTransaction trans)
        {
            var goodsSerial = GoodsSerialRepository.CreateSub(parent, barcodes, formItem.ConfirmedId, db, trans);

            GoodsSerialFormRepository.Create(new GoodsSerialFormEntity
            {
                SerialId = goodsSerial.Id,
                FormId = formItem.Id,
                FormKind = FormKind.ReceiveItem,
                CreatedId = formItem.ConfirmedId,
                CreatedTime = formItem.ConfirmedTime,
            }, db, trans);

            return goodsSerial.Id;
        }

        private static string CreateIncomingForm(ReceiveFormEntity form, ReceiveFormItemEntity formItem, Database db, DbTransaction trans)
        {
            var incoming = new IncomingFormEntity
            {
                OrderId = form.OrderId,
                OrderFormNo = form.OrderFormNo,
                OrderDetailId = form.OrderDetailId,
                ApplyUnitId = form.ApplyUnitId,
                SerialId = formItem.SerialId,
                ProductId = form.ProductId,
                IncomingCount = formItem.ReceivedCount,
                HospitalId = form.HospitalId,
                VendorId = form.VendorId,
                IsConfirmed = false,
                CreatedId = formItem.ConfirmedId,
                CreatedTime = formItem.ConfirmedTime
            };

            IncomingFormRepository.Create(incoming, db, trans);

            return incoming.Id;
        }
        #endregion

        //        public static IList<ReceiveFormEntity> QueryConfirm(string hospitalId)
        //        {
        //            var sql = string.Format(@"select {0} from receive_form where scan_over = 0 and hospital_id=@p_hospital_id
        //order by order_form_no", COLUMN_SQL);

        //            var db = DatabaseFactory.CreateDatabase();
        //            var dc = db.GetSqlStringCommand(sql);
        //            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

        //            var list = new List<ReceiveFormEntity>();
        //            using (var reader = db.ExecuteReader(dc))
        //            {
        //                while (reader.Read())
        //                {
        //                    var entity = new ReceiveFormEntity();
        //                    entity.Init(reader);

        //                    list.Add(entity);
        //                }
        //            }

        //            return list;
        //        }



        #region Confirm
        //        public static void Confirm(string id, string userId)
        //        {
        //            var db = DatabaseFactory.CreateDatabase();

        //            using (var conn = db.CreateConnection())
        //            {
        //                conn.Open();
        //                using (var trans = conn.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        var entity = Get(id, db, trans);
        //                        if (entity == null)
        //                        {
        //                            throw new Exception("The receive form does not exist.");
        //                        }

        //                        var scanCount = GoodsStateRepository.CountValid(id, FormType.Receive);

        //                        UpdateScanStatus(id, scanCount, userId, db, trans);

        //                        string futureFormId;
        //                        FormType futureFormType;
        //                        if(OrderRepository.NeedMakeInspection(entity.OrderDetailId))
        //                        {
        //                            futureFormId = SaveInspectionForm(entity, scanCount, userId, db, trans);
        //                            futureFormType = FormType.Inspection;
        //                        }
        //                        else
        //                        {
        //                            futureFormId = SaveIncomingForm(entity, scanCount, userId, db, trans);
        //                            futureFormType = FormType.Incoming;
        //                        }

        //                        GoodsStateRepository.ChangeState(id, FormType.Receive, futureFormId, futureFormType, userId, db, trans);

        //                        //TODO: handle the status of order form
        //                        //UpdateOrderStatus(entity.OrderDetailId, db, trans);

        //                        trans.Commit();
        //                    }
        //                    catch
        //                    {
        //                        trans.Rollback();
        //                        throw;
        //                    }
        //                }
        //            }
        //        }

        //        private static void UpdateScanStatus(string id, int scanCount, string userId, Database db, DbTransaction trans)
        //        {
        //            var sql = @"update receive_form set scan_over=1, receive_count=@p_receive_count, 
        //confirm_user_id=@p_confirm_user_id, confirm_datetime = @p_confirm_datetime
        //where id=@p_id";
        //            var dc = db.GetSqlStringCommand(sql);
        //            db.AddInParameter(dc, "p_id", DbType.String, id);
        //            db.AddInParameter(dc, "p_receive_count", DbType.String, scanCount);
        //            db.AddInParameter(dc, "p_confirm_user_id", DbType.String, userId);
        //            db.AddInParameter(dc, "p_confirm_datetime", DbType.String, DateTime.Now);

        //            db.ExecuteNonQuery(dc, trans);
        //        }

        //        private static string SaveInspectionForm(ReceiveFormEntity entity, int scanCount, string userId, Database db, DbTransaction trans)
        //        {
        //            var inspection = new InspectionFormEntity()
        //            {
        //                Id = Guid.NewGuid().ToString(),
        //                ReceiveCount = scanCount,
        //                InspectionCount = 0,
        //                ReceiveForm = entity.Id,
        //                OrderId = entity.OrderId,
        //                OrderFormNo = entity.OrderFormNo,
        //                OrderDetailId = entity.OrderDetailId,
        //                HospitalId = entity.HospitalId,
        //                ApplyUnitId = entity.ApplyUnitId,
        //                VendorId = entity.VendorId,
        //                ProductId = entity.ProductId,
        //                ScanOver = false,
        //                CreatedId = userId,
        //                CreatedTime = DateTime.Now,
        //                UpdatedId = userId,
        //                UpdatedTime = DateTime.Now
        //            };

        //            InspectionFormRepository.Save(inspection, db, trans);

        //            return inspection.Id;
        //        }

        //        private static string SaveIncomingForm(ReceiveFormEntity entity, int scanCount, string userId, Database db, DbTransaction trans)
        //        {
        //            var incoming = new IncomingFormEntity
        //            {
        //                Id = Guid.NewGuid().ToString(),
        //                IncomingCount = scanCount,
        //                OrderId = entity.OrderId,
        //                OrderFormNo = entity.OrderFormNo,
        //                OrderDetailId = entity.OrderDetailId,
        //                HospitalId = entity.HospitalId,
        //                ApplyUnitId = entity.ApplyUnitId,
        //                VendorId = entity.VendorId,
        //                ProductId = entity.ProductId,
        //                ScanOver = false,
        //                CreatedId = userId,
        //                CreatedTime = DateTime.Now,
        //                UpdatedId = userId,
        //                UpdatedTime = DateTime.Now
        //            };

        //            IncomingFormRepository.Save(incoming, db, trans);

        //            return incoming.Id;
        //        }

        //        private static void UpdateOrderStatus(string orderDetailId, Database db, DbTransaction trans)
        //        {
        //            var sql = "select sum(receive_count) from receive_form where order_detail_id=@p_order_detail_id";
        //            var dc = db.GetSqlStringCommand(sql);
        //            db.AddInParameter(dc, "p_order_detail_id", DbType.String, orderDetailId);

        //            var count = 0;
        //            using (var reader = db.ExecuteReader(dc, trans))
        //            {
        //                reader.Read();

        //                count = Convert.ToInt32(reader[0]);
        //            }

        //            var detailEntity = OrderRepository.GetDetail(orderDetailId, db, trans);
        //            if (detailEntity != null && detailEntity.Count == count)
        //            {
        //                OrderRepository.UpdateDetailStatus(orderDetailId, OrderDetailStatus.RECEIVED, db, trans);
        //            }
        //        }
        #endregion
    }
}
