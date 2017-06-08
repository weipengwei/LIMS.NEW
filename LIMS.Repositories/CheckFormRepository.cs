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
    public static class CheckFormRepository
    {
        private const string COLUMN_SQL = "id,name,storeroom_id,hospital_id,status,created_id,created_time,updated_id,updated_time";

        public static CheckFormEntity Get(string id)
        {
            var sql = string.Format("select {0} from check_form where id=@p_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, id);

            using(var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    var entity = new CheckFormEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static IList<CheckFormCategoryEntity> GetCategories(string id)
        {
            var sql = "select id,check_id,category from check_form_categories where check_id=@p_check_id order by category";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_check_id", DbType.String, id);

            var list = new List<CheckFormCategoryEntity>();
            using(var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new CheckFormCategoryEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<CheckFormProductEntity> GetProducts(string formId)
        {
            var sql = @"
select 
    id,check_id,category,product_id,inv_standard_count,inv_using_package_count,
    check_standard_count,check_using_package_count,status,
    operator_id,operated_time,check_over,created_id,created_time
from check_form_products where check_id=@check_id order by category";

            var db = DatabaseFactory.CreateDatabase();

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "check_id", DbType.String, formId);

            var list = new List<CheckFormProductEntity>();
            using(var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new CheckFormProductEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static CheckFormProductEntity GetCheckProduct(string formId, string productId)
        {
            var sql = @"
select 
    id,check_id,category,product_id,inv_standard_count,inv_using_package_count,
    check_standard_count,check_using_package_count,status,
    operator_id,operated_time,check_over,created_id,created_time
from check_form_products where check_id=@check_id and product_id=@p_product_id";

            var db = DatabaseFactory.CreateDatabase();

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "check_id", DbType.String, formId);
            db.AddInParameter(cmd, "p_product_id", DbType.String, productId);

            var list = new List<CheckFormProductEntity>();
            using (var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    var entity = new CheckFormProductEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        #region Save Form
        public static void Save(CheckFormEntity form, IList<CheckFormCategoryEntity> categories)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(form.Id))
                        {
                            Create(form, db, trans);
                        }
                        else
                        {
                            Update(form, db, trans);
                            DeleteCategories(form.Id, db, trans);
                        }

                        CreateCategories(form.Id, categories, db, trans);
                        AnalyseProducts(form.Id, db, trans);

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

        private static void Create(CheckFormEntity form, Database db, DbTransaction trans)
        {
            var sql = @"insert into check_form(id,name,storeroom_id,hospital_id,status,created_id,created_time)
values(@p_id,@p_name,@p_storeroom_id,@p_hospital_id,@p_status,@p_created_id,@p_created_time)";

            form.Id = Guid.NewGuid().ToString();

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, form.Id);
            db.AddInParameter(cmd, "p_name", DbType.String, form.Name);
            db.AddInParameter(cmd, "p_storeroom_id", DbType.String, form.StoreroomId);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, form.HospitalId);
            db.AddInParameter(cmd, "p_status", DbType.String, form.Status);
            db.AddInParameter(cmd, "p_created_id", DbType.String, form.CreatedId);
            db.AddInParameter(cmd, "p_created_time", DbType.DateTime, form.CreatedTime);

            db.ExecuteNonQuery(cmd, trans);
        }

        private static void Update(CheckFormEntity form, Database db, DbTransaction trans)
        {
            var sql = "update check_form set name=@p_name,storeroom_id=@p_storeroom_id where id=@p_id";

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, form.Id);
            db.AddInParameter(cmd, "p_name", DbType.String, form.Name);
            db.AddInParameter(cmd, "p_storeroom_id", DbType.String, form.StoreroomId);

            db.ExecuteNonQuery(cmd, trans);
        }

        private static void DeleteCategories(string checkId, Database db, DbTransaction trans)
        {
            var sql = "delete check_form_categories where check_id=@p_check_id";

            var cmd = db.GetSqlStringCommand(sql);
            
            db.AddInParameter(cmd, "p_check_id", DbType.String, checkId);

            db.ExecuteNonQuery(cmd, trans);
        }

        private static void CreateCategories(string checkId, IList<CheckFormCategoryEntity> categories, Database db, DbTransaction trans)
        {
            var sql = "insert into check_form_categories(id,check_id,category) values(@p_id,@p_check_id,@p_category)";

            foreach(var category in categories)
            {
                var cmd = db.GetSqlStringCommand(sql);

                category.Id = Guid.NewGuid().ToString();
                category.CheckId = checkId;

                db.AddInParameter(cmd, "p_id", DbType.String, category.Id);
                db.AddInParameter(cmd, "p_check_id", DbType.String, category.CheckId);
                db.AddInParameter(cmd, "p_category", DbType.String, category.Category);

                db.ExecuteNonQuery(cmd, trans);
            }
        }

        private static void AnalyseProducts(string checkId, Database db, DbTransaction trans)
        {
            var cmd = db.GetStoredProcCommand("dbo.sp_check_form_analytics", checkId);
            db.ExecuteNonQuery(cmd, trans);
        }

        private static string FormatCheckCode(int baseKey)
        {
            var today = DateTime.Today;
            var date = (today.Year * 10000 + today.Month * 100 + today.Day).ToString().Substring(2);

            return date + baseKey.ToString().PadLeft(7);
        }
        #endregion

        #region Query
        public static IList<CheckFormEntity> Query(CheckCondition condition, PagerInfo pager)
        {
            pager.ComputePageCount(QueryCount(condition));

            var list = new List<CheckFormEntity>();


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
                orderSql += "created_time DESC";
            }

            var sql = string.Format(@"SELECT {0} FROM check_form WHERE {1}", COLUMN_SQL, GetConditionSql(condition));

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
                    var entity = new CheckFormEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        private static int QueryCount(CheckCondition condition)
        {
            var sql = "SELECT COUNT(id) FROM check_form WHERE ";

            var conditionSql = GetConditionSql(condition);
            if (!string.IsNullOrEmpty(conditionSql))
            {
                sql += conditionSql;
            }

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            AddParameter(dc, db, condition);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                reader.Read();

                return reader[0] == DBNull.Value ? 0 : reader.GetInt32(0);
            }
        }

        private static void AddParameter(DbCommand dc, Database db, CheckCondition condition)
        {
            db.AddInParameter(dc, "p_user_id", DbType.String, condition.UserId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, condition.HospitalId);

            if (condition is CheckScanCondition)
            {
                db.AddInParameter(dc, "p_checking", DbType.String, CheckFormStatus.Checking);
            }

            if (!string.IsNullOrEmpty(condition.StoreroomId))
            {
                db.AddInParameter(dc, "p_storeroom_id", DbType.String, condition.StoreroomId);
            }

            if (!string.IsNullOrEmpty(condition.Status))
            {
                db.AddInParameter(dc, "p_status", DbType.String, condition.Status);
            }
        }

        private static string GetConditionSql(CheckCondition condition)
        {
            var conditionSql = " 1=1 and hospital_id=@p_hospital_id and storeroom_id in (select unit_id from user_privilege where user_id = @p_user_id AND unit_root_id=@p_hospital_id and operate=1)";

            if(condition is CheckScanCondition)
            {
                conditionSql += " and status in (@p_checking)";
            }

            if (!string.IsNullOrEmpty(condition.StoreroomId))
            {
                conditionSql += " and storeroom_id=@p_storeroom_id";
            }

            if (!string.IsNullOrEmpty(condition.Status))
            {
                conditionSql += " and status=@p_status";
            }

            return conditionSql;
        }
        #endregion


        public static void UpdateStatus(string id, string status, string userId)
        {
            var sql = "update check_form set status=@p_status,updated_id=@p_updated_id,updated_time=@p_updated_time where id=@p_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, id);
            db.AddInParameter(cmd, "p_status", DbType.String, status);
            db.AddInParameter(cmd, "p_updated_id", DbType.String, userId);
            db.AddInParameter(cmd, "p_updated_time", DbType.String, DateTime.Now);

            db.ExecuteNonQuery(cmd);
        }

        private static void UpdateStatus(string id, string status, string userId, Database db, DbTransaction trans)
        {
            var sql = "update check_form set status=@p_status,updated_id=@p_updated_id,updated_time=@p_updated_time where id=@p_id";
            
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, id);
            db.AddInParameter(cmd, "p_status", DbType.String, status);
            db.AddInParameter(cmd, "p_updated_id", DbType.String, userId);
            db.AddInParameter(cmd, "p_updated_time", DbType.String, DateTime.Now);

            db.ExecuteNonQuery(cmd, trans);
        }

        
        #region Check Check Product
        public static int UpdateCheckProducts(string checkId, IList<CheckFormProductEntity> checkProducts, string userId)
        {
            var noMatchCount = 0;
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ClearCheckOperator(checkId, userId, db, trans);
                        if(checkProducts != null && checkProducts.Count > 0)
                        {
                            foreach (var item in checkProducts)
                            {
                                UpdateCheckProduct(item, db, trans);
                            }
                        }

                        noMatchCount = ComputeCheckProducts(checkId, userId, db, trans);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return noMatchCount;
        }

        private static void ClearCheckOperator(string checkId, string userId, Database db, DbTransaction trans)
        {
            var sql = @"update check_form_products 
set check_standard_count=null,check_using_package_count=null,operator_id=null,operated_time=null,status=@p_status 
where check_id=@p_check_id and operator_id=@p_operator_id and check_over=@p_check_over";

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_status", DbType.String, CheckProductStatus.NoCheck);
            db.AddInParameter(cmd, "p_check_id", DbType.String, checkId);
            db.AddInParameter(cmd, "p_operator_id", DbType.String, userId);
            db.AddInParameter(cmd, "p_check_over", DbType.Boolean, false);

            db.ExecuteNonQuery(cmd, trans);
        }

        private static void UpdateCheckProduct(CheckFormProductEntity checkProduct, Database db, DbTransaction trans)
        {
            var sql = @"update check_form_products 
set check_standard_count=@p_standard_count,check_using_package_count=@p_package_count,
    operator_id=@p_operator_id,operated_time=@p_operated_time
where id=@p_id and check_over=@p_check_over";
            
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, checkProduct.Id);
            db.AddInParameter(cmd, "p_standard_count", DbType.Int32, checkProduct.CheckStandardCount);
            db.AddInParameter(cmd, "p_package_count", DbType.Int32, checkProduct.CheckUsingPackageCount);
            db.AddInParameter(cmd, "p_operator_id", DbType.String, checkProduct.OperatorId);
            db.AddInParameter(cmd, "p_operated_time", DbType.DateTime, checkProduct.OperatedTime);
            db.AddInParameter(cmd, "p_check_over", DbType.Boolean, false);

            db.ExecuteNonQuery(cmd, trans);
        }

        private static int ComputeCheckProducts(string checkId, string userId, Database db, DbTransaction trans)
        {
            var cmd = db.GetStoredProcCommand("dbo.sp_compute_check_form_products", checkId, userId);

            using (var reader = db.ExecuteReader(cmd, trans))
            {
                if (reader.Read())
                {
                    return reader.GetInt32(0);
                }
                else
                {
                    return 0;
                }
            }
        }

        public static bool IsAllMatch(string checkId, string userId)
        {
            var sql = @"select count(*) num from check_form_products where check_id=@p_check_id and operator_id=@p_operator_id and status=@p_status and check_over=@p_check_over";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_check_id", DbType.String, checkId);
            db.AddInParameter(cmd, "p_operator_id", DbType.String, userId);
            db.AddInParameter(cmd, "p_status", DbType.String, CheckProductStatus.NoMatch);
            db.AddInParameter(cmd, "p_check_over", DbType.Boolean, false);

            using (var reader = db.ExecuteReader(cmd))
            {
                reader.Read();

                return reader.GetInt32(0) == 0;
            }
        }

        public static void HandleCheckProducts(string checkId, string userId)
        {
            var sql = @"update check_form_products set status=@p_new_status where check_id=@p_check_id and status=@p_status and operator_id=@p_operator_id and check_over=@p_check_over";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_new_status", DbType.String, CheckProductStatus.Handle);
            db.AddInParameter(cmd, "p_check_id", DbType.String, checkId);
            db.AddInParameter(cmd, "p_status", DbType.String, CheckProductStatus.NoMatch);
            db.AddInParameter(cmd, "p_operator_id", DbType.String, userId);
            db.AddInParameter(cmd, "p_check_over", DbType.Boolean, false);

            db.ExecuteNonQuery(cmd);
        }

        public static void OverCheckProducts(string checkId, string userId)
        {
            var sql = @"update check_form_products set check_over=@p_check_over where check_id=@p_check_id and operator_id=@p_operator_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_check_id", DbType.String, checkId);
            db.AddInParameter(cmd, "p_operator_id", DbType.String, userId);
            db.AddInParameter(cmd, "p_check_over", DbType.Boolean, true);

            db.ExecuteNonQuery(cmd);
        }

        public static void RedoCheckProducts(string checkId, string userId)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        RedoCheckProducts(checkId, userId, db, trans);
                        ClearAdjustType(checkId, userId, db, trans);

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

        private static void RedoCheckProducts(string checkId, string userId, Database db, DbTransaction trans)
        {
            var sql = @"update check_form_products set check_over=@p_check_over where check_id=@p_check_id and operator_id=@p_operator_id";
            
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_check_id", DbType.String, checkId);
            db.AddInParameter(cmd, "p_operator_id", DbType.String, userId);
            db.AddInParameter(cmd, "p_check_over", DbType.Boolean, false);

            db.ExecuteNonQuery(cmd);
        }

        private static void ClearAdjustType(string checkId, string userId, Database db, DbTransaction trans)
        {
            var sql = @"update check_form_products_validation set adjust_type=null where check_id=@p_check_id and created_id=@p_created_id and status<>@p_match";
            
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_check_id", DbType.String, checkId);
            db.AddInParameter(cmd, "p_created_id", DbType.String, userId);
            db.AddInParameter(cmd, "p_match", DbType.String, CheckProductValidationStatus.Match);

            db.ExecuteNonQuery(cmd);
        }
        
        public static IList<CheckFormProductEntity> GetHandleCheckProducts(string formId, string userId)
        {
            var sql = @"
select 
    id,check_id,category,product_id,inv_standard_count,inv_using_package_count,
    check_standard_count,check_using_package_count,status,
    operator_id,operated_time,check_over,created_id,created_time
from check_form_products where check_id=@check_id and status=@p_status and operator_id=@p_user_id";

            var db = DatabaseFactory.CreateDatabase();

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "check_id", DbType.String, formId);
            db.AddInParameter(cmd, "p_status", DbType.String, CheckProductStatus.Handle);
            db.AddInParameter(cmd, "p_user_id", DbType.String, userId);

            var list = new List<CheckFormProductEntity>();
            using (var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new CheckFormProductEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<CheckFormProductEntity> GetHandledCheckProducts(string formId)
        {
            var sql = @"
select 
    id,check_id,category,product_id,inv_standard_count,inv_using_package_count,
    check_standard_count,check_using_package_count,status,
    operator_id,operated_time,check_over,created_id,created_time
from check_form_products where check_id=@check_id and status=@p_status";

            var db = DatabaseFactory.CreateDatabase();

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "check_id", DbType.String, formId);
            db.AddInParameter(cmd, "p_status", DbType.String, CheckProductStatus.Handle);

            var list = new List<CheckFormProductEntity>();
            using (var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new CheckFormProductEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static void ReturnCheckProducts(string checkId, string userId)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ReturnCheckProducts(checkId, userId, db, trans);
                        DeleteCheckProductionValidation(checkId, userId, db, trans);

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

        private static void ReturnCheckProducts(string checkId, string userId, Database db, DbTransaction trans)
        {
            var sql = @"update check_form_products set status=@p_new_status where check_id=@p_check_id and status=@p_status and operator_id=@p_operator_id and check_over=@p_check_over";
            
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_new_status", DbType.String, CheckProductStatus.NoMatch);
            db.AddInParameter(cmd, "p_check_id", DbType.String, checkId);
            db.AddInParameter(cmd, "p_status", DbType.String, CheckProductStatus.Handle);
            db.AddInParameter(cmd, "p_operator_id", DbType.String, userId);
            db.AddInParameter(cmd, "p_check_over", DbType.Boolean, false);

            db.ExecuteNonQuery(cmd);
        }
        
        private static void DeleteCheckProductionValidation(string checkId, string userId, Database db, DbTransaction trans)
        {
            var sql = @"delete check_form_products_validation where check_id=@p_check_id 
and product_id in (select product_id from check_form_products where check_id=@p_check_id and operator_id=@p_operator_id and check_over=@p_check_over)";

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_check_id", DbType.String, checkId);
            db.AddInParameter(cmd, "p_operator_id", DbType.String, userId);
            db.AddInParameter(cmd, "p_check_over", DbType.Boolean, false);

            db.ExecuteNonQuery(cmd, trans);
        }
        #endregion


        #region Edit Products Validation
        public static CheckFormProductValidationEntity GetProductValidation(string id)
        {
            var sql = @"
select 
    id,check_id,product_id,barcode,inv_storeroom_id,inv_is_standard,inv_count,
    check_is_standard,check_count,status,adjust_type,created_id,created_time 
from check_form_products_validation
where id=@p_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, id);

            using (var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    var entity = new CheckFormProductValidationEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static CheckFormProductValidationEntity GetProductValidation(string formId, string barcode)
        {
            var sql = @"
select 
    id,check_id,product_id,barcode,inv_storeroom_id,inv_is_standard,inv_count,
    check_is_standard,check_count,status,adjust_type,created_id,created_time 
from check_form_products_validation
where check_id=@p_check_id and barcode=@p_barcode";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_check_id", DbType.String, formId);
            db.AddInParameter(cmd, "p_barcode", DbType.String, barcode);

            using(var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    var entity = new CheckFormProductValidationEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static void SaveProductValidation(CheckFormProductValidationEntity entity)
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                Create(entity);
            }
            else
            {
                Update(entity);
            }
        }

        private static void Create(CheckFormProductValidationEntity entity)
        {
            var sql = @"insert into check_form_products_validation
(
    id,check_id,product_id,barcode,inv_storeroom_id,inv_is_standard,inv_count,
    check_is_standard,check_count,status,created_id,created_time)
values
(   
    @p_id,@p_check_id,@p_product_id,@p_barcode,@p_inv_storeroom_id,@p_inv_is_standard,@p_inv_count,
    @p_check_is_standard,@p_check_count,@p_status,@p_created_id,@p_created_time
)";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            entity.Id = Guid.NewGuid().ToString();

            db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);
            db.AddInParameter(cmd, "p_check_id", DbType.String, entity.CheckId);
            db.AddInParameter(cmd, "p_product_id", DbType.String, entity.ProductId);
            db.AddInParameter(cmd, "p_barcode", DbType.String, entity.Barcode);
            db.AddInParameter(cmd, "p_inv_storeroom_id", DbType.String, entity.InvStoreroomId);
            db.AddInParameter(cmd, "p_inv_is_standard", DbType.Boolean, entity.InvIsStandard.HasValue ? (object)entity.InvIsStandard.Value : DBNull.Value);
            db.AddInParameter(cmd, "p_inv_count", DbType.Int32, entity.InvCount.HasValue ? (object)entity.InvCount.Value : DBNull.Value);
            db.AddInParameter(cmd, "p_check_is_standard", DbType.Boolean, entity.CheckIsStandard.HasValue ? (object)entity.CheckIsStandard.Value : DBNull.Value);
            db.AddInParameter(cmd, "p_check_count", DbType.Int32, entity.CheckCount.HasValue ? (object)entity.CheckCount.Value : DBNull.Value);
            db.AddInParameter(cmd, "p_status", DbType.String, entity.Status);
            db.AddInParameter(cmd, "p_created_id", DbType.String, entity.CreatedId);
            db.AddInParameter(cmd, "p_created_time", DbType.DateTime, entity.CreatedTime);

            db.ExecuteNonQuery(cmd);
        }

        private static void Update(CheckFormProductValidationEntity entity)
        {
            var sql = @"update check_form_products_validation set 
check_is_standard=@p_check_is_standard,check_count=@p_check_count,status=@p_status,
created_id=@p_created_id,created_time=@p_created_time
where id=@p_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);
            db.AddInParameter(cmd, "p_check_is_standard", DbType.Boolean, entity.CheckIsStandard);
            db.AddInParameter(cmd, "p_check_count", DbType.Int32, entity.CheckCount);
            db.AddInParameter(cmd, "p_status", DbType.String, entity.Status);
            db.AddInParameter(cmd, "p_created_id", DbType.String, entity.CreatedId);
            db.AddInParameter(cmd, "p_created_time", DbType.DateTime, entity.CreatedTime);

            db.ExecuteNonQuery(cmd);
        }

        public static void Compute(string id, string userId)
        {
            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetStoredProcCommand("dbo.sp_compute_check_form", id, userId);

            db.ExecuteNonQuery(cmd);
        }

        public static IList<CheckFormProductValidationEntity> GetProductsValidation(string formId, string userId)
        {
            var list = new List<CheckFormProductValidationEntity>();

            var sql = @"
select 
    id,check_id,product_id,barcode,inv_storeroom_id,inv_is_standard,inv_count,
    check_is_standard,check_count,status,adjust_type,created_id,created_time 
from check_form_products_validation
where check_id=@p_check_id and status in (@p_init_status, @p_storeroom_status, @p_count_status, @p_no_exist, @p_in_system) and created_id=@p_created_id order by barcode";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_check_id", DbType.String, formId);
            db.AddInParameter(cmd, "p_init_status", DbType.String, CheckProductValidationStatus.Init);
            db.AddInParameter(cmd, "p_storeroom_status", DbType.String, CheckProductValidationStatus.Storeroom);
            db.AddInParameter(cmd, "p_count_status", DbType.String, CheckProductValidationStatus.Count);
            db.AddInParameter(cmd, "p_no_exist", DbType.String, CheckProductValidationStatus.NoExist);
            db.AddInParameter(cmd, "p_in_system", DbType.String, CheckProductValidationStatus.InSystem);
            db.AddInParameter(cmd, "p_created_id", DbType.String, userId);

            using (var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new CheckFormProductValidationEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<CheckFormProductValidationEntity> GetProductsValidation(string formId)
        {
            var sql = @"
select 
    id,check_id,product_id,barcode,inv_storeroom_id,inv_is_standard,inv_count,
    check_is_standard,check_count,status,adjust_type,created_id,created_time 
from check_form_products_validation
where check_id=@p_check_id and status in (@p_storeroom_status, @p_count_status, @p_no_exist, @p_in_system) order by barcode";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_check_id", DbType.String, formId);
            db.AddInParameter(cmd, "p_storeroom_status", DbType.String, CheckProductValidationStatus.Storeroom);
            db.AddInParameter(cmd, "p_count_status", DbType.String, CheckProductValidationStatus.Count);
            db.AddInParameter(cmd, "p_no_exist", DbType.String, CheckProductValidationStatus.NoExist);
            db.AddInParameter(cmd, "p_in_system", DbType.String, CheckProductValidationStatus.InSystem);

            var list = new List<CheckFormProductValidationEntity>();
            using (var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new CheckFormProductValidationEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }
        #endregion


        #region Handle Check Product

        public static void UpdateAdjustType(string checkId, string id, string adjustType)
        {
            var sql = "update check_form_products_validation set adjust_type=@p_adjust_type where id=@p_id and check_id=@p_check_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_adjust_type", DbType.String, adjustType);
            db.AddInParameter(cmd, "p_id", DbType.String, id);
            db.AddInParameter(cmd, "p_check_id", DbType.String, checkId);

            db.ExecuteNonQuery(cmd);
        }

        public static int CountUnhandle(string checkId)
        { 
var sql = @"
select count(*) number
from check_form_products
where check_id=@p_check_id and check_over=@p_check_over";

        var db = DatabaseFactory.CreateDatabase();
        var cmd = db.GetSqlStringCommand(sql);
        db.AddInParameter(cmd, "p_check_id", DbType.String, checkId);
            db.AddInParameter(cmd, "p_check_over", DbType.String, false);

            var list = new List<CheckFormProductValidationEntity>();
            using (var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    return reader.GetInt32(0);
                }
            }

            return 0;
        }
        public static int CountUnadjust(string checkId)
        {
            var sql = @"
select count(*) number
from check_form_products_validation
where check_id=@p_check_id and adjust_type is null";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_check_id", DbType.String, checkId);

            var list = new List<CheckFormProductValidationEntity>();
            using (var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    return reader.GetInt32(0);
                }
            }

            return 0;
        }

        public static void AdjustInventory(string id, string userId)
        {

            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        AdjustInventory(id, db, trans);
                        UpdateStatus(id, CheckFormStatus.Complete, userId, db, trans);

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

        private static void AdjustInventory(string id, Database db, DbTransaction trans)
        {
            var cmd = db.GetStoredProcCommand("dbo.sp_adjust_storeroom_inventory", id);

            db.ExecuteNonQuery(cmd, trans);
        }
        #endregion
    }
}
