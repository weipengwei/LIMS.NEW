using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

using LIMS.Util;
using LIMS.Models;
using LIMS.Entities;

namespace LIMS.Repositories
{
    public static class GoodsFlowRepository
    {
        private const string COLUMN_SQL = @"
id, barcode, product_id, order_form_no,
dispatch_form, dispatch_valid, dispatch_time, dispatch_user,
receive_form, receive_valid, receive_time, receive_user,
inspection_form, inspection_valid, inspection_time, inspection_user,
incoming_form, incoming_valid, incoming_time, incoming_user,
return_form, return_valid, return_time, return_user
";

        public static string GetBarcode()
        {
            var baseKey = IdentityCreatorRepository.Get(IdentityKey.GOODS_FLOW, 1);
            return FormatBarcode(baseKey);
        }

        public static void Create(int baseKey, int count, string productId, int orderFormNo, string dispatchFormId, Database db, DbTransaction trans)
        {
            var sql = @"insert into goods_flow(id, barcode, product_id, order_form_no, dispatch_form) 
values(@p_id, @p_barcode, @p_product_id, @p_order_form_no, @p_dispatch_form)";

            for(var i = 0; i < count; i++)
            {
                var barcode = FormatBarcode(baseKey + i);

                var dc = db.GetSqlStringCommand(sql);
                db.AddInParameter(dc, "p_id", DbType.String, Guid.NewGuid().ToString());
                db.AddInParameter(dc, "p_barcode", DbType.String, barcode);
                db.AddInParameter(dc, "p_product_id", DbType.String, productId);
                db.AddInParameter(dc, "p_order_form_no", DbType.Int32, orderFormNo);
                db.AddInParameter(dc, "p_dispatch_form", DbType.String, dispatchFormId);

                db.ExecuteNonQuery(dc, trans);
            }
        }

        private static string FormatBarcode(int baseKey)
        {
            return (10000000000000 + baseKey).ToString().Substring(1);
        }

        public static IList<GoodsFlowEntity> QueryWaitingValid(GoodsFlowValidType validType, bool isValid, string vendorId, string hospitalId)
        {
            var sql = string.Format(@"select top 30 {0} from goods_flow 
where 1=1 {1} order by barcode", COLUMN_SQL, GetFormNoSql(validType, false));

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            if(validType == GoodsFlowValidType.Dispatch)
            {
                db.AddInParameter(dc, "p_vendor_id", DbType.String, vendorId);
            }
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);
            AddValidValue(db, dc, validType, isValid);

            var list = new List<GoodsFlowEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new GoodsFlowEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<GoodsFlowEntity> QueryWaitingValid(GoodsFlowValidType validType, int formNo, bool isValid, string vendorId, string hospitalId)
        {
            var sql = string.Format(@"select top 30 {0} from goods_flow 
where 1=1 {1} order by barcode", COLUMN_SQL, GetFormNoSql(validType, true));

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_form_no", DbType.Int32, formNo);
            if (validType == GoodsFlowValidType.Dispatch)
            {
                db.AddInParameter(dc, "p_vendor_id", DbType.String, vendorId);
            }
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);
            AddValidValue(db, dc, validType, isValid);

            var list = new List<GoodsFlowEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new GoodsFlowEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static void BatchCancelValid(GoodsFlowValidType validType, string formId, string userId)
        {
            var sql = string.Format("update goods_flow set {0} where 1=1 {1}", GetUpdateValidSql(validType), GetFormIdWhere(validType));
            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_is_valid", DbType.Boolean, false);
            db.AddInParameter(dc, "p_user", DbType.String, userId);
            db.AddInParameter(dc, "p_datetime", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_form_id", DbType.String, formId);

            db.ExecuteNonQuery(dc);
        }

        public static void CancelValid(GoodsFlowValidType validType, string barcode, string userId, string vendorId, string hospitalId)
        {
            var sql = string.Format("update goods_flow set {0} where barcode=@p_barcode {1}", 
                GetUpdateValidSql(validType),
                GetFormNoSql(validType, false));
            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_barcode", DbType.String, barcode);
            db.AddInParameter(dc, "p_is_valid", DbType.Boolean, false);
            db.AddInParameter(dc, "p_user", DbType.String, userId);
            db.AddInParameter(dc, "p_datetime", DbType.DateTime, DateTime.Now);

            if (validType == GoodsFlowValidType.Dispatch)
            {
                db.AddInParameter(dc, "p_vendor_id", DbType.String, vendorId);
            }
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);
            AddValidValue(db, dc, validType, true);

            db.ExecuteNonQuery(dc);
        }

        public static void Valid(GoodsFlowValidType validType, string barcode, string userId, string vendorId, string hospitalId)
        {
            var sql = string.Format("update goods_flow set {0} where barcode=@p_barcode {1}", 
                GetUpdateValidSql(validType), GetValidTypeSql(validType));
            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_barcode", DbType.String, barcode);
            db.AddInParameter(dc, "p_is_valid", DbType.Boolean, true);
            db.AddInParameter(dc, "p_user", DbType.String, userId);
            db.AddInParameter(dc, "p_datetime", DbType.DateTime, DateTime.Now);
            if (validType == GoodsFlowValidType.Dispatch)
            {
                db.AddInParameter(dc, "p_vendor_id", DbType.String, vendorId);
            }
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            db.ExecuteNonQuery(dc);
        }

        private static string GetUpdateValidSql(GoodsFlowValidType validType)
        {
            var sql = string.Empty;
            switch(validType)
            {
                case GoodsFlowValidType.Receive:
                    sql = "receive_valid=@p_is_valid, receive_user=@p_user, receive_time=@p_datetime";
                    break;
                case GoodsFlowValidType.Dispatch:
                default:
                    sql = "dispatch_valid=@p_is_valid, dispatch_user=@p_user, dispatch_time=@p_datetime";
                    break;
            }

            return sql;
        }

        private static string GetFormIdWhere(GoodsFlowValidType validType)
        {
            var sql = string.Empty;
            switch (validType)
            {
                case GoodsFlowValidType.Receive:
                    sql = " and receive_form = @p_form_id";
                    break;
                case GoodsFlowValidType.Dispatch:
                default:
                    sql = " and dispatch_form = @p_form_id";
                    break;
            }

            return sql;
        }

        private static string GetValidColumn(GoodsFlowValidType validType)
        {
            var name = string.Empty;
            switch (validType)
            {
                case GoodsFlowValidType.Receive:
                    name = "receive_valid";
                    break;
                case GoodsFlowValidType.Dispatch:
                default:
                    name = "dispatch_valid";
                    break;
            }

            return name;
        }

        private static void AddValidValue(Database db, DbCommand dc, GoodsFlowValidType validType, bool isValid)
        {
            string name;
            switch (validType)
            {
                case GoodsFlowValidType.Receive:
                    name = "p_receive_valid";
                    break;
                case GoodsFlowValidType.Dispatch:
                default:
                    name = "p_dispatch_valid";
                    break;
            }

            db.AddInParameter(dc, name, DbType.Boolean, isValid);
        }

        private static string GetFormNoSql(GoodsFlowValidType validType, bool includeFormNo)
        {
            var sql = string.Empty;
            switch (validType)
            {
                case GoodsFlowValidType.Receive:
                    sql = " and dispatch_valid=1 and receive_valid=@p_receive_valid and receive_form in (select id from receive_form where 1=1 {0} and hospital_id=@p_hospital_id and scan_over=0)";
                    break;
                case GoodsFlowValidType.Dispatch:
                default:
                    sql = " and dispatch_valid=@p_dispatch_valid and dispatch_form in (select id from dispatch_form where 1=1 {0} and vendor_id=@p_vendor_id and hospital_id=@p_hospital_id and scan_over=0)";
                    break;
            }

            sql = string.Format(sql, includeFormNo ? " and order_form_no = @p_form_no " : "");

            return sql;
        }

        private static string GetValidTypeSql(GoodsFlowValidType validType)
        {
            var sql = string.Empty;
            switch (validType)
            {
                case GoodsFlowValidType.Receive:
                    sql = " and dispatch_valid=1 and receive_form in (select id from receive_form where hospital_id=@p_hospital_id and scan_over=0)";
                    break;
                case GoodsFlowValidType.Dispatch:
                default:
                    sql = " and dispatch_form in (select id from dispatch_form where vendor_id=@p_vendor_id and hospital_id=@p_hospital_id and scan_over=0)";
                    break;
            }
            
            return sql;
        }


        public static GoodsFlowEntity GetByBarcode(GoodsFlowValidType validType, string barcode, string vendorId, string hospitalId)
        {
            var sql = string.Format(@"select {0} from goods_flow 
where 1=1 and barcode=@p_barcode {1}", COLUMN_SQL, GetValidTypeSql(validType));

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            if (validType == GoodsFlowValidType.Dispatch)
            {
                db.AddInParameter(dc, "p_vendor_id", DbType.String, vendorId);
            }
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(dc, "p_barcode", DbType.String, barcode);

            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new GoodsFlowEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static GoodsFlowEntity GetByBarcode(string barcode)
        {
            var sql = string.Format("select {0} from goods_flow where barcode = @p_barcode", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_barcode", DbType.String, barcode);

            GoodsFlowEntity entity = null;
            using (var reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    entity = new GoodsFlowEntity();
                    entity.Init(reader);

                    break;
                }
            }

            return entity;
        }


        public static void SaveReceiveForm(string dispatchForm, string receiveForm, Database db, DbTransaction trans)
        {
            var sql = @"update goods_flow set receive_form = @p_receive_form, receive_valid = 0
where dispatch_form=@p_dispatch_form and dispatch_valid = 1";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_dispatch_form", DbType.String, dispatchForm);
            db.AddInParameter(dc, "p_receive_form", DbType.String, receiveForm);

            db.ExecuteNonQuery(dc, trans);
        }


        #region Sum valid
        public static int SumValid(GoodsFlowValidType validType, string validFormId)
        {
            var sql = "select count(*) from goods_flow where 1=1 " + GetSumWhere(validType);
            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_form", DbType.String, validFormId);

            int count = 0;
            using (var reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    count = Convert.ToInt32(reader[0]);
                    break;
                }
            }

            return count;
        }

        private static string GetSumWhere(GoodsFlowValidType validType)
        {
            string sql;
            switch(validType)
            {
                case GoodsFlowValidType.Incoming:
                    sql = " and incoming_valid=1 and incoming_form=@p_form";
                    break;
                case GoodsFlowValidType.Inspection:
                    sql = " and inspection_valid=1 and inspection_form=@p_form";
                    break;
                case GoodsFlowValidType.Receive:
                    sql =  " and receive_valid=1 and receive_form=@p_form";
                    break;
                default:
                    sql = " and dispatch_valid=1 and dispatch_form=@p_form";
                    break;
            }

            return sql;
        }
        #endregion



        #region Inspection
        public static void SaveInspectionForm(string receiveForm, string inspectionForm, Database db, DbTransaction trans)
        {
            var sql = @"update goods_flow set inspection_form = @p_inspection_form, inspection_valid = 0
where receive_form=@p_receive_form and receive_valid = 1";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_receive_form", DbType.String, receiveForm);
            db.AddInParameter(dc, "p_inspection_form", DbType.String, inspectionForm);

            db.ExecuteNonQuery(dc, trans);
        }

        public static bool ValidInspection(string barcode, string userId, string hospitalId)
        {
            var sql = @"update goods_flow 
set inspection_valid = 1, inspection_user = @p_inspection_user, inspection_time = @p_inspection_time
where barcode=@p_barcode and inspection_valid = 0 and inspection_form in (select id from inspection_form where hospital_id = @p_hospital_id and scan_over = 0)";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_barcode", DbType.String, barcode);
            db.AddInParameter(dc, "p_inspection_user", DbType.String, userId);
            db.AddInParameter(dc, "p_inspection_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            return db.ExecuteNonQuery(dc) > 0;
        }

        public static bool CancelInspection(string barcode, string userId, string hospitalId)
        {
            var sql = @"update goods_flow 
set inspection_valid = 0, inspection_user = @p_inspection_user, inspection_time = @p_inspection_time
where barcode=@p_barcode and inspection_valid = 1 and inspection_form in (select id from inspection_form where hospital_id = @p_hospital_id and scan_over = 0)";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_barcode", DbType.String, barcode);
            db.AddInParameter(dc, "p_inspection_user", DbType.String, userId);
            db.AddInParameter(dc, "p_inspection_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            return db.ExecuteNonQuery(dc) > 0;
        }

        public static void BatchCancelInspection(string inspectionId, string userId, string hospitalId)
        {
            var sql = @"update goods_flow 
set inspection_valid = 0, inspection_user = @p_inspection_user, inspection_time = @p_inspection_time
where inspection_valid = 1 and inspection_form in (select id from inspection_form where id = @p_inspection_Id and hospital_id = @p_hospital_id and scan_over = 0)";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_inspection_Id", DbType.String, inspectionId);
            db.AddInParameter(dc, "p_inspection_user", DbType.String, userId);
            db.AddInParameter(dc, "p_inspection_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            db.ExecuteNonQuery(dc);
        }
        #endregion



        #region Incoming
        public static void SaveIncomingFormByReceiveForm(string receiveForm, string incomingForm, Database db, DbTransaction trans)
        {
            var sql = @"update goods_flow set incoming_form = @p_incoming_form, incoming_valid = 0
where receive_form=@p_receive_form and receive_valid = 1";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_receive_form", DbType.String, receiveForm);
            db.AddInParameter(dc, "p_incoming_form", DbType.String, incomingForm);

            db.ExecuteNonQuery(dc, trans);
        }

        public static void SaveIncomingFormByInspectionForm(string inspectionForm, string incomingForm, Database db, DbTransaction trans)
        {
            var sql = @"update goods_flow set incoming_form = @p_incoming_form, incoming_valid = 0
where inspection_form=@p_inspection_form and inspection_valid = 1";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_inspection_form", DbType.String, inspectionForm);
            db.AddInParameter(dc, "p_incoming_form", DbType.String, incomingForm);

            db.ExecuteNonQuery(dc, trans);
        }

        public static bool ValidIncoming(string barcode, string userId, string hospitalId)
        {
            var sql = @"update goods_flow 
set incoming_valid = 1, incoming_user = @p_incoming_user, incoming_time = @p_incoming_time
where barcode=@p_barcode and incoming_valid = 0 and incoming_form in (select id from incoming_form where hospital_id = @p_hospital_id and scan_over = 0)";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_barcode", DbType.String, barcode);
            db.AddInParameter(dc, "p_incoming_user", DbType.String, userId);
            db.AddInParameter(dc, "p_incoming_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            return db.ExecuteNonQuery(dc) > 0;
        }

        public static bool CancelIncoming(string barcode, string userId, string hospitalId)
        {
            var sql = @"update goods_flow 
set incoming_valid = 0, incoming_user = @p_incoming_user, incoming_time = @p_incoming_time
where barcode=@p_barcode and incoming_valid = 1 and incoming_form in (select id from incoming_form where hospital_id = @p_hospital_id and scan_over = 0)";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_barcode", DbType.String, barcode);
            db.AddInParameter(dc, "p_incoming_user", DbType.String, userId);
            db.AddInParameter(dc, "p_incoming_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            return db.ExecuteNonQuery(dc) > 0;
        }

        public static void BatchCancelIncoming(string incomingId, string userId, string hospitalId)
        {
            var sql = @"update goods_flow 
set incoming_valid = 0, incoming_user = @p_incoming_user, incoming_time = @p_incoming_time
where incoming_valid = 1 and incoming_form in (select id from incoming_form where id = @p_incoming_Id and hospital_id = @p_hospital_id and scan_over = 0)";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_incoming_Id", DbType.String, incomingId);
            db.AddInParameter(dc, "p_incoming_user", DbType.String, userId);
            db.AddInParameter(dc, "p_incoming_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            db.ExecuteNonQuery(dc);
        }
        #endregion



        #region Return
        public static bool ValidReturn(string barcode, string returnForm, string userId)
        {
            var sql = @"update goods_flow 
set return_user = @p_return_user, return_time = @p_return_time, return_form = @p_return_form, return_valid = 1
where barcode=@p_barcode";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_barcode", DbType.String, barcode);
            db.AddInParameter(dc, "p_return_user", DbType.String, userId);
            db.AddInParameter(dc, "p_return_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_return_form", DbType.String, returnForm);

            return db.ExecuteNonQuery(dc) > 0;
        }

        public static bool CancelReturn(string barcode, string returnForm, string userId)
        {
            var sql = @"update goods_flow 
set return_user = @p_return_user, return_time = @p_return_time, return_valid = 0
where barcode=@p_barcode and return_form = @p_return_form";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_barcode", DbType.String, barcode);
            db.AddInParameter(dc, "p_return_user", DbType.String, userId);
            db.AddInParameter(dc, "p_return_time", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dc, "p_return_form", DbType.String, returnForm);

            return db.ExecuteNonQuery(dc) > 0;
        }
        #endregion
    }
}
