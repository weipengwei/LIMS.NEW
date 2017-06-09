using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using LIMS.Entities;
using LIMS.Models;

namespace LIMS.Repositories
{
    public static class ProductRepository
    {
        private static string COLUMN_SQL = @"id, name, full_name, description, brand, short_code, 
mini_package_unit, mini_package_spec, mini_package_count, package_unit, 
category, barcode, barcode_url, is_local,
created_id, created_time, updated_id, updated_time,register_number,ValidDate";

        public static void Add(ProductEntity product)
        {
            var sql = string.Format(@"INSERT INTO products({0})
VALUES(@p_id, @p_name, @p_full_name, @p_description, @p_brand, @p_short_code,
@p_mini_package_unit, @p_mini_package_spec, @p_mini_package_count, @p_package_unit,
@p_category, @p_barcode, @p_barcode_url, @p_is_local,
@p_created_id, @p_created_time, @p_updated_id, @p_updated_time,@p_barcode_url,@p_register_number,@p_ValidDate)", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, product.Id);
            db.AddInParameter(dc, "p_name", DbType.String, product.Name);
            db.AddInParameter(dc, "p_full_name", DbType.String, product.FullName);
            db.AddInParameter(dc, "p_description", DbType.String, product.Description);
            db.AddInParameter(dc, "p_brand", DbType.String, product.Brand);
            db.AddInParameter(dc, "p_short_code", DbType.String, product.ShortCode);
            db.AddInParameter(dc, "p_mini_package_unit", DbType.String, product.MiniPackageUnit);
            db.AddInParameter(dc, "p_mini_package_spec", DbType.String, product.MiniPackageSpec);
            db.AddInParameter(dc, "p_mini_package_count", DbType.Int32, product.MiniPackageCount);
            db.AddInParameter(dc, "p_package_unit", DbType.String, product.PackageUnit);
            db.AddInParameter(dc, "p_category", DbType.String, product.Category);
            db.AddInParameter(dc, "p_barcode", DbType.String, product.Barcode);
            db.AddInParameter(dc, "p_barcode_url", DbType.String, product.BarcodeUrl);
            db.AddInParameter(dc, "p_is_local", DbType.Boolean, product.IsLocal);
            db.AddInParameter(dc, "p_created_id", DbType.String, product.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, product.CreatedTime);
            db.AddInParameter(dc, "p_updated_id", DbType.String, product.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, product.UpdatedTime);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, product.UpdatedTime);           
            db.AddInParameter(dc, "p_register_number", DbType.String, product.RegisterNumber);
            db.AddInParameter(dc, "p_ValidDate", DbType.DateTime, product.ValidDate);
            db.ExecuteNonQuery(dc);
        }

        public static void Update(ProductEntity product)
        {
            var sql = @"UPDATE products
SET name = @p_name, full_name = @p_full_name, description = @p_description, brand = @p_brand, short_code = @p_short_code, 
mini_package_unit = @p_mini_package_unit, mini_package_spec = @p_mini_package_spec, mini_package_count = @p_mini_package_count, 
package_unit = @p_package_unit, category = @p_category, is_local = @p_is_local,
updated_id = @p_updated_id, updated_time = @p_updated_time,barcode_url=@p_barcode_url,register_number=@p_register_number,ValidDate=@p_ValidDate
WHERE id = @p_id";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_id", DbType.String, product.Id);
            db.AddInParameter(dc, "p_name", DbType.String, product.Name);
            db.AddInParameter(dc, "p_full_name", DbType.String, product.FullName);
            db.AddInParameter(dc, "p_description", DbType.String, product.Description);
            db.AddInParameter(dc, "p_brand", DbType.String, product.Brand);
            db.AddInParameter(dc, "p_short_code", DbType.String, product.ShortCode);
            db.AddInParameter(dc, "p_mini_package_unit", DbType.String, product.MiniPackageUnit);
            db.AddInParameter(dc, "p_mini_package_spec", DbType.String, product.MiniPackageSpec);
            db.AddInParameter(dc, "p_mini_package_count", DbType.Int32, product.MiniPackageCount);
            db.AddInParameter(dc, "p_package_unit", DbType.String, product.PackageUnit);
            db.AddInParameter(dc, "p_category", DbType.String, product.Category);
            db.AddInParameter(dc, "p_is_local", DbType.Boolean, product.IsLocal);
            db.AddInParameter(dc, "p_updated_id", DbType.String, product.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, product.UpdatedTime);

            db.AddInParameter(dc, "p_barcode_url", DbType.String, product.BarcodeUrl);
            db.AddInParameter(dc, "p_register_number", DbType.String, product.RegisterNumber);
            db.AddInParameter(dc, "p_ValidDate", DbType.DateTime, product.ValidDate);
            db.ExecuteNonQuery(dc);
        }

        public static ProductEntity Get(string id)
        {
            var sql = string.Format("SELECT {0} FROM products WHERE id=@p_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var entity = new ProductEntity();
                    entity.Init(reader);

                    return entity;
                }
                else
                {
                    return null;
                }
            }
        }

        public static IList<ProductEntity> Get(IList<string> ids)
        {
            var list = new List<ProductEntity>();

            if (ids == null  || ids.Count == 0)
            {
                return list;
            }

            var sql = string.Format("SELECT {0} FROM products WHERE id in ('{1}')", COLUMN_SQL, string.Join("','", ids));

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new ProductEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<ProductEntity> Query()
        {
            var sql = string.Format("SELECT {0} FROM products ORDER BY name", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            var list = new List<ProductEntity>();
            using (IDataReader reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new ProductEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<ProductEntity> Query(string condition, PagerInfo pager)
        {
            pager.ComputePageCount(QueryCount(condition));

            var list = new List<ProductEntity>();


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
                orderSql += "name";
            }

            var sql = string.Format(@"SELECT {0} FROM products WHERE 1=1{1}", COLUMN_SQL, GetConditionSql(condition));

            sql = @"SELECT * FROM
            (
                SELECT ROW_NUMBER() OVER(" + orderSql + @") pid," + COLUMN_SQL + @"
                FROM (" + sql + @") t            
            ) t1 WHERE t1.pid BETWEEN @p_pageNo * @p_pageSize + 1 AND (@p_pageNo + 1) * @p_pageSize ";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_condition", DbType.String, "%" + condition + "%");
            db.AddInParameter(dc, "p_pageNo", DbType.Int32, pager.PageIndex);
            db.AddInParameter(dc, "p_pageSize", DbType.Int32, pager.PageSize);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new ProductEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        private static int QueryCount(string condition)
        {
            var sql = string.Format(@"SELECT COUNT(id) FROM products WHERE 1=1 ", COLUMN_SQL);

            var conditionSql = GetConditionSql(condition);
            if (!string.IsNullOrEmpty(conditionSql))
            {
                sql += conditionSql;
            }

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_condition", DbType.String, condition);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                reader.Read();

                return reader.GetInt32(0);
            }
        }

        private static string GetConditionSql(string condition)
        {
            var conditionSql = " ";
            if (!string.IsNullOrEmpty(condition))
            {
                conditionSql = " AND (name LIKE @p_condition OR description LIKE @p_condition OR short_code LIKE @p_condition)";
            }

            return conditionSql;
        }

        public static IList<ProductEntity> GetByVendor(string vendorId)
        {
            var sql = string.Format(@"SELECT * FROM products WHERE id in
(
SELECT product_id FROM vendor_products WHERE vendor_id = @p_vendor_id
)", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, vendorId);

            var list = new List<ProductEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var product = new ProductEntity();
                    product.Init(reader);

                    list.Add(product);
                }
            }

            return list;
        }


        public static void UpdateBarcode(string id, string barcode, string url)
        {
            var sql = "update products set barcode=@p_barcode,barcode_url=@p_barcode_url where id=@p_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_barcode", DbType.String, barcode);
            db.AddInParameter(cmd, "p_barcode_url", DbType.String, url);
            db.AddInParameter(cmd, "p_id", DbType.String, id);

            db.ExecuteNonQuery(cmd);
        }

        public static ProductEntity GetByBarcode(string barcode)
        {
            var sql = string.Format("SELECT {0} FROM products WHERE barcode=@p_barcode", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_barcode", DbType.String, barcode);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var entity = new ProductEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }
    }
}
