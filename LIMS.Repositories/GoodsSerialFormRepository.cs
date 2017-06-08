using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.Data;

using LIMS.Util;
using LIMS.Entities;

namespace LIMS.Repositories
{
    public static class GoodsSerialFormRepository
    {
        private const string COLUMNS = "serial_id,form_id,form_kind,created_id,created_time";


        public static GoodsSerialFormEntity GetBySerialId(string serialId)
        {
            var sql = string.Format("select {0} from goods_serial_form where serial_id=@p_serial_id", COLUMNS);

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_serial_id", DbType.String, serialId);

            using (var reader = db.ExecuteReader(cmd))
            {
                if(reader.Read())
                {
                    var entity = new GoodsSerialFormEntity();
                    entity.Init(reader);

                    return entity;
                }

                return null;
            }
        }


        #region Serial Flow
        public static void Create(GoodsSerialFormEntity current, GoodsSerialFormEntity previous, Database db, DbTransaction trans)
        {
            previous.CreatedTime = DateTime.Now;
            SaveForms(previous, db, trans);

            Create(current, db, trans);
        }

        internal static void Create(GoodsSerialFormEntity current, Database db, DbTransaction trans)
        {
            var sql = string.Format(@"insert into 
goods_serial_form(serial_id,form_id,form_kind,created_id,created_time)
values(@p_serial_id,@p_form_id,@p_form_kind,@p_created_id,@p_created_time)", COLUMNS);
            
            current.CreatedTime = DateTime.Now;

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_serial_id", DbType.String, current.SerialId);
            db.AddInParameter(cmd, "p_form_id", DbType.String, current.FormId);
            db.AddInParameter(cmd, "p_form_kind", DbType.String, current.FormKind);
            db.AddInParameter(cmd, "p_created_id", DbType.String, current.CreatedId);
            db.AddInParameter(cmd, "p_created_time", DbType.DateTime, current.CreatedTime);

            db.ExecuteNonQuery(cmd, trans);

            SaveForms(current, db, trans);
        }

        public static void FlowNextForm(GoodsSerialFormEntity next, Database db, DbTransaction trans)
        {
            var sql = @"update goods_serial_form set form_id=@p_form_id,form_kind=@p_form_kind,created_id=@p_created_id,created_time=@p_created_time where serial_id=@p_serial_id";

            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_serial_id", DbType.String, next.SerialId);
            db.AddInParameter(cmd, "p_form_id", DbType.String, next.FormId);
            db.AddInParameter(cmd, "p_form_kind", DbType.String, next.FormKind);
            db.AddInParameter(cmd, "p_created_id", DbType.String, next.CreatedId);
            db.AddInParameter(cmd, "p_created_time", DbType.DateTime, next.CreatedTime);

            db.ExecuteNonQuery(cmd, trans);

            SaveForms(next, db, trans);
        }

        private static void SaveForms(GoodsSerialFormEntity entity, Database db, DbTransaction trans)
        {
            var sql = string.Format(@"insert into 
goods_serial_forms(serial_id,form_id,form_kind,created_id,created_time)
values(@p_serial_id,@p_form_id,@p_form_kind,@p_created_id,@p_created_time)", COLUMNS);

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_serial_id", DbType.String, entity.SerialId);
            db.AddInParameter(cmd, "p_form_id", DbType.String, entity.FormId);
            db.AddInParameter(cmd, "p_form_kind", DbType.String, entity.FormKind);
            db.AddInParameter(cmd, "p_created_id", DbType.String, entity.CreatedId);
            db.AddInParameter(cmd, "p_created_time", DbType.DateTime, entity.CreatedTime);

            db.ExecuteNonQuery(cmd, trans);
        }
        
        #endregion


        #region Change Status
        //public static bool UpdateStatusInVendor(string barcode, string vendorId, string formKind, bool isValid, string operatorId, out string errorCode)
        //{
        //    var goodsSerial = GoodsSerialRepository.GetByBarcode(barcode);
        //    if (goodsSerial == null)
        //    {
        //        throw new Exception("The goods serial does not exist.");
        //    }

        //    if (string.Compare(goodsSerial.VendorId, vendorId, true) != 0)
        //    {
        //        throw new Exception("The goods serial does not exist in the vendor.");
        //    }

        //    return UpdateStatus(goodsSerial.Id, formKind, isValid, operatorId, out errorCode);
        //}

        //public static bool UpdateStatusInHospital(string barcode, string hospitalId, string formKind, bool isValid, string operatorId, out string errorCode)
        //{
        //    var goodsSerial = GoodsSerialRepository.GetByBarcode(barcode);
        //    if(goodsSerial == null)
        //    {
        //        throw new Exception("The goods serial does not exist.");
        //    }

        //    if(string.Compare(goodsSerial.HospitalId, hospitalId, true) != 0)
        //    {
        //        throw new Exception("The goods serial does not exist in the hospital.");
        //    }

        //    return UpdateStatus(goodsSerial.Id, formKind, isValid, operatorId, out errorCode);
        //}

        //private static bool UpdateStatus(string serialId, string formKind, bool isValid, string operatorId, out string errorCode)
        //{
        //    errorCode = string.Empty;

        //    var entity = GetBySerialId(serialId);
        //    if (entity == null)
        //    {
        //        throw new Exception("The goods serial form dose not exist.");
        //    }

        //    if (string.Compare(entity.FormKind, formKind, true) != 0)
        //    {
        //        errorCode = ErrorCodes.NotMatchFormKindInGoodsFlow;
        //        return false;
        //    }

        //    if (entity.IsConfirmed == isValid)
        //    {
        //        errorCode = ErrorCodes.StatusErrorInGoodsFlow;
        //        return false;
        //    }

        //    var db = DatabaseFactory.CreateDatabase();


        //    using (var conn = db.CreateConnection())
        //    {
        //        conn.Open();
        //        using (var trans = conn.BeginTransaction())
        //        {
        //            try
        //            {
        //                entity.IsConfirmed = isValid;
        //                entity.ConfirmedId = operatorId;
        //                entity.ConfirmedTime = DateTime.Now;

        //                var sql = "update goods_serial_form set is_confirmed=@p_is_confirmed,confirmed_id=@p_confirmed_id,confirmed_time=@p_confirmed_time where id=@p_id";
        //                var cmd = db.GetSqlStringCommand(sql);

        //                db.AddInParameter(cmd, "p_is_confirmed", DbType.Boolean, entity.IsConfirmed);
        //                db.AddInParameter(cmd, "p_confirmed_id", DbType.String, entity.ConfirmedId);
        //                db.AddInParameter(cmd, "p_confirmed_time", DbType.DateTime, entity.ConfirmedTime);
        //                db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);

        //                db.ExecuteNonQuery(cmd, trans);

        //                trans.Commit();

        //                return true;
        //            }
        //            catch
        //            {
        //                trans.Rollback();
        //                throw;
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
