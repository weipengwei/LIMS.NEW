using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using LIMS.Entities;

namespace LIMS.Repositories
{
    public static class HospitalProductRepository
    {
        private const string COLUMN_SQL = @"
id, hospital_id, unit_id, product_id, alias, category, need_check, need_split, split_capacity,
mini_split_number, donate_count, donate_base, valid_days, arrival_days, price, package_price, contact_id, is_active, 
created_id, created_time, updated_id, updated_time, split_copies, split_unit,grant_unit_count,order_unit_count";

        public static HospitalProductEntity Get(string unitId, string productId)
        {
            var sql = string.Format(@"SELECT {0} FROM hospital_products 
WHERE unit_id = @p_unit_id and product_id = @p_product_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_unit_id", DbType.String, unitId);
            db.AddInParameter(dc, "p_product_id", DbType.String, productId);

            HospitalProductEntity entity = null;
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    entity = new HospitalProductEntity();
                    entity.Init(reader);

                    break;
                }
            }

            return entity;
        }

        internal static HospitalProductEntity GetOneProduct(string productId, string hospitalId)
        {
            var sql = string.Format(@"SELECT top 1 {0} FROM hospital_products 
WHERE hospital_id = @p_hospital_id and product_id = @p_product_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(dc, "p_product_id", DbType.String, productId);

            HospitalProductEntity entity = null;
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    entity = new HospitalProductEntity();
                    entity.Init(reader);

                    break;
                }
            }

            return entity;
        }

        public static IList<HospitalProductEntity> GetByUnit(string unitId)
        {
            var sql = string.Format(@"SELECT {0} FROM hospital_products WHERE unit_id = @p_unit_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_unit_id", DbType.String, unitId);

            var list = new List<HospitalProductEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var product = new HospitalProductEntity();
                    product.Init(reader);

                    list.Add(product);
                }
            }

            return list;
        }
        
        public static IList<HospitalProductEntity> GetByHospital(string hospitalId)
        {
            var sql = string.Format(@"SELECT {0} FROM hospital_products WHERE hospital_id = @p_hospital_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            var list = new List<HospitalProductEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var product = new HospitalProductEntity();
                    product.Init(reader);

                    list.Add(product);
                }
            }

            return list;
        }

        public static IList<string> GetCategories(string hospitalId)
        {
            var sql = "select distinct category from hospital_products where hospital_id=@p_hospital_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);

            var list = new List<string>();
            using(var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var category = reader["category"].ToString();
                    if (!string.IsNullOrEmpty(category))
                    {
                        list.Add(category);
                    }
                }
            }

            return list;
        }

        #region Save
        public static void Save(HospitalProductEntity entity)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(entity.Id))
                        {
                            Add(entity, db, trans);
                        }
                        else
                        {
                            Update(entity, db, trans);
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
        }

        private static void Add(HospitalProductEntity entity, Database db, DbTransaction trans)
        {
            var sql = string.Format(@"insert into hospital_products({0})
values(@p_id, @p_hospital_id, @p_unit_id, @p_product_id, @p_alias, @p_category, @p_need_check, @p_need_split, @p_split_capacity,
@p_mini_split_number, @p_donate_count, @p_donate_base, @p_valid_days, @p_arrival_days, @p_price, @p_package_price, @p_contact_id, @p_is_active,
@p_created_id, @p_created_time, @p_updated_id, @p_updated_time, @p_split_copies, @p_split_unit)", COLUMN_SQL);
            
            var dc = db.GetSqlStringCommand(sql);

            entity.Id = Guid.NewGuid().ToString();

            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, entity.HospitalId);
            db.AddInParameter(dc, "p_unit_id", DbType.String, entity.UnitId);
            db.AddInParameter(dc, "p_product_id", DbType.String, entity.ProductId);
            db.AddInParameter(dc, "p_alias", DbType.String, entity.Alias);
            db.AddInParameter(dc, "p_category", DbType.String, entity.Category);
            db.AddInParameter(dc, "p_need_check", DbType.Boolean, entity.NeedCheck);
            db.AddInParameter(dc, "p_need_split", DbType.Boolean, entity.NeedSplit);
            db.AddInParameter(dc, "p_split_capacity", DbType.String, entity.SplitCapacity);
            db.AddInParameter(dc, "p_mini_split_number", DbType.Int32, entity.MiniSplitNumber);
            db.AddInParameter(dc, "p_donate_count", DbType.Int32, entity.DonateCount);
            db.AddInParameter(dc, "p_donate_base", DbType.Int32, entity.DonateBase);
            db.AddInParameter(dc, "p_valid_days", DbType.Decimal, entity.ValidDays);
            db.AddInParameter(dc, "p_arrival_days", DbType.Int32, entity.ArrivalDays);
            db.AddInParameter(dc, "p_price", DbType.Decimal, entity.Price);
            db.AddInParameter(dc, "p_package_price", DbType.Decimal, entity.PackagePrice);
            db.AddInParameter(dc, "p_contact_id", DbType.String, entity.ContactId);
            db.AddInParameter(dc, "p_is_active", DbType.Boolean, entity.IsActive);
            db.AddInParameter(dc, "p_created_id", DbType.String, entity.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, entity.CreatedTime);
            db.AddInParameter(dc, "p_updated_id", DbType.String, entity.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, entity.UpdatedTime);
            db.AddInParameter(dc, "p_split_copies", DbType.Int32, entity.SplitCopies);
            db.AddInParameter(dc, "p_split_unit", DbType.String, entity.SplitUnit);

            db.ExecuteNonQuery(dc, trans);
        }

        private static void Update(HospitalProductEntity entity, Database db, DbTransaction trans)
        {
            var sql = @"update hospital_products set
alias = @p_alias, category=@p_category, need_check = @p_need_check, need_split = @p_need_split, split_capacity = @p_split_capacity,
mini_split_number = @p_mini_split_number, donate_count = @p_donate_count, donate_base = @p_donate_base, valid_days = @p_valid_days, 
arrival_days = @p_arrival_days, price = @p_price, package_price = @p_package_price, contact_id = @p_contact_id, is_active = @p_is_active,
updated_id = @p_updated_id, updated_time = @p_updated_time, split_copies = @p_split_copies, split_unit = @p_split_unit where id = @p_id";
            
            var dc = db.GetSqlStringCommand(sql);
            
            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);
            db.AddInParameter(dc, "p_alias", DbType.String, entity.Alias);
            db.AddInParameter(dc, "p_category", DbType.String, entity.Category);
            db.AddInParameter(dc, "p_need_check", DbType.Boolean, entity.NeedCheck);
            db.AddInParameter(dc, "p_need_split", DbType.Boolean, entity.NeedSplit);
            db.AddInParameter(dc, "p_split_capacity", DbType.String, entity.SplitCapacity);
            db.AddInParameter(dc, "p_mini_split_number", DbType.Int32, entity.MiniSplitNumber);
            db.AddInParameter(dc, "p_donate_count", DbType.Int32, entity.DonateCount);
            db.AddInParameter(dc, "p_donate_base", DbType.Int32, entity.DonateBase);
            db.AddInParameter(dc, "p_valid_days", DbType.Decimal, entity.ValidDays);
            db.AddInParameter(dc, "p_arrival_days", DbType.Int32, entity.ArrivalDays);
            db.AddInParameter(dc, "p_price", DbType.Decimal, entity.Price);
            db.AddInParameter(dc, "p_package_price", DbType.Decimal, entity.PackagePrice);
            db.AddInParameter(dc, "p_contact_id", DbType.String, entity.ContactId);
            db.AddInParameter(dc, "p_is_active", DbType.Boolean, entity.IsActive);
            db.AddInParameter(dc, "p_updated_id", DbType.String, entity.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, entity.UpdatedTime);
            db.AddInParameter(dc, "p_split_copies", DbType.Int32, entity.SplitCopies);
            db.AddInParameter(dc, "p_split_unit", DbType.String, entity.SplitUnit);

            db.ExecuteNonQuery(dc, trans);
        }
        #endregion
    }
}
