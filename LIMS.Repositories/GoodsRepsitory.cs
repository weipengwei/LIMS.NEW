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
    public static class GoodsRepsitory
    {
        private const string COLUMN_SQL = @"
id,name,barcode,serial_id,parent_id,hospital_id,vendor_id,product_id,batch_no,expired_date,
storeroom_id,package_capacity,package_count,measuring_unit,status,granted_count,
split_user_id,split_date,created_id,created_time,updated_id,updated_time";

        internal static void Create(GoodsEntity goods, Database db, DbTransaction trans)
        {
            var sql = @"
insert into goods
(
    id,name,parent_id,barcode,serial_id,hospital_id,vendor_id,product_id,batch_no,expired_date,
    storeroom_id,package_capacity,package_count,measuring_unit,status,created_id,created_time
)
values(
    @p_id,@p_name,@p_parent_id,@p_barcode,@p_serial_id,@p_hospital_id,@p_vendor_id,@p_product_id,@p_batch_no,@p_expired_date,@p_storeroom_id,
    @p_package_capacity,@p_package_count,@p_measuring_unit,@p_status,@p_created_id,@p_created_time
)";
            goods.Id = Guid.NewGuid().ToString();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, goods.Id);
            db.AddInParameter(dc, "p_name", DbType.String, goods.Name);
            db.AddInParameter(dc, "p_parent_id", DbType.String, goods.ParentId);
            db.AddInParameter(dc, "p_barcode", DbType.String, goods.Barcode);
            db.AddInParameter(dc, "p_serial_id", DbType.String, goods.SerialId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, goods.HospitalId);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, goods.VendorId);
            db.AddInParameter(dc, "p_product_id", DbType.String, goods.ProductId);
            db.AddInParameter(dc, "p_batch_no", DbType.String, goods.BatchNo);
            db.AddInParameter(dc, "p_expired_date", DbType.DateTime, goods.ExpiredDate);
            db.AddInParameter(dc, "p_storeroom_id", DbType.String, goods.StoreroomId);
            db.AddInParameter(dc, "p_package_capacity", DbType.String, goods.PackageCapacity);
            db.AddInParameter(dc, "p_package_count", DbType.Int32, goods.PackageCount);
            db.AddInParameter(dc, "p_measuring_unit", DbType.String, goods.MeasuringUnit);
            db.AddInParameter(dc, "p_status", DbType.Int32, GoodsStatus.Usable);
            db.AddInParameter(dc, "p_created_id", DbType.String, goods.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, goods.CreatedTime);

            db.ExecuteNonQuery(dc, trans);
        }
        
        public static GoodsEntity Get(string barcode, string hospitalId)
        {
            var sql = string.Format("select {0} from goods where barcode = @p_barcode and hospital_id = @p_hospital_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_barcode", DbType.String, barcode);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new GoodsEntity();
                    entity.Init(reader);

                    return entity;
                }

                return null;
            }
        }

        public static IList<GoodsEntity> GetByParent(string parentId, string hospitalId)
        {
            var sql = string.Format("select {0} from goods where parent_id = @p_parent_id and hospital_id = @p_hospital_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_parent_id", DbType.String, parentId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            var list = new List<GoodsEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new GoodsEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static GoodsEntity GetOneBySerial(string serialId, string hospitalId)
        {
            var sql = string.Format(@"
select top 1 {0} from goods 
where serial_id = @p_serial_id and hospital_id = @p_hospital_id 
and package_count > granted_count and status = @p_status", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_serial_id", DbType.String, serialId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(dc, "p_status", DbType.Int32, (int)GoodsStatus.Usable);

            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new GoodsEntity();
                    entity.Init(reader);

                    return entity;
                }

                return null;
            }
        }

        internal static void CreateRuntime(GoodsRuntimeEntity entity, Database db, DbTransaction trans)
        {
            var sql = @"insert into goods_runtime(id,hospital_id,apply_id,serial_id,barcode,count,product_id)
values(@p_id,@p_hospital_id,@p_apply_id,@p_serial_id,@p_barcode,@p_count,@p_product_id)";
            
            var cmd = db.GetSqlStringCommand(sql);

            entity.Id = Guid.NewGuid().ToString();

            db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, entity.HospitalId);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, entity.ApplyId);
            db.AddInParameter(cmd, "p_serial_id", DbType.String, entity.SerialId);
            db.AddInParameter(cmd, "p_barcode", DbType.String, entity.Barcode);
            db.AddInParameter(cmd, "p_count", DbType.String, entity.Count);
            db.AddInParameter(cmd, "p_product_id", DbType.String, entity.ProductId);

            db.ExecuteNonQuery(cmd, trans);
        }

        public static int SumRuntime(string barcode, string hospitalId)
        {
            var sql = "select sum(count) from goods_runtime where barcode=@p_barcode and hospital_id=@p_hospital_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_barcode", DbType.String, barcode);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);

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

        public static int SumRuntime(string barcode, string applyId, string hospitalId)
        {
            var sql = "select sum(count) from goods_runtime where barcode=@p_barcode and apply_id=@p_apply_id and hospital_id=@p_hospital_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_barcode", DbType.String, barcode);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, applyId);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);

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

        public static IList<GoodsRuntimeEntity> GetRuntime(string applyId)
        {
            var sql = @"select id,hospital_id,apply_id,serial_id,barcode,count,product_id from goods_runtime where apply_id=@p_apply_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, applyId);

            var list = new List<GoodsRuntimeEntity>();
            using(var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new GoodsRuntimeEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        internal static void DeleteRuntime(string applyId, string hospitalId, Database db, DbTransaction trans)
        {
            var sql = "delete goods_runtime where apply_id=@p_apply_id";

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, applyId);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);

            db.ExecuteNonQuery(cmd, trans);
        }

        public static void DeleteRuntime(string applyId, string barcode, string hospitalId, Database db, DbTransaction trans)
        {
            var sql = "delete goods_runtime where apply_id=@p_apply_id and hospital_id=@p_hospital_id and barcode=@p_barcode";

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, applyId);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(cmd, "p_barcode", DbType.String, barcode);

            db.ExecuteNonQuery(cmd, trans);
        }

        internal static void Reduce(string barcode, int count, string hospitalId, Database db, DbTransaction trans)
        {
            var sql = "update goods set granted_count+=@p_granted_count where barcode=@p_barcode and hospital_id=@p_hospital_id";
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_barcode", DbType.String, barcode);
            db.AddInParameter(cmd, "p_granted_count", DbType.Int32, count);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);

            db.ExecuteNonQuery(cmd, trans);
        }

      
        #region Create
        public static void Create(string incomingId, GoodsEntity goods, GoodsExtraEntity goodsExtra, int goodsCount, string userId, Database db, DbTransaction trans)
        {
            goods.SerialId = GoodsExtraRepsitory.Create(goodsExtra, db, trans);
            Create(incomingId, goods, userId, db, trans);

            GoodsInventoryRepository.AddOriginalCount(goods.ProductId, goods.HospitalId, goodsCount, db, trans);
            if(!goodsExtra.NeedSplit)
            {
                GoodsInventoryRepository.AddUsableCount(goods.ProductId, goods.HospitalId, goodsCount, db, trans);
            }
        }

        private static void Create(string incomingId, GoodsEntity goods, string userId, Database db, DbTransaction trans)
        {
            var sql = @"
insert into goods(
    id, name, extra_id, hospital_id, product_id, storeroom_id,
barcode, remaining_capacity, status, 
package_capacity, package_count, measuring_unit, 
created_id, created_time, updated_id, updated_time, 
expired_date, batch_no, vendor_id
)
select
	newid(), @p_name, @p_extra_id, @p_hospital_id, product_id, @p_storeroom_id,
    barcode, @p_remaining_capacity,@p_status, 
    @p_package_capacity, @p_package_count, @p_measuring_unit,
    @p_created_id, @p_created_time, @p_updated_id, @p_updated_time,
    expired_date, batch_no, vendor_id
from goods_state where future_form_id=@p_incoming_id and future_form_type=@p_form_type";
            
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_name", DbType.String, goods.Name);
            db.AddInParameter(dc, "p_extra_id", DbType.String, goods.SerialId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, goods.HospitalId);
            db.AddInParameter(dc, "p_storeroom_id", DbType.String, goods.StoreroomId);
            //db.AddInParameter(dc, "p_remaining_capacity", DbType.Decimal, goods.RemainingCapacity);
            db.AddInParameter(dc, "p_status", DbType.Int32, GoodsStatus.Usable);
            db.AddInParameter(dc, "p_package_capacity", DbType.String, goods.PackageCapacity);
            db.AddInParameter(dc, "p_package_count", DbType.Int32, goods.PackageCount);
            db.AddInParameter(dc, "p_measuring_unit", DbType.String, goods.MeasuringUnit);
            db.AddInParameter(dc, "p_created_id", DbType.String, userId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_updated_id", DbType.String, userId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_incoming_id", DbType.String, incomingId);
            db.AddInParameter(dc, "p_form_type", DbType.Int32, FormType.Incoming);

            db.ExecuteNonQuery(dc, trans);
        }
        #endregion

        #region Split
        public static IList<GoodsEntity> Split(GoodsEntity goods, GoodsSerialEntity goodsSerial, string userId)
        {
            var expiredDate = GetSplitExpiredDate(goodsSerial);
            var product = ProductRepository.Get(goods.ProductId);
            var hospitalProduct = HospitalProductRepository.GetOneProduct(goods.ProductId, goods.HospitalId);

            var list = new List<GoodsEntity>();

            var db = DatabaseFactory.CreateDatabase();
            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        if (UpdateSplittingUser(goods.Id, userId, db, trans))
                        {
                            var newSerial = NewGoodsSerial(goodsSerial, expiredDate, userId);
                            var barcodes = GoodsSerialRepository.Create(newSerial, db, trans);

                            var name = !string.IsNullOrEmpty(goods.Name) ? goods.Name : hospitalProduct.Alias;
                            if (string.IsNullOrEmpty(name))
                            {
                                name = product.Name;
                            }

                            if (string.IsNullOrEmpty(hospitalProduct.SplitCapacity))
                            {
                                name += "(" + hospitalProduct.SplitCopies + "-{0})";
                            }
                            else
                            {
                                name += "(" + hospitalProduct.MiniSplitNumber + "*" + hospitalProduct.SplitCapacity + hospitalProduct.SplitUnit + "," + hospitalProduct.SplitCopies + "-{0})";
                            }

                            var index = 0;
                            foreach (var barcode in barcodes)
                            {
                                index++;

                                var newGoods = new GoodsEntity
                                {
                                    Name = string.Format(name, index),
                                    Barcode = barcode,
                                    SerialId = newSerial.Id,
                                    ParentId = goods.Id,
                                    HospitalId = goods.HospitalId,
                                    VendorId = goods.VendorId,
                                    ProductId = goods.ProductId,
                                    BatchNo = goods.BatchNo,
                                    ExpiredDate = expiredDate,
                                    StoreroomId = goods.StoreroomId,
                                    PackageCapacity = goodsSerial.SplitCapacity,
                                    PackageCount = goodsSerial.SplitPackageCount,
                                    MeasuringUnit = goodsSerial.SplitUnit,
                                    Status = GoodsStatus.Usable,
                                    GrantedCount = 0,
                                    SplitUserId = userId,
                                    SplitDate = DateTime.Now,
                                    CreatedId = userId,
                                    CreatedTime = DateTime.Now,
                                    UpdatedId = userId,
                                    UpdatedTime = DateTime.Now
                                };

                                Create(newGoods, db, trans);

                                list.Add(newGoods);
                            }

                            AdjustInventory(goods, newSerial.Id, barcodes.Count, goodsSerial.SplitPackageCount, userId, db, trans);
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

            return list;
        }

        private static DateTime GetSplitExpiredDate(GoodsSerialEntity goodsSerial)
        {
            DateTime newExpiredDate;
            if (goodsSerial.ExpiredDate.HasValue && goodsSerial.ExpiredDate.Value != DateTime.MaxValue)
            {
                if(goodsSerial.ExpiredDate > DateTime.Today)
                {
                    newExpiredDate = DateTime.Today.AddDays((double)goodsSerial.ValidDays);
                    if (newExpiredDate > goodsSerial.ExpiredDate)
                    {
                        newExpiredDate = goodsSerial.ExpiredDate.Value;
                    }
                }
                else
                {
                    newExpiredDate = goodsSerial.ExpiredDate.Value;
                }
            }
            else
            {
                newExpiredDate = DateTime.MaxValue;
            }

            return newExpiredDate;
        }

        private static GoodsSerialEntity NewGoodsSerial(GoodsSerialEntity goodsSerial, DateTime expiredDate, string userId)
        {
            return new GoodsSerialEntity
            {
                SerialNo = goodsSerial.SerialNo,
                ProductId = goodsSerial.ProductId,
                DispatchedCount = goodsSerial.SplitCopies,
                HospitalId = goodsSerial.HospitalId,
                VendorId = goodsSerial.VendorId,
                NeedAudit = goodsSerial.NeedAudit,
                NeedCheck = goodsSerial.NeedCheck,
                NeedSplit = false,
                SplitCopies = 0,
                SplitUnit = string.Empty,
                SplitPackageCount = 0,
                SplitCapacity = string.Empty,
                ValidDays = 0,
                BatchNo = goodsSerial.BatchNo,
                ExpiredDate = expiredDate,
                LogisticsCode = goodsSerial.LogisticsCode,
                LogisticsContent = goodsSerial.LogisticsContent,
                IsClosed = false,
                CreatedId = userId,
                CreatedTime = DateTime.Now,
                UpdatedId = userId,
                UpdatedTime = DateTime.Now
            };
        }

        private static bool UpdateSplittingUser(string id, string userId, Database db, DbTransaction trans)
        {
            var sql = "update goods set split_user_id=@p_user_id,split_date=@p_split_date where id=@p_id and package_count > granted_count";

            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_id", DbType.String, id);
            db.AddInParameter(cmd, "p_user_id", DbType.String, userId);
            db.AddInParameter(cmd, "p_split_date", DbType.DateTime, DateTime.Now);

            return db.ExecuteNonQuery(cmd, trans) > 0;
        }
        
        private static void AdjustInventory(GoodsEntity goods, string serialId, int count, int packageCount, string userId, 
            Database db, DbTransaction trans)
        {
            var inventroy = new GoodsInventoryEntity
            {
                SerialId = serialId,
                BatchNo = goods.BatchNo,
                ProductId = goods.ProductId,
                StoreroomId = goods.StoreroomId,
                ExpiredDate = goods.ExpiredDate,
                HospitalId = goods.HospitalId,
                VendorId = goods.VendorId,
                OriginalCount = count,
                SplitCount = 0,
                UsableCount = count,
                ApplyCount = packageCount * count,
                GrantedCount = 0,
                CreatedId = userId,
                CreatedTime = DateTime.Now
            };
            GoodsInventoryRepository.Create(inventroy, db, trans);

            Reduce(goods.Barcode, 1, goods.HospitalId, db, trans);
            GoodsInventoryRepository.Reduce(goods.SerialId, goods.HospitalId, 1, db, trans);

            if(goods.PackageCount == goods.GrantedCount + 1)
            {
                Complete(goods.Id, db, trans);
            }
        }

        //        public static void Split(string barcode, string hospitalId, IList<string> children, string splitUnit, DateTime? expiredDate, string userId)
        //        {
        //            var goods = Get(barcode, hospitalId);
        //            if(goods == null)
        //            {
        //                throw new InvalidException(string.Format("扫描码({0})找不到对应的货品或者货品不属于本院!", barcode));
        //            }
        //            var goodsExtra = GoodsExtraRepsitory.Get(goods.SerialId);
        //            if(goodsExtra == null)
        //            {
        //                throw new InvalidException(string.Format("扫描码({0})对应的货品没有分装信息!", barcode));
        //            }
        //            var goodsState = GoodsStateRepository.GetByBarcode(barcode);
        //            if(goodsState == null)
        //            {
        //                throw new InvalidException(string.Format("扫描码({0})对应的货品没有状态信息!", barcode));
        //            }

        //            var db = DatabaseFactory.CreateDatabase();
        //            using (var conn = db.CreateConnection())
        //            {
        //                conn.Open();
        //                using (var trans = conn.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        var childExtra = GetGoodsExtra(goodsExtra, splitUnit);
        //                        var extraId = GoodsExtraRepsitory.Create(childExtra, db, trans);

        //                        foreach(var child in children)
        //                        {
        //                            var childBarcode = child.Substring(0, child.IndexOf(','));
        //                            var childState = GetGoodsState(goodsState, childBarcode, userId);
        //                            GoodsStateRepository.Create(childState, db, trans);

        //                            var childGoods = GetGoods(goods, goodsExtra, childBarcode, extraId, splitUnit, expiredDate, userId);
        //                            childGoods.Name = child.Substring(childBarcode.Length + 1);
        //                            Save(childGoods, userId, db, trans);
        //                        }

        //                        GoodsInventoryRepository.AddUsableCount(goods.ProductId, goods.HospitalId, children.Count, db, trans);
        //                        GoodsInventoryRepository.IncreaseSplit(goods.ProductId, goods.HospitalId, db, trans);
        //                        Complete(goods.Id, db, trans);

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

        //        private static void Save(GoodsEntity entity, string userId, Database db, DbTransaction trans)
        //        {
        //            var sql = string.Format(@"
        //insert into goods(
        //    {0}
        //)
        //values(
        //    @p_id,@p_name, @p_extra_id, @p_parent_id, @p_hospital_id, @p_product_id, @p_storeroom_id,
        //    @p_barcode, @p_barcode_date, null, @p_expired_date, @p_status,
        //    @p_package_capacity, @p_package_count, null, @p_measuring_unit, null, @p_split_user_id, @p_split_date,
        //    @p_created_id, @p_created_time, @p_updated_id, @p_updated_time, @p_batch_no, @p_vendor_id
        //)", COLUMN_SQL);

        //            var dc = db.GetSqlStringCommand(sql);

        //            entity.Id = Guid.NewGuid().ToString();

        //            db.AddInParameter(dc, "p_id", DbType.String, Guid.NewGuid().ToString());
        //            db.AddInParameter(dc, "p_name", DbType.String, entity.Name);
        //            db.AddInParameter(dc, "p_extra_id", DbType.String, entity.SerialId);
        //            db.AddInParameter(dc, "p_parent_id", DbType.String, entity.ParentId);
        //            db.AddInParameter(dc, "p_hospital_id", DbType.String, entity.HospitalId);
        //            db.AddInParameter(dc, "p_product_id", DbType.String, entity.ProductId);
        //            db.AddInParameter(dc, "p_storeroom_id", DbType.String, entity.StoreroomId);
        //            db.AddInParameter(dc, "p_barcode", DbType.String, entity.Barcode);
        //            db.AddInParameter(dc, "p_barcode_date", DbType.DateTime, DateTime.Now);
        //            db.AddInParameter(dc, "p_expired_date", DbType.DateTime, entity.ExpiredDate);
        //            db.AddInParameter(dc, "p_status", DbType.Int32, GoodsStatus.Usable);
        //            db.AddInParameter(dc, "p_package_capacity", DbType.Decimal, entity.PackageCapacity);
        //            db.AddInParameter(dc, "p_package_count", DbType.Int32, entity.PackageCount);
        //            db.AddInParameter(dc, "p_measuring_unit", DbType.String, entity.MeasuringUnit);
        //            db.AddInParameter(dc, "p_split_user_id", DbType.String, userId);
        //            db.AddInParameter(dc, "p_split_date", DbType.DateTime, DateTime.Now);
        //            db.AddInParameter(dc, "p_created_id", DbType.String, userId);
        //            db.AddInParameter(dc, "p_created_time", DbType.DateTime, DateTime.Now);
        //            db.AddInParameter(dc, "p_updated_id", DbType.String, userId);
        //            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, DateTime.Now);
        //            db.AddInParameter(dc, "p_batch_no", DbType.String, entity.BatchNo);
        //            db.AddInParameter(dc, "p_vendor_id", DbType.String, entity.VendorId);

        //            db.ExecuteNonQuery(dc, trans);
        //        }

        //        private static GoodsStateEntity GetGoodsState(GoodsStateEntity parent, string barcode, string userId)
        //        {
        //            return new GoodsStateEntity
        //            {
        //                Barcode = barcode,
        //                ProductId = parent.ProductId,
        //                OrderFormId = parent.OrderFormId,
        //                OrderFormNo = parent.OrderFormNo,
        //                FormId = parent.FormId,
        //                FormType = parent.FormType,
        //                StateCreatedUser = userId,
        //                StateCreateTime = DateTime.Now,
        //                StateValidateUser = userId,
        //                StateValidateTime = DateTime.Now,
        //                StateChangedUser = userId,
        //                StateChangedTime = DateTime.Now,
        //                VendorId = parent.VendorId
        //            };
        //        }

        //        private static GoodsExtraEntity GetGoodsExtra(GoodsExtraEntity parent, string splitUnit)
        //        {
        //            return new GoodsExtraEntity
        //            {
        //                NeedCheck = false,
        //                NeedSplit = false,
        //                SplitCapacity = "",
        //                MiniSplitNumber = 0,
        //                SplitCopies = 0,
        //                SplitUnit = ""
        //            };
        //        }

        //private static GoodsEntity GetGoods(GoodsEntity parent, GoodsExtraEntity parentExtra, string barcode, string extraId, string splitUnit, DateTime? expiredDate, string userId)
        //{
        //    return new GoodsEntity
        //    {
        //        SerialId = extraId,
        //        ParentId = parent.Id,
        //        HospitalId = parent.HospitalId,
        //        ProductId = parent.ProductId,
        //        Barcode = barcode,
        //        //BarcodeDate = DateTime.Now,
        //        Status = GoodsStatus.Usable,
        //        //ExpiredDate = expiredDate,
        //        StoreroomId = parent.StoreroomId,
        //        BatchNo = parent.BatchNo,
        //        PackageCapacity = parentExtra.SplitCapacity,
        //        PackageCount = parentExtra.MiniSplitNumber,
        //        MeasuringUnit = splitUnit,
        //        SplitUserId = userId,
        //        SplitDate = DateTime.Now,
        //        //StartSplitDate = DateTime.Now,
        //        CreatedId = userId,
        //        CreatedTime = DateTime.Now,
        //        UpdatedId = userId,
        //        UpdatedTime = DateTime.Now,
        //        VendorId = parent.VendorId
        //    };
        //}
        #endregion

        public static void MoveStoreroom(string storeroomId, string moveinId, string hospitalId, string userId, Database db, DbTransaction trans)
        {
            var sql = @"update goods set storeroom_id = @p_storeroom_id, updated_id = @p_user_id, updated_time = @p_datetime 
where barcode in (select barcode from goods_state where form_id=@p_form_id and form_type=@p_form_type) and hospital_id = @p_hospital_id";
            
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_storeroom_id", DbType.String, storeroomId);
            db.AddInParameter(dc, "p_user_id", DbType.String, userId);
            db.AddInParameter(dc, "p_datetime", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_form_id", DbType.String, moveinId);
            db.AddInParameter(dc, "p_form_type", DbType.Int32, FormType.MoveIn);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            db.ExecuteNonQuery(dc, trans);
        }

        private static void Complete(string id, Database db, DbTransaction trans)
        {
            var sql = "update goods set status = @p_status where id = @p_id";            
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_id", DbType.String, id);
            db.AddInParameter(dc, "p_status", DbType.Int32, (int)GoodsStatus.Closed);

            db.ExecuteNonQuery(dc, trans);
        }

        public static void CompleteByApplyForm(string applyId, Database db, DbTransaction trans)
        {
            var sql = @"
update a set a.status = @p_status from goods a 
join goods_state b on a.barcode = b.barcode
where b.form_id = @p_form_id and b.form_type = @p_from_type";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_status", DbType.Int32, (int)GoodsStatus.Closed);
            db.AddInParameter(dc, "p_form_id", DbType.String, applyId);
            db.AddInParameter(dc, "p_from_type", DbType.Int32, (int)FormType.Apply);

            db.ExecuteNonQuery(dc, trans);
        }


        #region Goods Check
        //        public static IList<ProductInventoryEntity> QueryProductInventory(ProductInventoryCondition condition)
        //        {
        //            var list = new List<ProductInventoryEntity>();
        //            SplittingProductInventory(condition, list);
        //            UsableProductInventory(condition, list);

        //            return list;
        //        }

        //        private static void SplittingProductInventory(ProductInventoryCondition condition, IList<ProductInventoryEntity> list)
        //        {
        //            var conditionSql = "";
        //            if (!string.IsNullOrEmpty(condition.StoreroomId))
        //            {
        //                conditionSql += "and a.storeroom_id=@p_storeroom_id";
        //            }
        //            if (!string.IsNullOrEmpty(condition.ProductId))
        //            {
        //                conditionSql += " and a.product_id=@p_product_id";
        //            }

        //            var sql = string.Format(@"select 
        //	a.storeroom_id,a.product_id,a.expired_date,count(*) number
        //from goods a join goods_extra b on a.extra_id=b.id and b.need_split=1
        //where hospital_id=@p_hospital_id {0}
        //group by a.storeroom_id, a.product_id, a.expired_date", conditionSql);

        //            var db = DatabaseFactory.CreateDatabase();
        //            var dc = db.GetSqlStringCommand(sql);

        //            db.AddInParameter(dc, "p_hospital_id", DbType.String, condition.HospitalId);
        //            if (!string.IsNullOrEmpty(condition.StoreroomId))
        //            {
        //                db.AddInParameter(dc, "p_storeroom_id", DbType.String, condition.StoreroomId);
        //            }
        //            if (!string.IsNullOrEmpty(condition.ProductId))
        //            {
        //                db.AddInParameter(dc, "p_product_id", DbType.String, condition.ProductId);
        //            }

        //            using (var reader = db.ExecuteReader(dc))
        //            {
        //                while (reader.Read())
        //                {
        //                    var entity = new ProductInventoryEntity();
        //                    entity.Init(reader, true);

        //                    list.Add(entity);
        //                }
        //            }
        //        }

        //        private static void UsableProductInventory(ProductInventoryCondition condition, IList<ProductInventoryEntity> list)
        //        {
        //            var conditionSql = "";
        //            if (!string.IsNullOrEmpty(condition.StoreroomId))
        //            {
        //                conditionSql += "and a.storeroom_id=@p_storeroom_id";
        //            }
        //            if (!string.IsNullOrEmpty(condition.ProductId))
        //            {
        //                conditionSql += " and a.product_id=@p_product_id";
        //            }

        //            var sql = string.Format(@"select 
        //	a.storeroom_id,a.product_id,a.expired_date,count(*) number
        //from goods a join goods_extra b on a.extra_id=b.id and b.need_split=0
        //where hospital_id=@p_hospital_id {0}
        //group by a.storeroom_id, a.product_id, a.expired_date", conditionSql);

        //            var db = DatabaseFactory.CreateDatabase();
        //            var dc = db.GetSqlStringCommand(sql);

        //            db.AddInParameter(dc, "p_hospital_id", DbType.String, condition.HospitalId);
        //            if (!string.IsNullOrEmpty(condition.StoreroomId))
        //            {
        //                db.AddInParameter(dc, "p_storeroom_id", DbType.String, condition.StoreroomId);
        //            }
        //            if (!string.IsNullOrEmpty(condition.ProductId))
        //            {
        //                db.AddInParameter(dc, "p_product_id", DbType.String, condition.ProductId);
        //            }

        //            using (var reader = db.ExecuteReader(dc))
        //            {
        //                while (reader.Read())
        //                {
        //                    var entity = new ProductInventoryEntity();
        //                    entity.Init(reader, false);

        //                    list.Add(entity);
        //                }
        //            }
        //        }
        #endregion

        public static List<GoodsEntity> GetMovableGoods(string hospitalId, string storeroomId, string productId)
        {
            var sql = string.Format(@"select {0} from goods g
where hospital_id = @p_hospital_id 
and storeroom_id = @p_storeroom_id 
and product_id = @p_product_id
and status = 1
and granted_count = 0 
and not exists(select 1 from goods_runtime gr where g.serial_id = gr.serial_id and g.barcode = gr.barcode) 
and not exists(select 1 from goods_runtime gr, move_form_serial mfs where gr.apply_id = mfs.move_id and gr.serial_id = mfs.to_serial and g.serial_id = mfs.from_serial and g.barcode = gr.barcode)
order by expired_date, created_time ", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(dc, "p_storeroom_id", DbType.String, storeroomId);
            db.AddInParameter(dc, "p_product_id", DbType.String, productId);

            var list = new List<GoodsEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new GoodsEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }
            return list;
        }

        public static GoodsRuntimeEntity GetRuntime(string barcode, string hospitalId)
        {
            var sql = @"select id,hospital_id,apply_id,serial_id,barcode,count,product_id from goods_runtime where barcode=@p_barcode and hospital_id=@p_hospital_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_barcode", DbType.String, barcode);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);

            using (var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new GoodsRuntimeEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static GoodsRuntimeEntity SaveRuntime(GoodsRuntimeEntity entity, Database db, DbTransaction dbTrans)
        {
            var sql = string.Empty;

            if (string.IsNullOrEmpty(entity.Id))
            {
                sql = @"insert into goods_runtime(id,hospital_id,apply_id,serial_id,barcode,count,product_id)
values(@p_id,@p_hospital_id,@p_apply_id,@p_serial_id,@p_barcode,@p_count,@p_product_id)";

                entity.Id = Guid.NewGuid().ToString();
            }
            else
            {
                sql = @"update goods_runtime set
hospital_id=@p_hospital_id
,apply_id=@p_apply_id
,serial_id=@p_serial_id
,barcode=@p_barcode
,count=@p_count
,product_id=@p_product_id
where id=@p_id";
            }

            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, entity.HospitalId);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, entity.ApplyId);
            db.AddInParameter(cmd, "p_serial_id", DbType.String, entity.SerialId);
            db.AddInParameter(cmd, "p_barcode", DbType.String, entity.Barcode);
            db.AddInParameter(cmd, "p_count", DbType.Int32, entity.Count);
            db.AddInParameter(cmd, "p_product_id", DbType.String, entity.ProductId);

            db.ExecuteNonQuery(cmd, dbTrans);

            return entity;
        }

        internal static IList<GoodsRuntimeEntity> FindRuntimes(string applyId, string productId)
        {
            var sql = @"select id,hospital_id,apply_id,serial_id,barcode,count,product_id from goods_runtime where apply_id=@p_apply_id and product_id=@p_product_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, applyId);
            db.AddInParameter(cmd, "p_product_id", DbType.String, productId);

            var list = new List<GoodsRuntimeEntity>();
            using (var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new GoodsRuntimeEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }
    }
}
