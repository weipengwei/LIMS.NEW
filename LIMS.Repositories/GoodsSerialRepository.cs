using System;
using System.Text;
using System.Data;
using System.Linq;
using System.Data.Common;
using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.Data;

using LIMS.Util;
using LIMS.Entities;

namespace LIMS.Repositories
{
    public static class GoodsSerialRepository
    {
        private const string COLUMNS = @"
id,serial_no,parent_id,product_id,dispatched_count,hospital_id,vendor_id,
need_audit,need_check,need_split,split_copies,split_unit,split_capacity,split_package_count,valid_days,
batch_no,expired_date,logistics_code,logistics_content,is_closed,created_id,created_time,updated_id,updated_time";

        public static GoodsSerialEntity Get(string serialId, Database db, DbTransaction trans)
        {
            var sql = string.Format("select {0} from goods_serial where id=@p_id", COLUMNS);

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, serialId);

            using (var reader = db.ExecuteReader(cmd,trans))
            {
                if (reader.Read())
                {
                    var entity = new GoodsSerialEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static GoodsSerialEntity Get(string serialId)
        {
            var sql = string.Format("select {0} from goods_serial where id=@p_id", COLUMNS);

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, serialId);

            using (var reader = db.ExecuteReader(cmd))
            {
                if(reader.Read())
                {
                    var entity = new GoodsSerialEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static IList<GoodsSerialEntity> GetSplitSerials(IList<string> serialIds)
        {
            var list = new List<GoodsSerialEntity>();

            if (serialIds == null || serialIds.Count == 0)
            {
                return list;
            }

            IList<string> names;
            string namesSql;
            StringHelper.GenerInParameters("p_id", serialIds.Count, out names, out namesSql);

            var sql = string.Format("select {0} from goods_serial where id in ({1}) and need_split=@p_need_split", COLUMNS, namesSql);

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_need_split", DbType.Boolean, true);
            for(var i =0; i < serialIds.Count; i++)
            {
                db.AddInParameter(cmd, names[i], DbType.String, serialIds[i]);
            }

            using(var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new GoodsSerialEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static GoodsSerialEntity GetByBarcode(string barcode, Database db, DbTransaction trans)
        {
            var sql = @"select 
	a.id,a.serial_no,a.parent_id,a.product_id,a.dispatched_count,a.hospital_id,a.vendor_id,a.need_audit,a.need_check,a.need_split,
    a.split_copies,a.split_unit,a.valid_days,a.batch_no,a.expired_date,a.split_capacity,a.split_package_count,
	a.logistics_code,a.logistics_content,a.is_closed,a.created_id,a.created_time,a.updated_id,a.updated_time 
from goods_serial a join goods_serial_barcodes b on a.id=b.serial_id and b.out=@p_out where b.barcode=@p_barcode";

            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_barcode", DbType.String, barcode);
            db.AddInParameter(cmd, "p_out", DbType.Boolean, false);

            using (var reader = db.ExecuteReader(cmd,trans))
            {
                if (reader.Read())
                {
                    var entity = new GoodsSerialEntity();
                    entity.Init(reader);

                    return entity;
                }

                return null;
            }

        }

        public static GoodsSerialEntity GetByBarcode(string barcode)
        {
            var sql = @"select 
	a.id,a.serial_no,a.parent_id,a.product_id,a.dispatched_count,a.hospital_id,a.vendor_id,a.need_audit,a.need_check,a.need_split,
    a.split_copies,a.split_unit,a.valid_days,a.batch_no,a.expired_date,a.split_capacity,a.split_package_count,
	a.logistics_code,a.logistics_content,a.is_closed,a.created_id,a.created_time,a.updated_id,a.updated_time 
from goods_serial a join goods_serial_barcodes b on a.id=b.serial_id and b.out=@p_out where b.barcode=@p_barcode";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_barcode", DbType.String, barcode);
            db.AddInParameter(cmd, "p_out", DbType.Boolean, false);

            using (var reader = db.ExecuteReader(cmd))
            {
                if(reader.Read())
                {
                    var entity = new GoodsSerialEntity();
                    entity.Init(reader);

                    return entity;
                }

                return null;
            }

        }

        public static IList<GoodsSerialBarcodeEntity> GetBarcodes(string serialId, Database db, DbTransaction trans)
        {
            var sql = "select id,serial_id,barcode,is_printed,out from goods_serial_barcodes where serial_id=@p_serial_id and out=@p_out order by barcode";

            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_serial_id", DbType.String, serialId);
            db.AddInParameter(cmd, "p_out", DbType.Boolean, false);

            var list = new List<GoodsSerialBarcodeEntity>();
            using (var reader = db.ExecuteReader(cmd,trans))
            {
                while (reader.Read())
                {
                    var entity = new GoodsSerialBarcodeEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }
            return list;
        }

        public static IList<GoodsSerialBarcodeEntity> GetBarcodes(string serialId)
        {
            var sql = "select id,serial_id,barcode,is_printed,out from goods_serial_barcodes where serial_id=@p_serial_id and out=@p_out order by barcode";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_serial_id", DbType.String, serialId);
            db.AddInParameter(cmd, "p_out", DbType.Boolean, false);

            var list = new List<GoodsSerialBarcodeEntity>();
            using (var reader = db.ExecuteReader(cmd))
            {
                while(reader.Read())
                {
                    var entity = new GoodsSerialBarcodeEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<GoodsSerialBarcodeEntity> GetBarcodes(IList<string> serialIds, bool includeOut = false)
        {
            var list = new List<GoodsSerialBarcodeEntity>();
            if(serialIds.Count == 0)
            {
                return list;
            }

            string sqlIds;
            IList<string> names;
            StringHelper.GenerInParameters("p_serial_", serialIds.Count, out names, out sqlIds);


            var sql = string.Format("select id,serial_id,barcode,is_printed,out from goods_serial_barcodes where serial_id in ({0})", sqlIds);
            if (!includeOut)
            {
                sql += " and out=@p_out";
            }

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            for(var i = 0; i < names.Count; i ++)
            {
                db.AddInParameter(cmd, names[i], DbType.String, serialIds[i]);
            }
            if (!includeOut)
            {
                db.AddInParameter(cmd, "p_out", DbType.Boolean, false);
            }
            
            using (var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new GoodsSerialBarcodeEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<GoodsSerialBarcodeEntity> GetBarcodesByRoot(string serialId)
        {
            var ids = GetSubSerials(serialId).Select(item => item.Id).ToList();
            ids.Add(serialId);

            IList<string> names;
            string namesSql;
            StringHelper.GenerInParameters("p_serial_id", ids.Count, out names, out namesSql);

            var sql = string.Format("select id,serial_id,barcode,is_printed,out from goods_serial_barcodes where serial_id in ({0}) and out=@p_out order by barcode", namesSql);

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            for(var i = 0; i< names.Count; i++)
            {
                db.AddInParameter(cmd, names[i], DbType.String, ids[i]);
            }
            db.AddInParameter(cmd, "p_out", DbType.Boolean, false);

            var list = new List<GoodsSerialBarcodeEntity>();
            using (var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new GoodsSerialBarcodeEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        private static IList<GoodsSerialEntity> GetSubSerials(string parentId)
        {
            var sql = string.Format("select {0} from goods_serial where parent_id=@p_parent_id", COLUMNS);

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_parent_id", DbType.String, parentId);

            var list = new List<GoodsSerialEntity>();
            using (var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    var entity = new GoodsSerialEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }


        #region Create GoodsSerial
        public static IList<string> Create(GoodsSerialEntity entity, Database db, DbTransaction trans)
        {
            var sql = @"insert into goods_serial(
id,serial_no,product_id,dispatched_count,batch_no,expired_date,
need_audit,need_check,need_split,split_copies,split_unit,split_capacity,split_package_count,valid_days,
hospital_id,vendor_id,is_closed,created_id,created_time,updated_id,updated_time)
values(
    @p_id,@p_serial_no,@p_product_id,@p_dispatched_count,@p_batch_no,@p_expired_date,
    @p_need_audit,@p_need_check,@p_need_split,@p_split_copies,@p_split_unit,@p_split_capacity,@p_split_package_count,@p_valid_days,
    @p_hospital_id,@p_vendor_id,@p_is_closed,@p_created_id,@p_created_time,@p_updated_id,@p_updated_time)";

            entity.Id = Guid.NewGuid().ToString();
            entity.SerialNo = FormatSerialNo(IdentityCreatorRepository.Get(GetSerialKey(), 1));

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);
            db.AddInParameter(cmd, "p_serial_no", DbType.String, entity.SerialNo);
            db.AddInParameter(cmd, "p_product_id", DbType.String, entity.ProductId);
            db.AddInParameter(cmd, "p_dispatched_count", DbType.Int64, entity.DispatchedCount);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, entity.HospitalId);
            db.AddInParameter(cmd, "p_vendor_id", DbType.String, entity.VendorId);
            db.AddInParameter(cmd, "p_batch_no", DbType.String, entity.BatchNo);
            db.AddInParameter(cmd, "p_expired_date", DbType.DateTime, entity.ExpiredDate);

            db.AddInParameter(cmd, "p_need_audit", DbType.Boolean, entity.NeedAudit);
            db.AddInParameter(cmd, "p_need_check", DbType.Boolean, entity.NeedCheck);
            db.AddInParameter(cmd, "p_need_split", DbType.Boolean, entity.NeedSplit);
            db.AddInParameter(cmd, "p_split_copies", DbType.Int32, entity.SplitCopies);
            db.AddInParameter(cmd, "p_split_unit", DbType.String, entity.SplitUnit);
            db.AddInParameter(cmd, "p_split_capacity", DbType.String, entity.SplitCapacity);
            db.AddInParameter(cmd, "p_split_package_count", DbType.Int32, entity.SplitPackageCount);
            db.AddInParameter(cmd, "p_valid_days", DbType.Decimal, entity.ValidDays);

            db.AddInParameter(cmd, "p_is_closed", DbType.Boolean, entity.IsClosed);
            db.AddInParameter(cmd, "p_created_id", DbType.String, entity.CreatedId);
            db.AddInParameter(cmd, "p_created_time", DbType.DateTime, entity.CreatedTime);
            db.AddInParameter(cmd, "p_updated_id", DbType.String, entity.UpdatedId);
            db.AddInParameter(cmd, "p_updated_time", DbType.DateTime, entity.UpdatedTime);

            db.ExecuteNonQuery(cmd, trans);

            return CreateSerialBarcodes(entity.Id, entity.DispatchedCount, db, trans);
        }

        private static IList<string> CreateSerialBarcodes(string serialId, int count, Database db, DbTransaction trans)
        {
            var barcodes = new List<string>();

            var baseKey = IdentityCreatorRepository.Get(IdentityKey.GOODS_BARCODE, count);

            var sql = @"insert into goods_serial_barcodes(id,serial_id,barcode,is_printed,out) values(@p_id,@p_serial_id,@p_barcode,@p_is_printed,@p_out)";
            for (var i = 0; i < count; i++)
            {
                var barcode = StringHelper.Barcode(baseKey + i);
                barcodes.Add(barcode);

                var dc = db.GetSqlStringCommand(sql);
                db.AddInParameter(dc, "p_id", DbType.String, Guid.NewGuid().ToString());
                db.AddInParameter(dc, "p_serial_id", DbType.String, serialId);
                db.AddInParameter(dc, "p_barcode", DbType.String, barcode);
                db.AddInParameter(dc, "p_is_printed", DbType.Boolean, false);
                db.AddInParameter(dc, "p_out", DbType.Boolean, false);

                db.ExecuteNonQuery(dc, trans);
            }

            return barcodes;
        }
        #endregion


        #region Create Sub GoodsSerial
        public static GoodsSerialEntity CreateSub(GoodsSerialEntity parent, IList<string> barcodes, string userId, Database db, DbTransaction trans)
        {
            var entity = new GoodsSerialEntity
            {
                Id = Guid.NewGuid().ToString(),
                SerialNo = parent.SerialNo,
                ParentId = parent.Id,
                ProductId = parent.ProductId,
                DispatchedCount = barcodes.Count,
                HospitalId = parent.HospitalId,
                VendorId = parent.VendorId,
                NeedAudit = parent.NeedAudit,
                NeedCheck = parent.NeedCheck,
                NeedSplit = parent.NeedSplit,
                SplitCopies = parent.SplitCopies,
                SplitUnit = parent.SplitUnit,
                SplitCapacity = parent.SplitCapacity,
                SplitPackageCount = parent.SplitPackageCount,
                ValidDays = parent.ValidDays,
                IsClosed = parent.IsClosed,
                BatchNo = parent.BatchNo,
                ExpiredDate = parent.ExpiredDate,
                LogisticsCode = parent.LogisticsCode,
                LogisticsContent = parent.LogisticsContent,
                CreatedId = userId,
                CreatedTime = DateTime.Now,
                UpdatedId = userId,
                UpdatedTime = DateTime.Now
            };

            var sql = @"insert into goods_serial(
    id,serial_no,parent_id,product_id,dispatched_count,hospital_id,vendor_id,
    batch_no,expired_date,logistics_code,logistics_content,
    need_audit,need_check,need_split,split_copies,split_unit,split_capacity,split_package_count,valid_days,
    is_closed,created_id,created_time,updated_id,updated_time
)
values(
    @p_id,@p_serial_no,@p_parent_id,@p_product_id,@p_dispatched_count,@p_hospital_id,@p_vendor_id,
    @p_batch_no,@p_expired_date,@p_logistics_code,@p_logistics_content,
    @p_need_audit,@p_need_check,@p_need_split,@p_split_copies,@p_split_unit,@p_split_capacity,@p_split_package_count,@p_valid_days,
    @p_is_closed,@p_created_id,@p_created_time,@p_updated_id,@p_updated_time
)";
            
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);
            db.AddInParameter(cmd, "p_serial_no", DbType.String, entity.SerialNo);
            db.AddInParameter(cmd, "p_parent_id", DbType.String, entity.ParentId);
            db.AddInParameter(cmd, "p_product_id", DbType.String, entity.ProductId);
            db.AddInParameter(cmd, "p_dispatched_count", DbType.Int64, entity.DispatchedCount);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, entity.HospitalId);
            db.AddInParameter(cmd, "p_vendor_id", DbType.String, entity.VendorId);
            db.AddInParameter(cmd, "p_batch_no", DbType.String, entity.BatchNo);
            db.AddInParameter(cmd, "p_expired_date", DbType.DateTime, entity.ExpiredDate);
            db.AddInParameter(cmd, "p_logistics_code", DbType.String, entity.LogisticsCode);
            db.AddInParameter(cmd, "p_logistics_content", DbType.String, entity.LogisticsContent);

            db.AddInParameter(cmd, "p_need_audit", DbType.Boolean, entity.NeedAudit);
            db.AddInParameter(cmd, "p_need_check", DbType.Boolean, entity.NeedCheck);
            db.AddInParameter(cmd, "p_need_split", DbType.Boolean, entity.NeedSplit);
            db.AddInParameter(cmd, "p_split_copies", DbType.Int32, entity.SplitCopies);
            db.AddInParameter(cmd, "p_split_unit", DbType.String, entity.SplitUnit);
            db.AddInParameter(cmd, "p_split_capacity", DbType.String, entity.SplitCapacity);
            db.AddInParameter(cmd, "p_split_package_count", DbType.Int32, entity.SplitPackageCount);
            db.AddInParameter(cmd, "p_valid_days", DbType.Decimal, entity.ValidDays);

            db.AddInParameter(cmd, "p_is_closed", DbType.Boolean, entity.IsClosed);
            db.AddInParameter(cmd, "p_created_id", DbType.String, entity.CreatedId);
            db.AddInParameter(cmd, "p_created_time", DbType.DateTime, entity.CreatedTime);
            db.AddInParameter(cmd, "p_updated_id", DbType.String, entity.UpdatedId);
            db.AddInParameter(cmd, "p_updated_time", DbType.DateTime, entity.UpdatedTime);

            db.ExecuteNonQuery(cmd, trans);

            UpdateSerialParentCount(entity, db, trans);

            UpdateSerialBarcodes(entity.Id, entity.ParentId, barcodes, db, trans);

            

            return entity;
        }

        private static void UpdateSerialParentCount(GoodsSerialEntity entity, Database db, DbTransaction trans)
        {
            var sql = "update goods_serial set dispatched_count=dispatched_count-@p_subserial_count where id=@p_parent_id";
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_parent_id", DbType.String, entity.ParentId);
            db.AddInParameter(cmd, "p_subserial_count", DbType.Int64, entity.DispatchedCount);
            db.ExecuteNonQuery(cmd, trans);
        }


        private static void UpdateSerialBarcodes(string serialId, string parentSerialId, IList<string> barcodes, Database db, DbTransaction trans)
        {
            if (barcodes == null || barcodes.Count == 0)
            {
                return;
            }

            IList<string> paramNames;
            string paramSql;
            StringHelper.GenerInParameters("p_barcode", barcodes.Count, out paramNames, out paramSql);

            var sql = string.Format("update goods_serial_barcodes set serial_id=@p_serial_id where serial_id=@p_parent_serial_id and barcode in ({0})", paramSql);
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_serial_id", DbType.String, serialId);
            db.AddInParameter(cmd, "p_parent_serial_id", DbType.String, parentSerialId);
            for (var i = 0; i < barcodes.Count; i++)
            {
                db.AddInParameter(cmd, paramNames[i], DbType.String, barcodes[i]);
            }

            db.ExecuteNonQuery(cmd, trans);
        }
        #endregion


        private static string GetSerialKey()
        {
            var year = DateTime.Now.Year;
            return string.Format("SERIAL_NO_{0}", year);
        }

        private static string FormatSerialNo(int baseSerialNo)
        {
            var date = DateTime.Now;
            var part1 = date.Year * 10000 + date.Month * 100 + date.Day;

            var max = 100000000;
            var part2 = baseSerialNo >= max ? baseSerialNo.ToString() : baseSerialNo.ToString().PadLeft(8, '0');

            return string.Format("S{0}{1}", part1, part2);
        }

        internal static void UpdateSerialInfo(GoodsSerialEntity entity, Database db, DbTransaction trans)
        {
            var sql = @"update goods_serial set 
logistics_code=@p_logistics_code,logistics_content=@p_logistics_content,
updated_id=@p_updated_id,updated_time=@p_updated_time where id=@p_id";
            
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);
            db.AddInParameter(cmd, "p_logistics_code", DbType.String, entity.LogisticsCode);
            db.AddInParameter(cmd, "p_logistics_content", DbType.String, entity.LogisticsContent);
            db.AddInParameter(cmd, "p_updated_id", DbType.String, entity.UpdatedId);
            db.AddInParameter(cmd, "p_updated_time", DbType.DateTime, entity.UpdatedTime);

            db.ExecuteNonQuery(cmd, trans);
        }

        internal static void Close(string serialId, Database db, DbTransaction trans)
        {
            var sql = @"update goods_serial set is_closed=@p_is_closed where id=@p_id";

            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_id", DbType.String, serialId);
            db.AddInParameter(cmd, "p_is_closed", DbType.Boolean, true);

            db.ExecuteNonQuery(cmd, trans);
        }

        public static void UpdatePrint(string serialId, IList<string> barcodes)
        {
            if(string.IsNullOrEmpty(serialId))
            {
                throw new Exception("The serial id is empty.");
            }

            if(barcodes == null || barcodes.Count == 0)
            {
                return;
            }

            IList<string> paramNames;
            string paramSql;
            StringHelper.GenerInParameters("p_barcode", barcodes.Count, out paramNames, out paramSql);

            var sql = string.Format("update goods_serial_barcodes set is_printed=@p_is_printed where serial_id=@p_serial_id and barcode in ({0})", paramSql);
            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_serial_id", DbType.String, serialId);
            db.AddInParameter(cmd, "p_is_printed", DbType.Boolean, true);
            for (var i = 0; i < barcodes.Count; i ++)
            {
                db.AddInParameter(cmd, paramNames[i], DbType.String, barcodes[i]);
            }

            db.ExecuteNonQuery(cmd);
        }

//        public static GoodsSerialEntity Save(GoodsSerialEntity entity, Database db, DbTransaction trans)
//        {
//            var sql = string.Empty;

//            if (string.IsNullOrEmpty(entity.Id))
//            {
//                sql = @"insert into goods_serial(
//id,serial_no,product_id,dispatched_count,batch_no,expired_date,
//need_audit,need_check,need_split,split_copies,split_unit,valid_days,
//hospital_id,vendor_id,is_closed,created_id,created_time,updated_id,updated_time)
//values(
//    @p_id,@p_serial_no,@p_product_id,@p_dispatched_count,@p_batch_no,@p_expired_date,
//    @p_need_audit,@p_need_check,@p_need_split,@p_split_copies,@p_split_unit,@p_valid_days,
//    @p_hospital_id,@p_vendor_id,@p_is_closed,@p_created_id,@p_created_time,@p_updated_id,@p_updated_time)";

//                entity.Id = Guid.NewGuid().ToString();
//            }
//            else
//            {
//                sql = @"update goods_serial set
//serial_no=@p_serial_no,product_id=@p_product_id,dispatched_count=@p_dispatched_count
//,batch_no=@p_batch_no,expired_date=@p_expired_date,need_audit = @p_need_audit
//,need_check = @p_need_check,need_split = @p_need_split,split_copies=@p_split_copies
//,split_unit=@p_split_unit,valid_days=@p_valid_days,hospital_id=@p_hospital_id
//,vendor_id=@p_vendor_id,is_closed=@p_is_closed,created_id=@p_created_id
//,created_tim=e@p_created_time,updated_id=@p_updated_id,updated_time=@p_updated_time 
//where id=@p_id";
//            }

//            var cmd = db.GetSqlStringCommand(sql);
//            db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);
//            db.AddInParameter(cmd, "p_serial_no", DbType.String, entity.SerialNo);
//            db.AddInParameter(cmd, "p_product_id", DbType.String, entity.ProductId);
//            db.AddInParameter(cmd, "p_dispatched_count", DbType.Int64, entity.DispatchedCount);
//            db.AddInParameter(cmd, "p_hospital_id", DbType.String, entity.HospitalId);
//            db.AddInParameter(cmd, "p_vendor_id", DbType.String, entity.VendorId);
//            db.AddInParameter(cmd, "p_batch_no", DbType.String, entity.BatchNo);
//            db.AddInParameter(cmd, "p_expired_date", DbType.DateTime, entity.ExpiredDate);

//            db.AddInParameter(cmd, "p_need_audit", DbType.Boolean, entity.NeedAudit);
//            db.AddInParameter(cmd, "p_need_check", DbType.Boolean, entity.NeedCheck);
//            db.AddInParameter(cmd, "p_need_split", DbType.Boolean, entity.NeedSplit);
//            db.AddInParameter(cmd, "p_split_copies", DbType.Int32, entity.SplitCopies);
//            db.AddInParameter(cmd, "p_split_unit", DbType.String, entity.SplitUnit);
//            db.AddInParameter(cmd, "p_valid_days", DbType.Decimal, entity.ValidDays);

//            db.AddInParameter(cmd, "p_is_closed", DbType.Boolean, entity.IsClosed);
//            db.AddInParameter(cmd, "p_created_id", DbType.String, entity.CreatedId);
//            db.AddInParameter(cmd, "p_created_time", DbType.DateTime, entity.CreatedTime);
//            db.AddInParameter(cmd, "p_updated_id", DbType.String, entity.UpdatedId);
//            db.AddInParameter(cmd, "p_updated_time", DbType.DateTime, entity.UpdatedTime);

//            db.ExecuteNonQuery(cmd, trans);

//            return entity;
//        }
    }
}
