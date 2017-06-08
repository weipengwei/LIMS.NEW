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
    public class IncomingFormRepository
    {
        private const string COLUMN_SQL = @"
id,order_id,order_form_no,order_detail_id,apply_unit_id,serial_id,product_id,incoming_count,storeroom_id,
hospital_id,vendor_id,is_confirmed,confirmed_id,confirmed_time,created_id,created_time";

        internal static void Create(IncomingFormEntity entity, Database db, DbTransaction trans)
        {
            var sql = @"insert into incoming_form
(
id,order_id,order_form_no,order_detail_id,apply_unit_id,serial_id,product_id,incoming_count,
hospital_id,vendor_id,is_confirmed,created_id,created_time
)
values(
@p_id,@p_order_id,@p_order_form_no,@p_order_detail_id,@p_apply_unit_id,@p_serial_id,@p_product_id,@p_incoming_count,
@p_hospital_id,@p_vendor_id,@p_is_confirmed,@p_created_id,@p_created_time
)";
            entity.Id = Guid.NewGuid().ToString();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);
            db.AddInParameter(dc, "p_order_id", DbType.String, entity.OrderId);
            db.AddInParameter(dc, "p_order_form_no", DbType.Int32, entity.OrderFormNo);
            db.AddInParameter(dc, "p_order_detail_id", DbType.String, entity.OrderDetailId);
            db.AddInParameter(dc, "p_apply_unit_id", DbType.String, entity.ApplyUnitId);
            db.AddInParameter(dc, "p_serial_id", DbType.String, entity.SerialId);
            db.AddInParameter(dc, "p_product_id", DbType.String, entity.ProductId);
            db.AddInParameter(dc, "p_incoming_count", DbType.Int32, entity.IncomingCount);
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
                FormKind = FormKind.Incoming,
                CreatedId = entity.CreatedId,
                CreatedTime = entity.CreatedTime
            }, db, trans);
        }

        public static IncomingFormEntity GetBySerialId(string serialId, Database db, DbTransaction trans)
        {
            var sql = string.Format("select {0} from incoming_form where serial_id=@p_serial_id", COLUMN_SQL);

            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_serial_id", DbType.String, serialId);
            using (var reader = db.ExecuteReader(cmd,trans))
            {
                if (reader.Read())
                {
                    var entity = new IncomingFormEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static IncomingFormEntity GetBySerialId(string serialId)
        {
            var sql = string.Format("select {0} from incoming_form where serial_id=@p_serial_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_serial_id", DbType.String, serialId);
            using (var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    var entity = new IncomingFormEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        //internal static void Pass(IncomingFormEntity form, Database db, DbTransaction trans)
        //{
        //    Update(form, db, trans);
        //    CreateInventory(form, db, trans);
        //}
        public static void Pass(IncomingFormEntity form, Database db, DbTransaction trans)
        {
            Update(form, db, trans);
            CreateInventory(form, db, trans);
            UpdateOrderStatus(form.Id, db, trans);
        }

        public static void Pass(IncomingFormEntity form)
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
                        CreateInventory(form, db, trans);

                        UpdateOrderStatus(form.Id, db, trans);

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

        private static void UpdateOrderStatus(string incomingId, Database db, DbTransaction trans)
        {
            var cmd = db.GetStoredProcCommand("dbo.sp_update_order_status", incomingId);

            db.ExecuteNonQuery(cmd, trans);
        }

        private static void Update(IncomingFormEntity form, Database db, DbTransaction trans)
        {
            var sql = @"update incoming_form set storeroom_id=@p_storeroom_id,is_confirmed=@p_is_confirmed,confirmed_id=@p_confirmed_id,confirmed_time=@p_confirmed_time where id=@p_id";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, form.Id);
            db.AddInParameter(dc, "p_storeroom_id", DbType.String, form.StoreroomId);
            db.AddInParameter(dc, "p_is_confirmed", DbType.String, form.IsConfirmed);
            db.AddInParameter(dc, "p_confirmed_id", DbType.String, form.ConfirmedId);
            db.AddInParameter(dc, "p_confirmed_time", DbType.String, form.ConfirmedTime);

            db.ExecuteNonQuery(dc, trans);
        }

        private static void CreateInventory(IncomingFormEntity form, Database db, DbTransaction trans)
        {
            var goodsSerial = GoodsSerialRepository.Get(form.SerialId, db, trans);
            var barcodes = GoodsSerialRepository.GetBarcodes(form.SerialId, db, trans);
            var product = ProductRepository.Get(goodsSerial.ProductId);

            var inventory = new GoodsInventoryEntity
            {
                SerialId = goodsSerial.Id,
                BatchNo = goodsSerial.BatchNo,
                ProductId = goodsSerial.ProductId,
                StoreroomId = form.StoreroomId,
                ExpiredDate = goodsSerial.ExpiredDate.Value,
                HospitalId = goodsSerial.HospitalId,
                VendorId = goodsSerial.VendorId,
                OriginalCount = form.IncomingCount,
                SplitCount = 0,
                UsableCount = form.IncomingCount, //goodsSerial.NeedSplit ? 0 : form.IncomingCount,
                GrantedCount = 0,
                CreatedId = form.ConfirmedId,
                CreatedTime = form.ConfirmedTime
            };
            inventory.ApplyCount = inventory.UsableCount * product.MiniPackageCount;
            GoodsInventoryRepository.Create(inventory, db, trans);

            AllocateBarcodes(barcodes, inventory, product, db, trans);
        }

        private static void AllocateBarcodes(IList<GoodsSerialBarcodeEntity> barcodes, GoodsInventoryEntity inventory, ProductEntity product, Database db, DbTransaction trans)
        {
            foreach(var item in barcodes)
            {
                var goods = new GoodsEntity();
                goods.Name = product.Name;
                goods.Barcode = item.Barcode;
                goods.SerialId = item.SerialId;
                goods.HospitalId = inventory.HospitalId;
                goods.VendorId = inventory.VendorId;
                goods.ProductId = inventory.ProductId;
                goods.BatchNo = inventory.BatchNo;
                goods.ExpiredDate = inventory.ExpiredDate;
                goods.StoreroomId = inventory.StoreroomId;
                goods.PackageCapacity = product.MiniPackageSpec;
                goods.PackageCount = product.MiniPackageCount;
                goods.MeasuringUnit = product.PackageUnit;
                goods.Status = GoodsStatus.Usable;
                goods.CreatedId = inventory.CreatedId;
                goods.CreatedTime = inventory.CreatedTime;

                GoodsRepsitory.Create(goods, db, trans);
            }
        }

//        private static void CreateDetails(IncomingFormEntity form, IList<IncomingDetailEntity> details, Database db, DbTransaction trans)
//        {
//            var sql = @"insert into 
//incoming_detail(id,incoming_id,storeroom_id,count)
//values(@p_id,@p_incoming_id,@p_storeroom_id,@p_count)";

        //            var dc = db.GetSqlStringCommand(sql);

        //            foreach (var item in details)
        //            {
        //                item.Id = Guid.NewGuid().ToString();
        //                item.IncomingId = form.Id;

        //                db.AddInParameter(dc, "p_id", DbType.String, item.Id);
        //                db.AddInParameter(dc, "p_incoming_id", DbType.String, item.IncomingId);
        //                db.AddInParameter(dc, "p_storeroom_id", DbType.String, item.StoreroomId);
        //                db.AddInParameter(dc, "p_count", DbType.Int32, item.Count);

        //                db.ExecuteNonQuery(dc, trans);
        //            }
        //        }








//        public static IList<IncomingFormEntity> QueryConfirm(string hospitalId)
//        {
//            var sql = string.Format(@"select {0} from incoming_form where scan_over = 0 and hospital_id=@p_hospital_id
//order by order_form_no", COLUMN_SQL);

//            var db = DatabaseFactory.CreateDatabase();
//            var dc = db.GetSqlStringCommand(sql);
//            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

//            var list = new List<IncomingFormEntity>();
//            using (var reader = db.ExecuteReader(dc))
//            {
//                while (reader.Read())
//                {
//                    var entity = new IncomingFormEntity();
//                    entity.Init(reader);

//                    list.Add(entity);
//                }
//            }

//            return list;
//        }
        
        public static IncomingFormEntity Get(string id)
        {
            return Get(id, null, null);
        }

        private static IncomingFormEntity Get(string id, Database db, DbTransaction trans)
        {
            var sql = string.Format("select {0} from incoming_form {1} where id=@p_id", COLUMN_SQL, TransHelper.UpdateLock(trans));

            if (db == null)
            {
                db = DatabaseFactory.CreateDatabase();
            }

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);

            IncomingFormEntity entity = null;
            using (var reader = (trans == null ? db.ExecuteReader(dc) : db.ExecuteReader(dc, trans)))
            {
                while (reader.Read())
                {
                    entity = new IncomingFormEntity();
                    entity.Init(reader);

                    break;
                }
            }

            return entity;
        }


        #region Confirm
        //public static void Confirm(string id, string storeroomId, string userId)
        //{
        //    var db = DatabaseFactory.CreateDatabase();

        //    using (var conn = db.CreateConnection())
        //    {
        //        conn.Open();
        //        using (var trans = conn.BeginTransaction())
        //        {
        //            try
        //            {
        //                var entity = Get(id, db, trans);
        //                if (entity == null)
        //                {
        //                    throw new Exception("The incoming form does not exist.");
        //                }
        //                if(!entity.IsConfirmed)
        //                {
        //                    var scanCount = GoodsStateRepository.CountValid(id, FormType.Incoming);
        //                    entity.IncomingCount = scanCount;

        //                    UpdateConfirmStatus(id, scanCount, storeroomId, userId, db, trans);

        //                    var goods = GetGoods(entity, userId);
        //                    var goodsExtra = GetGoodsExtra(entity);
        //                    GoodsRepsitory.Create(entity.Id, goods, goodsExtra, entity.IncomingCount, userId, db, trans);

        //                    GoodsStateRepository.ChangeState(id, FormType.Incoming, userId, db, trans);

        //                    //UpdateOrderStatus(entity.OrderDetailId, db, trans);
        //                }

        //                trans.Commit();
        //            }
        //            catch
        //            {
        //                trans.Rollback();
        //                throw;
        //            }
        //        }
        //    }
        //}

//        private static void UpdateConfirmStatus(string id, int scanCount, string storeroomId, string userId, Database db, DbTransaction trans)
//        {
//            var sql = @"update incoming_form set scan_over=1, incoming_count=@p_incoming_count, storeroom_id=@p_storeroom_id,
//confirm_user_id=@p_confirm_user_id, confirm_datetime = @p_confirm_datetime
//where id=@p_id";
//            var dc = db.GetSqlStringCommand(sql);
//            db.AddInParameter(dc, "p_id", DbType.String, id);
//            db.AddInParameter(dc, "p_incoming_count", DbType.String, scanCount);
//            db.AddInParameter(dc, "p_storeroom_id", DbType.String, storeroomId);
//            db.AddInParameter(dc, "p_confirm_user_id", DbType.String, userId);
//            db.AddInParameter(dc, "p_confirm_datetime", DbType.String, DateTime.Now);

//            db.ExecuteNonQuery(dc, trans);
//        }

//        private static void UpdateOrderStatus(string orderDetailId, Database db, DbTransaction trans)
//        {
//            var sql = "select sum(incoming_count) from incoming_form where order_detail_id=@p_order_detail_id";
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
//                OrderRepository.UpdateDetailStatus(orderDetailId, OrderDetailStatus.COMPLETED, db, trans);
//            }
//        }

//        private static GoodsEntity GetGoods(IncomingFormEntity entity, string userId)
//        {
//            var product = ProductRepository.Get(entity.ProductId);
//            if (product == null)
//            {
//                throw new Exception("The product does not exist.");
//            }

//            var goods = new GoodsEntity();
//            goods.Name = product.Name;
//            goods.ProductId = entity.ProductId;
//            goods.HospitalId = entity.HospitalId;
//            //goods.RemainingCapacity = product.MiniPackageCount;
//            goods.Status = GoodsStatus.Usable;
//            goods.PackageCapacity = product.MiniPackageSpec;
//            goods.PackageCount = product.MiniPackageCount;
//            goods.MeasuringUnit = product.PackageUnit;
//            goods.CreatedId = goods.UpdatedId = userId;
//            goods.CreatedTime = goods.UpdatedTime = DateTime.Now;

//            return goods;
//        }

        //private static GoodsExtraEntity GetGoodsExtra(IncomingFormEntity entity)
        //{
        //    var orderDetail = OrderRepository.GetDetail(entity.OrderDetailId);

        //    var goodsExtra = new GoodsExtraEntity();
        //    goodsExtra.NeedCheck = orderDetail.NeedCheck;
        //    goodsExtra.NeedSplit = orderDetail.NeedSplit;
        //    goodsExtra.SplitCapacity = orderDetail.SplitCapacity;
        //    goodsExtra.MiniSplitNumber = orderDetail.MiniSplitNumber;
        //    goodsExtra.ValidDays = orderDetail.ValidDays;
        //    goodsExtra.ExpiredDate = orderDetail.ExpiredDate;
        //    goodsExtra.SplitUnit = orderDetail.SplitUnit;
        //    goodsExtra.SplitCopies = orderDetail.SplitCopies;

        //    return goodsExtra;
        //}
        #endregion
    }
}
