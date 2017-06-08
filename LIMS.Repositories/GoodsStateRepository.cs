using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.Data;

using LIMS.Entities;
using LIMS.Util;

namespace LIMS.Repositories
{
    public static class GoodsStateRepository
    {
        private const string COLUMNS = @"
id, barcode, product_id, order_form_id, order_form_no, 
form_id, form_type, state_created_user, state_created_time, state_validate_user, state_validate_time, state_changed_user, state_changed_time,
future_form_id, future_form_type, future_created_user, future_created_time, future_valid, future_validate_user, future_validate_time, vendor_id";

        public static string GetBarcode()
        {
            var baseKey = IdentityCreatorRepository.Get(IdentityKey.GOODS_BARCODE, 1);
            return FormatBarcode(baseKey);
        }

        public static IList<string> GetBarcodes(int count)
        {
            var barcodes = new List<string>();

            var baseKey = IdentityCreatorRepository.Get(IdentityKey.GOODS_BARCODE, count);
            for(var i = 0; i < count; i++)
            {
                barcodes.Add(FormatBarcode(baseKey + i));
            }

            return barcodes;
        }

        #region Create
        public static void Create(int baseBarcode, int count, GoodsStateEntity baseState, Database db, DbTransaction trans)
        {
            var sql = @"insert into goods_state(
id, barcode, product_id, order_form_id, order_form_no, 
future_form_id, future_form_type, future_created_user, future_created_time, vendor_id) 
values(@p_id, @p_barcode, @p_product_id, @p_order_form, @p_order_form_no, 
@p_future_form_id, @p_future_form_type, @p_future_created_user, @p_future_created_time,@p_vendor_id)";

            for (var i = 0; i < count; i++)
            {
                var barcode = FormatBarcode(baseBarcode + i);

                var dc = db.GetSqlStringCommand(sql);
                db.AddInParameter(dc, "p_id", DbType.String, Guid.NewGuid().ToString());
                db.AddInParameter(dc, "p_barcode", DbType.String, barcode);
                db.AddInParameter(dc, "p_product_id", DbType.String, baseState.ProductId);
                db.AddInParameter(dc, "p_order_form", DbType.String, baseState.OrderFormId);
                db.AddInParameter(dc, "p_order_form_no", DbType.Int32, baseState.OrderFormNo);
                db.AddInParameter(dc, "p_future_form_id", DbType.String, baseState.FutureFormId);
                db.AddInParameter(dc, "p_future_form_type", DbType.Int32, (int)baseState.FutureFormType);
                db.AddInParameter(dc, "p_future_created_user", DbType.String, baseState.FutureCreatedUser);
                db.AddInParameter(dc, "p_future_created_time", DbType.DateTime, baseState.FutureCreatedTime);
                db.AddInParameter(dc, "p_vendor_id", DbType.String, baseState.VendorId);

                db.ExecuteNonQuery(dc, trans);
            }
        }

        public static void Create(GoodsStateEntity goodsState, Database db, DbTransaction trans)
        {
            var sql = @"insert into goods_state(
id, barcode, product_id, form_id, form_type, order_form_id, order_form_no, state_created_user, state_created_time, state_validate_user, state_validate_time,state_changed_user, state_changed_time,vendor_id) 
values(@p_id, @p_barcode, @p_product_id, @p_form_id, @p_form_type, @p_order_form, @p_order_form_no, @p_state_created_user, @p_state_created_time, 
@p_state_validate_user, @p_state_validate_time,@p_state_changed_user, @p_state_changed_time,@p_vendor_id)";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, Guid.NewGuid().ToString());
            db.AddInParameter(dc, "p_barcode", DbType.String, goodsState.Barcode);
            db.AddInParameter(dc, "p_product_id", DbType.String, goodsState.ProductId);
            db.AddInParameter(dc, "p_order_form", DbType.String, goodsState.OrderFormId);
            db.AddInParameter(dc, "p_order_form_no", DbType.Int32, goodsState.OrderFormNo);
            db.AddInParameter(dc, "p_form_id", DbType.String, goodsState.FormId);
            db.AddInParameter(dc, "p_form_type", DbType.Int32, (int)goodsState.FormType);
            db.AddInParameter(dc, "p_state_created_user", DbType.String, goodsState.StateCreatedUser);
            db.AddInParameter(dc, "p_state_created_time", DbType.DateTime, goodsState.StateCreateTime);
            db.AddInParameter(dc, "p_state_validate_user", DbType.String, goodsState.StateValidateUser);
            db.AddInParameter(dc, "p_state_validate_time", DbType.DateTime, goodsState.StateValidateTime);
            db.AddInParameter(dc, "p_state_changed_user", DbType.String, goodsState.StateChangedUser);
            db.AddInParameter(dc, "p_state_changed_time", DbType.DateTime, goodsState.StateChangedTime);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, goodsState.VendorId);

            db.ExecuteNonQuery(dc, trans);

            ArchiveState(goodsState, db, trans);
        }
        
        private static string FormatBarcode(int baseKey)
        {
            return (10000000000000 + baseKey).ToString().Substring(1);
        }
        #endregion


        #region Get
        private static GoodsStateEntity Get(string id)
        {
            var sql = string.Format("select {0} from goods_state where id=@p_id", COLUMNS);
            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);

            using (var reader = db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var entity = new GoodsStateEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static GoodsStateEntity GetByBarcode(string barcode)
        {
            var sql = string.Format("select {0} from goods_state where barcode=@p_barcode", COLUMNS);
            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_barcode", DbType.String, barcode);

            using (var reader = db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var entity = new GoodsStateEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }
        #endregion


        #region Validate State
        public static bool CanFormValidate(string formId, FormType formType, string hospitalId, out string errorCode)
        {
            errorCode = string.Empty;
            if(string.IsNullOrEmpty(formId))
            {
                errorCode = GoodsStateValidateCodes.NotValidState;
                return false;
            }

            bool noForm;
            string formHospitalId;

            switch (formType)
            {
                case FormType.Receive:
                    var receive = ReceiveFormRepository.Get(formId);
                    noForm = receive == null;
                    formHospitalId = noForm ? string.Empty : receive.HospitalId;
                    break;
                case FormType.Inspection:
                    var inspection = InspectionFormRepository.Get(formId);
                    noForm = inspection == null;
                    formHospitalId = noForm ? string.Empty : inspection.HospitalId;
                    break;
                case FormType.Incoming:
                    var incoming = IncomingFormRepository.Get(formId);
                    noForm = incoming == null;
                    formHospitalId = noForm ? string.Empty : incoming.HospitalId;
                    break;
                case FormType.Apply:
                    var apply = ApplyFormRepository.Get(formId);
                    noForm = apply == null;
                    formHospitalId = noForm ? string.Empty : apply.HospitalId;
                    break;
                case FormType.Return:
                    var @return = ReturnFormRepository.Get(formId);
                    noForm = @return == null;
                    formHospitalId = noForm ? string.Empty : @return.HospitalId;
                    break;
                default:
                    errorCode = GoodsStateValidateCodes.NotValidState;
                    noForm = true;
                    formHospitalId = string.Empty;
                    break;
            }

            if(!string.IsNullOrEmpty(errorCode))
            {
                return false;
            }

            if (noForm)
            {
                errorCode = GoodsStateValidateCodes.FutureFormNotExist;
                return false;
            }

            if (string.Compare(formHospitalId, hospitalId, true) != 0)
            {
                errorCode = GoodsStateValidateCodes.HospitalNoFutureForm;
                return false;
            }

            return true;
        }

        public static bool CanValidate(string barcode, FormType formType, string hospitalId, out string errorCode)
        {
            GoodsStateEntity entity;
            if (!ValidateFutureState(barcode, formType, out entity, out errorCode))
            {
                return false;
            }

            errorCode = string.Empty;
            var result = false;
            switch (formType)
            {
                case FormType.Receive:
                    result = ValidateReceive(entity, hospitalId, out errorCode);
                    break;
                case FormType.Inspection:
                    result = ValidateInspection(entity, hospitalId, out errorCode);
                    break;
                case FormType.Incoming:
                    result = ValidateIncoming(entity, hospitalId, out errorCode);
                    break;
                case FormType.Apply:
                    result = ValidateApply(entity, hospitalId, out errorCode);
                    break;
                case FormType.Splitting:
                    result = ValidateSplitting(entity, hospitalId, out errorCode);
                    break;
                case FormType.MoveOut:
                    result = ValidateMoveout(entity, hospitalId, out errorCode);
                    break;
                case FormType.MoveIn:
                    result = ValidateMovein(entity, hospitalId, out errorCode);
                    break;
                default:
                    errorCode = GoodsStateValidateCodes.NotValidState;
                    result = false;
                    break;
            }

            return result;
        }
        
        public static bool CanValidate(string barcode, FormType formType, string hospitalId, string vendorId, out string errorCode)
        {
            GoodsStateEntity entity;
            if(!ValidateFutureState(barcode, formType, out entity, out errorCode))
            {
                return false;
            }
            
            bool result;
            switch(formType)
            {
                case FormType.Dispatch:
                    result = ValidateDispatch(entity, hospitalId, vendorId, out errorCode);
                    break;
                case FormType.Return:
                    result = ValidateReturn(entity, hospitalId, vendorId, out errorCode);
                    break;
                default:
                    errorCode = GoodsStateValidateCodes.NotValidState;
                    result = false;
                    break;
            }

            return result;
        }

        private static bool ValidateFutureState(string barcode, FormType formType, out GoodsStateEntity entity, out string errorCode)
        {
            entity = null;
            errorCode = string.Empty;

            if (string.IsNullOrEmpty(barcode))
            {
                errorCode = GoodsStateValidateCodes.BarcodeNotExist;
                return false;
            }

            entity = GetByBarcode(barcode);
            if (entity == null)
            {
                errorCode = GoodsStateValidateCodes.BarcodeNotExist;
                return false;
            }

            //if (formType != FormType.MoveOut && formType != FormType.Return && string.IsNullOrEmpty(entity.FutureFormId))
            //{
            //    errorCode = GoodsStateValidateCodes.NotValidState;
            //    return false;
            //}

            return true;
        }

        private static bool ValidateDispatch(GoodsStateEntity current, string hospitalId, string vendorId, out string errorCode)
        {
            errorCode = string.Empty;
            var form = DispatchFormRepository.Get(current.FutureFormId);
            if (form == null)
            {
                errorCode = GoodsStateValidateCodes.FutureFormNotExist;
                return false;
            }

            if (string.Compare(form.HospitalId, hospitalId, true) != 0)
            {
                errorCode = GoodsStateValidateCodes.HospitalNoFutureForm;
                return false;
            }

            if (!Enum.IsDefined(typeof(FormType), current.FutureFormType) 
                || (FormType)current.FutureFormType != FormType.Dispatch)
            {
                errorCode = GoodsStateValidateCodes.NotValidState;
                return false;
            }


            if(string.Compare(form.VendorId, vendorId, true) != 0)
            {
                errorCode = GoodsStateValidateCodes.VendorNoFutureForm;
                return false;
            }

            return true;
        }

        private static bool ValidateReceive(GoodsStateEntity current, string hospitalId, out string errorCode)
        {
            errorCode = string.Empty;

            var form = ReceiveFormRepository.Get(current.FutureFormId);
            if (form == null)
            {
                errorCode = GoodsStateValidateCodes.FutureFormNotExist;
                return false;
            }

            if (string.Compare(form.HospitalId, hospitalId, true) != 0)
            {
                errorCode = GoodsStateValidateCodes.HospitalNoFutureForm;
                return false;
            }

            if (!Enum.IsDefined(typeof(FormType), current.FutureFormType)
                || (FormType)current.FutureFormType != FormType.Receive)
            {
                errorCode = GoodsStateValidateCodes.NotValidState;
                return false;
            }

            return true;
        }

        private static bool ValidateInspection(GoodsStateEntity current, string hospitalId, out string errorCode)
        {
            errorCode = string.Empty;

            var form = InspectionFormRepository.Get(current.FutureFormId);
            if (form == null)
            {
                errorCode = GoodsStateValidateCodes.FutureFormNotExist;
                return false;
            }

            if (string.Compare(form.HospitalId, hospitalId, true) != 0)
            {
                errorCode = GoodsStateValidateCodes.HospitalNoFutureForm;
                return false;
            }

            if (!Enum.IsDefined(typeof(FormType), current.FutureFormType)
                || (FormType)current.FutureFormType != FormType.Inspection)
            {
                errorCode = GoodsStateValidateCodes.NotValidState;
                return false;
            }

            return true;
        }

        private static bool ValidateIncoming(GoodsStateEntity current, string hospitalId, out string errorCode)
        {
            errorCode = string.Empty;

            var form = IncomingFormRepository.Get(current.FutureFormId);
            if (form == null)
            {
                errorCode = GoodsStateValidateCodes.FutureFormNotExist;
                return false;
            }

            if (string.Compare(form.HospitalId, hospitalId, true) != 0)
            {
                errorCode = GoodsStateValidateCodes.HospitalNoFutureForm;
                return false;
            }

            if (!Enum.IsDefined(typeof(FormType), current.FutureFormType)
                || (FormType)current.FutureFormType != FormType.Incoming)
            {
                errorCode = GoodsStateValidateCodes.NotValidState;
                return false;
            }

            return true;
        }

        private static bool ValidateApply(GoodsStateEntity current, string hospitalId, out string errorCode)
        {
            errorCode = string.Empty;
            if (!Enum.IsDefined(typeof(FormType), current.FormType)
                || (FormType)current.FormType != FormType.Incoming)
            {
                errorCode = GoodsStateValidateCodes.NotValidState;
                return false;
            }

            return true;
        }

        private static bool ValidateSplitting(GoodsStateEntity current, string hospitalId, out string errorCode)
        {
            errorCode = string.Empty;
            return true;
            //if(!string.IsNullOrEmpty(current.FormId) && current.FormType == FormType.Incoming)
            //{
            //    return true;
            //}

            //errorCode = GoodsStateValidateCodes.NotValidState;
            //return false;
        }

        private static bool ValidateReturn(GoodsStateEntity current, string hospitalId, string vendorId, out string errorCode)
        {
            errorCode = string.Empty;
            var orderForm = OrderFormRepository.Get(current.OrderFormId);
            if (string.Compare(orderForm.HospitalId, hospitalId, false) != 0)
            {
                errorCode = GoodsStateValidateCodes.HospitalNoFutureForm;
                return false;
            }

            if (!string.IsNullOrEmpty(current.FormId)
                && (current.FormType == FormType.Dispatch
                    || current.FormType == FormType.Receive
                    || current.FormType == FormType.Inspection
                    || current.FormType == FormType.Incoming))
            {
                return true;
            }
            
            if (string.Compare(orderForm.VendorId, vendorId, true) != 0)
            {
                errorCode = GoodsStateValidateCodes.VendorNoFutureForm;
                return false;
            }

            errorCode = GoodsStateValidateCodes.NotValidState;
            return false;
        }

        private static bool ValidateMoveout(GoodsStateEntity current, string hospitalId, out string errorCode)
        {
            errorCode = string.Empty;

            if (!string.IsNullOrEmpty(current.FormId)
                && (current.FormType == FormType.Incoming))
            {
                return true;
            }

            var orderForm = OrderFormRepository.Get(current.OrderFormId);
            if (string.Compare(orderForm.HospitalId, hospitalId, false) != 0)
            {
                errorCode = GoodsStateValidateCodes.HospitalNoFutureForm;
                return false;
            }

            errorCode = GoodsStateValidateCodes.NotValidState;
            return false;
        }

        private static bool ValidateMovein(GoodsStateEntity current, string hospitalId, out string errorCode)
        {
            errorCode = string.Empty;

            if (!string.IsNullOrEmpty(current.FormId)
                && (current.FormType == FormType.MoveOut))
            {
                return true;
            }

            var orderForm = OrderFormRepository.Get(current.OrderFormId);
            if (string.Compare(orderForm.HospitalId, hospitalId, false) != 0)
            {
                errorCode = GoodsStateValidateCodes.HospitalNoFutureForm;
                return false;
            }

            errorCode = GoodsStateValidateCodes.NotValidState;
            return false;
        }
        #endregion


        #region Update State
        public static bool SetValid(string barcode, string futureFormId, FormType futureFormType, bool valid, string userId)
        {
            var sql = @"update goods_state set 
future_form_id=@p_future_form_id, future_form_type=@p_future_form_type, 
future_created_user=@p_future_created_user, future_created_time=@p_future_created_time,
future_valid=@p_future_valid, future_validate_user=@p_future_created_user, future_validate_time=@p_future_created_time
where barcode=@p_barcode";

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_future_form_id", DbType.String, futureFormId);
            db.AddInParameter(dc, "p_future_form_type", DbType.Int32, (int)futureFormType);
            db.AddInParameter(dc, "p_future_created_user", DbType.String, userId);
            db.AddInParameter(dc, "p_future_created_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_future_valid", DbType.Boolean, valid);
            db.AddInParameter(dc, "p_barcode", DbType.String, barcode);

            return db.ExecuteNonQuery(dc) > 0;
        }

        public static bool SetValid(string futureFormId, FormType futureFormType, bool valid, string userId)
        {
            var sql = @"update goods_state
set future_valid=@p_future_valid, future_validate_user=@p_future_validate_user, future_validate_time=@p_future_validate_time
where future_form_id=@p_future_form_id and future_form_type=@p_future_form_type";

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_future_valid", DbType.Boolean, valid);
            db.AddInParameter(dc, "p_future_validate_user", DbType.String, userId);
            db.AddInParameter(dc, "p_future_validate_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_future_form_id", DbType.String, futureFormId);
            db.AddInParameter(dc, "p_future_form_type", DbType.Int32, (int)futureFormType);

            return db.ExecuteNonQuery(dc) > 0;
        }

        public static bool SetValid(string barcode, bool valid, string userId)
        {
            var sql = @"update goods_state
set future_valid=@p_future_valid, future_validate_user=@p_future_validate_user, future_validate_time=@p_future_validate_time
where barcode=@p_barcode";

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_future_valid", DbType.Boolean, valid);
            db.AddInParameter(dc, "p_future_validate_user", DbType.String, userId);
            db.AddInParameter(dc, "p_future_validate_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_barcode", DbType.String, barcode);

            return db.ExecuteNonQuery(dc) > 0;
        }

        public static void ChangeState(string formId, FormType formType, string futureFormId, FormType futureFormType, string userId, string batchNo, DateTime expiredDate, Database db, DbTransaction trans)
        {
            var sql = @"update goods_state set 
form_id=future_form_id, form_type=future_form_type, 
state_created_user=future_created_user, state_created_time=future_created_time,
state_validate_user=future_validate_user, state_validate_time=future_validate_time, 
state_changed_user=@p_state_changed_user, state_changed_time=@p_state_changed_time,
future_form_id=@p_future_form_id, future_form_type=@p_future_form_type, 
future_created_user=@p_future_created_user, future_created_time=@p_future_created_time,
future_valid=0, batch_no=@p_batch_no, expired_date=@p_expired_date
where future_form_id=@p_form_id and future_form_type=@p_form_type and future_valid=1";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_state_changed_user", DbType.String, userId);
            db.AddInParameter(dc, "p_state_changed_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_future_form_id", DbType.String, futureFormId);
            db.AddInParameter(dc, "p_future_form_type", DbType.Int32, (int)futureFormType);
            db.AddInParameter(dc, "p_future_created_user", DbType.String, userId);
            db.AddInParameter(dc, "p_future_created_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_form_id", DbType.String, formId);
            db.AddInParameter(dc, "p_form_type", DbType.Int32, (int)formType);
            db.AddInParameter(dc, "p_batch_no", DbType.String, batchNo);
            db.AddInParameter(dc, "p_expired_date", DbType.DateTime, expiredDate);

            db.ExecuteNonQuery(dc, trans);

            ArchiveState(formId, formType, db, trans);
        }

        public static void ChangeState(string formId, FormType formType, string futureFormId, FormType futureFormType, string userId, Database db, DbTransaction trans)
        {
            var sql = @"update goods_state set 
form_id=future_form_id, form_type=future_form_type, 
state_created_user=future_created_user, state_created_time=future_created_time,
state_validate_user=future_validate_user, state_validate_time=future_validate_time, 
state_changed_user=@p_state_changed_user, state_changed_time=@p_state_changed_time,
future_form_id=@p_future_form_id, future_form_type=@p_future_form_type, 
future_created_user=@p_future_created_user, future_created_time=@p_future_created_time,
future_valid=0
where future_form_id=@p_form_id and future_form_type=@p_form_type and future_valid=1";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_state_changed_user", DbType.String, userId);
            db.AddInParameter(dc, "p_state_changed_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_future_form_id", DbType.String, futureFormId);
            db.AddInParameter(dc, "p_future_form_type", DbType.Int32, (int)futureFormType);
            db.AddInParameter(dc, "p_future_created_user", DbType.String, userId);
            db.AddInParameter(dc, "p_future_created_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_form_id", DbType.String, formId);
            db.AddInParameter(dc, "p_form_type", DbType.Int32, (int)formType);

            db.ExecuteNonQuery(dc, trans);

            ArchiveState(formId, formType, db, trans);
        }

        public static void ChangeState(string formId, FormType formType, string userId, Database db, DbTransaction trans)
        {
            var sql = @"update goods_state set 
form_id=future_form_id, form_type=future_form_type, 
state_created_user=future_created_user, state_created_time=future_created_time,
state_validate_user=future_validate_user, state_validate_time=future_validate_time, 
state_changed_user=@p_state_changed_user, state_changed_time=@p_state_changed_time,
future_form_id=null, future_form_type=null, 
future_created_user=@p_future_created_user, future_created_time=@p_future_created_time,
future_valid=0
where future_form_id=@p_form_id and future_form_type=@p_form_type and future_valid=1";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_state_changed_user", DbType.String, userId);
            db.AddInParameter(dc, "p_state_changed_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_future_created_user", DbType.String, userId);
            db.AddInParameter(dc, "p_future_created_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_form_id", DbType.String, formId);
            db.AddInParameter(dc, "p_form_type", DbType.Int32, (int)formType);

            db.ExecuteNonQuery(dc, trans);

            ArchiveState(formId, formType, db, trans);
        }

        private static void ArchiveState(string formId, FormType formType, Database db, DbTransaction trans)
        {
            var sql = @"insert into goods_state_archive
(
id, barcode, form_id, form_type, state_created_user, state_created_time,
state_validate_user, state_validate_time, state_changed_user, state_changed_time
)
select 
newid(), barcode, form_id, form_type, state_created_user, state_created_time,
state_validate_user, state_validate_time, state_changed_user, state_changed_time
from goods_state
where form_id=@p_form_id and form_type=@p_form_type";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_form_id", DbType.String, formId);
            db.AddInParameter(dc, "p_form_type", DbType.Int32, (int)formType);

            db.ExecuteNonQuery(dc, trans);
        }

        public static void SetState(string barcode, FormType formType, string formId, string userId, Database db, DbTransaction trans)
        {
            var sql = @"update goods_state set 
form_id=@p_form_id, form_type=@p_form_type, 
state_created_user=@p_user_id, state_created_time=@p_datetime,
state_validate_user=@p_user_id, state_validate_time=@p_datetime, 
state_changed_user=@p_user_id, state_changed_time=@p_datetime,
future_form_id=null, future_form_type=null, 
future_created_user=null, future_created_time=null,
future_validate_user=null, future_validate_time=null,
future_valid=0
where barcode=@p_barcode";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_form_id", DbType.String, formId);
            db.AddInParameter(dc, "p_form_type", DbType.Int32, (int)formType);
            db.AddInParameter(dc, "p_user_id", DbType.String, userId);
            db.AddInParameter(dc, "p_datetime", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_barcode", DbType.String, barcode);

            db.ExecuteNonQuery(dc, trans);

            ArchiveState(barcode, db, trans);
        }

        private static void ArchiveState(string barcode, Database db, DbTransaction trans)
        {
            var sql = @"insert into goods_state_archive
(
id, barcode, form_id, form_type, state_created_user, state_created_time,
state_validate_user, state_validate_time, state_changed_user, state_changed_time
)
select 
newid(), barcode, form_id, form_type, state_created_user, state_created_time,
state_validate_user, state_validate_time, state_changed_user, state_changed_time
from goods_state
where barcode=@p_barcode";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_form_id", DbType.String, barcode);
            db.AddInParameter(dc, "p_barcode", DbType.String, barcode);

            db.ExecuteNonQuery(dc, trans);
        }

        private static void ArchiveState(GoodsStateEntity goodsState, Database db, DbTransaction trans)
        {
            var sql = @"insert into goods_state_archive
(
    id, barcode, form_id, form_type, state_created_user, state_created_time,
    state_validate_user, state_validate_time,state_changed_user, state_changed_time
)
values
(
    @p_id, @p_barcode, @p_form_id, @p_form_type, @p_state_created_user, @p_state_created_time,
    @p_state_validate_user, @p_state_validate_time, @p_state_changed_user, @p_state_changed_time
)";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, Guid.NewGuid().ToString());
            db.AddInParameter(dc, "p_barcode", DbType.String, goodsState.Barcode);
            db.AddInParameter(dc, "p_form_id", DbType.String, goodsState.FormId);
            db.AddInParameter(dc, "p_form_type", DbType.Int32, (int)goodsState.FormType);
            db.AddInParameter(dc, "p_state_created_user", DbType.String, goodsState.StateCreatedUser);
            db.AddInParameter(dc, "p_state_created_time", DbType.DateTime, goodsState.StateCreateTime);
            db.AddInParameter(dc, "p_state_validate_user", DbType.String, goodsState.StateValidateUser);
            db.AddInParameter(dc, "p_state_validate_time", DbType.String, goodsState.StateValidateTime);
            db.AddInParameter(dc, "p_state_changed_user", DbType.String, goodsState.StateChangedUser);
            db.AddInParameter(dc, "p_state_changed_time", DbType.DateTime, goodsState.StateChangedTime);

            db.ExecuteNonQuery(dc, trans);
        }
        #endregion


        #region Query
        public static IList<GoodsStateEntity> QueryInvalid(int orderFormNo, FormType formType, string vendorId, string hospitalId)
        {
            var sql = string.Format(@"select {0} from goods_state where 
order_form_no=@p_order_form_no and future_form_type=@p_future_form_type and future_valid=0
and future_form_id in (select id from dispatch_form where order_form_no=@p_order_form_no and vendor_id=@p_vendor_id and hospital_id=@p_hospital_id and scan_over=0)
", COLUMNS);
            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_order_form_no", DbType.Int32, orderFormNo);
            db.AddInParameter(dc, "p_future_form_type", DbType.Int32, (int)formType);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, vendorId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            var list = new List<GoodsStateEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    var entity = new GoodsStateEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<GoodsStateEntity> QueryInvalid(FormType formType, string vendorId, string hospitalId)
        {
            var sql = string.Format(@"select top 30 {0} from goods_state
where future_form_id in (select id from dispatch_form where vendor_id=@p_vendor_id and hospital_id=@p_hospital_id and scan_over=0)
    and future_form_type=@p_future_form_type and future_valid=0", COLUMNS);
            
            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_future_form_type", DbType.Int32, (int)formType);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, vendorId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            var list = new List<GoodsStateEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new GoodsStateEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }
       
        public static IList<QueryBarcodeEntity> QueryBarcodeDetail(string barcode)
        {
            var list = new List<QueryBarcodeEntity>();
            if (barcode == null || barcode.Trim().Length == 0) return list;
            var sql = @"select top 50 b.id,b.barcode,a.hospital_id,c.name as hospital_name,b.order_form_no,a.filler_id as order_person_id,e.name as order_person_name
,a.apply_time,b.product_id,case when f.alias is null then g.name else f.alias end as product_name,case when b.form_type is null then -1 else b.form_type end as status
,case when b.vendor_id is null then '' else b.vendor_id end vendor_id ,case when b.vendor_id is null then '' else d.name end vendor_name
from orders a, goods_state b ,units c,units d,users e,hospital_products f,products g
where  b.barcode like @p_barcode 
and b.order_form_id=a.id and b.vendor_id=d.id and a.hospital_id=c.id and a.filler_id=e.id
and b.product_id=f.product_id and a.hospital_id=f.hospital_id and b.product_id=g.id order by barcode desc";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_barcode", DbType.String,"%"+barcode+"%");

            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new QueryBarcodeEntity();
                    entity.Init(reader);
                    var barcodeNo = entity.Barcode;
                    var listsub = new List<BarcodeStatusEntity>();
                    sql = @"select * from (
--订单
select b.id as form_id,b.form_no,14 as form_type_id,b.filler_id as operator_id,c.name as operator_name,b.apply_time as status_time 
 from goods_state a,orders b ,users c where a.barcode=@p_barcode and a.order_form_id=b.id and b.filler_id=c.id
union
--销售发货
select b.id,0,15,b.updated_id,c.name,b.updated_time
 from goods_state a,dispatch_form b ,users c where a.barcode=@p_barcode 
 and a.form_id=b.id and b.created_id=c.id
union 
 select a.form_id,0,a.form_type,a.state_validate_user,c.name,a.state_validate_time
 from goods_state_archive a ,users c where a.barcode=@p_barcode  and a.state_validate_user=c.id
 ) aa order by status_time";
                    var dbsub = DatabaseFactory.CreateDatabase();
                    var dcsub = dbsub.GetSqlStringCommand(sql);
                    dbsub.AddInParameter(dcsub, "p_barcode", DbType.String, barcodeNo);
                    using (var readerSub = dbsub.ExecuteReader(dcsub))
                    {
                        while (readerSub.Read())
                        {
                            var entitySub = new BarcodeStatusEntity();
                            entitySub.Init(readerSub);
                            listsub.Add(entitySub);
                        }
                    }
                    entity.StatusDetails = listsub;
                    list.Add(entity);
                }
            }
            return list;
        }
        
        #endregion


        public static int CountValid(string futureFormId, FormType futureFormType)
        {
            return CountValid(futureFormId, futureFormType, null, null);
        }

        public static int CountValid(string futureFormId, FormType futureFormType, Database db, DbTransaction trans)
        {
            var sql = string.Format(@"select count(id) from goods_state {0} where future_form_id=@p_future_form_id and future_form_type=@p_future_form_type and future_valid=1", TransHelper.UpdateLock(trans));

            if (db == null)
            {
                db = DatabaseFactory.CreateDatabase();
            }

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_future_form_id", DbType.String, futureFormId);
            db.AddInParameter(dc, "p_future_form_type", DbType.Int32, (int)futureFormType);

            if (trans == null)
            {
                using (var reader = db.ExecuteReader(dc))
                {
                    reader.Read();

                    return (int)reader.GetValue(0);
                }
            }
            else
            {
                using (var reader = db.ExecuteReader(dc, trans))
                {
                    reader.Read();

                    return (int)reader.GetValue(0);
                }
            }
        }
    }
}
