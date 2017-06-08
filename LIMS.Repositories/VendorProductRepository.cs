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
    public static class VendorProductRepository
    {
        private const string COLUMN_SQL = @"id, vendor_id, product_id, alias,
registration_id, expiration_date, unit_id, is_active,
created_id, created_time, updated_id, updated_time";

        public static IList<VendorProductEntity> Query(string vendorUnitId)
        {
            var list = new List<VendorProductEntity>();

            var sql = string.Format("SELECT {0} FROM vendor_products WHERE unit_id=@p_unit_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_unit_id", DbType.String, vendorUnitId);

            using (var reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    var entity = new VendorProductEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<VendorProductEntity> QueryByVendor(string vendorId)
        {
            var list = new List<VendorProductEntity>();

            var sql = string.Format(@"SELECT {0} FROM vendor_products 
WHERE unit_id in (select id from units where root_id = @p_vendor_id)", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, vendorId);

            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new VendorProductEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static VendorProductEntity Get(string unitId, string productId)
        {
            var sql = string.Format(@"select {0} from vendor_products where unit_id = @p_unit_id and product_id = @p_product_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_unit_id", DbType.String, unitId);
            db.AddInParameter(dc, "p_product_id", DbType.String, productId);

            using (var reader = db.ExecuteReader(dc))
            {
                if(reader.Read())
                {
                    var entity = new VendorProductEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static void Save(VendorProductEntity entity)
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                Add(entity);
            }
            else
            {
                Update(entity);
            }
        }

        private static void Add(VendorProductEntity entity)
        {
            var sql = string.Format(@"
INSERT INTO vendor_products({0})
VALUES(
@p_id, @p_vendor_id, @p_product_id, @p_alias,
@p_registration_id, @p_expiration_date, @p_unit_id, @p_is_active,
@p_created_id, @p_created_time, @p_updated_id, @p_updated_time)", COLUMN_SQL);

            entity.Id = Guid.NewGuid().ToString();

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, entity.VendorId);
            db.AddInParameter(dc, "p_product_id", DbType.String, entity.ProductId);
            db.AddInParameter(dc, "p_alias", DbType.String, entity.Alias);
            db.AddInParameter(dc, "p_registration_id", DbType.String, entity.RegistrationId);
            db.AddInParameter(dc, "p_expiration_date", DbType.DateTime, entity.ExpirationDate);
            db.AddInParameter(dc, "p_unit_id", DbType.String, entity.UnitId);
            db.AddInParameter(dc, "p_is_active", DbType.Boolean, entity.IsActive);
            db.AddInParameter(dc, "p_created_id", DbType.String, entity.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, entity.CreatedTime);
            db.AddInParameter(dc, "p_updated_id", DbType.String, entity.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, entity.UpdatedTime);

            db.ExecuteNonQuery(dc);
        }

        private static void Update(VendorProductEntity entity)
        {
            var sql = @"
update vendor_products
set alias = @p_alias, registration_id = @p_registration_id, expiration_date = @p_expiration_date,
    is_active = @p_is_active, updated_id = @p_updated_id, updated_time = @p_updated_time
where id = @p_id";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_alias", DbType.String, entity.Alias);
            db.AddInParameter(dc, "p_registration_id", DbType.String, entity.RegistrationId);
            db.AddInParameter(dc, "p_expiration_date", DbType.DateTime, entity.ExpirationDate);
            db.AddInParameter(dc, "p_is_active", DbType.Boolean, entity.IsActive);
            db.AddInParameter(dc, "p_updated_id", DbType.String, entity.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, entity.UpdatedTime);

            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);

            db.ExecuteNonQuery(dc);
        }



        //public static void Save(string unitId, IList<VendorProductEntity> entities)
        //{
        //    var db = DatabaseFactory.CreateDatabase();

        //    using (var conn = db.CreateConnection())
        //    {
        //        conn.Open();
        //        using (var trans = conn.BeginTransaction())
        //        {
        //            try
        //            {
        //                Delete(unitId, db, trans);
        //                BatchAdd(entities, db, trans);

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

        //private static void Delete(string unitId, Database db, DbTransaction trans)
        //{
        //    var sql = "DELETE vendor_products WHERE unit_id = @p_unit_id";
        //    var dc = db.GetSqlStringCommand(sql);
        //    db.AddInParameter(dc, "p_unit_id", DbType.String, unitId);

        //    db.ExecuteNonQuery(dc, trans);
        //}

        //private static void BatchAdd(IList<VendorProductEntity> entities, Database db, DbTransaction trans)
//        {
//            foreach(var item in entities)
//            {
//                var sql = string.Format(@"INSERT INTO vendor_products({0})
//VALUES(@p_id, @p_vendor_id, @p_unit_id, @p_product_id, @p_created_id, @p_created_time, @p_updated_id, @p_updated_time)", COLUMN_SQL);

//                var dc = db.GetSqlStringCommand(sql);
//                db.AddInParameter(dc, "p_id", DbType.String, item.Id);
//                db.AddInParameter(dc, "p_vendor_id", DbType.String, item.VendorId);
//                db.AddInParameter(dc, "p_unit_id", DbType.String, item.UnitId);
//                db.AddInParameter(dc, "p_product_id", DbType.String, item.ProductId);
//                db.AddInParameter(dc, "p_created_id", DbType.String, item.CreatedId);
//                db.AddInParameter(dc, "p_created_time", DbType.DateTime, item.CreatedTime);
//                db.AddInParameter(dc, "p_updated_id", DbType.String, item.UpdatedId);
//                db.AddInParameter(dc, "p_updated_time", DbType.DateTime, item.UpdatedTime);

//                db.ExecuteNonQuery(dc, trans);
//            }
//        }
    }
}
