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
using LIMS.Util;

namespace LIMS.Repositories
{
    public static class UnitRepository
    {
        private static string COLUMN_SQL = @"id, name, description, contact_id, short_code, default_receipt_id, type, parent_id, root_id, business_type,
created_id, created_time, updated_id, updated_time";

        public static void Add(UnitEntity unit, Database db, DbTransaction trans)
        {
            var sql = string.Format(@"INSERT INTO units({0})
VALUES(@p_id, @p_name, @p_description, @p_contact_id, @p_short_code, @p_default_receipt_id, @p_type, @p_parent_id, @p_root_id, @p_business_type,
@p_created_id, @p_created_time, @p_updated_id, @p_updated_time)", COLUMN_SQL);

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, unit.Id);
            db.AddInParameter(dc, "p_name", DbType.String, unit.Name);
            db.AddInParameter(dc, "p_description", DbType.String, unit.Description);
            db.AddInParameter(dc, "p_contact_id", DbType.String, unit.ContactId);
            db.AddInParameter(dc, "p_short_code", DbType.String, unit.ShortCode);
            db.AddInParameter(dc, "p_default_receipt_id", DbType.String, unit.DefaultReceiptId);
            db.AddInParameter(dc, "p_type", DbType.Int32, unit.Type);
            db.AddInParameter(dc, "p_parent_id", DbType.String, unit.ParentId);
            db.AddInParameter(dc, "p_root_id", DbType.String, unit.RootId);
            db.AddInParameter(dc, "p_business_type", DbType.Int32, unit.BusinessType);
            db.AddInParameter(dc, "p_created_id", DbType.String, unit.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, unit.CreatedTime);
            db.AddInParameter(dc, "p_updated_id", DbType.String, unit.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, unit.UpdatedTime);

            db.ExecuteNonQuery(dc, trans);
        }

        public static void Update(UnitEntity unit, Database db, DbTransaction trans)
        {
            var sql = @"UPDATE units
SET name = @p_name, description = @p_description, contact_id = @p_contact_id, short_code = @p_short_code, 
default_receipt_id = @p_default_receipt_id, type = @p_type, business_type = @p_business_type,
updated_id = @p_updated_id, updated_time = @p_updated_time
WHERE id = @p_id";

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_name", DbType.String, unit.Name);
            db.AddInParameter(dc, "p_description", DbType.String, unit.Description);
            db.AddInParameter(dc, "p_contact_id", DbType.String, unit.ContactId);
            db.AddInParameter(dc, "p_short_code", DbType.String, unit.ShortCode);
            db.AddInParameter(dc, "p_default_receipt_id", DbType.String, unit.DefaultReceiptId);
            db.AddInParameter(dc, "p_type", DbType.Int32, unit.Type);
            db.AddInParameter(dc, "p_business_type", DbType.Int32, unit.BusinessType);
            db.AddInParameter(dc, "p_updated_id", DbType.String, unit.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, unit.UpdatedTime);

            db.AddInParameter(dc, "p_id", DbType.String, unit.Id);

            db.ExecuteNonQuery(dc, trans);
        }

        public static UnitEntity Get(string id)
        {
            var sql = string.Format("SELECT {0} FROM units WHERE id=@p_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var entity = new UnitEntity();
                    entity.Init(reader);

                    return entity;
                }
                else
                {
                    return null;
                }
            }
        }

        public static IList<UnitEntity> GetByRootId(string rootId)
        {
            var sql = string.Format("SELECT {0} FROM units WHERE root_id=@p_root_id ORDER BY name", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_root_id", DbType.String, rootId);

            var list = new List<UnitEntity>();
            using (IDataReader reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new UnitEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<UnitEntity> GetAllById(string id)
        {
            var unit = Get(id);
            if(unit != null)
            {
                IList<UnitEntity> list;
                if(unit.RootId == Constant.DEFAULT_UNIT_ROOT_ID)
                {
                    list = GetByRootId(unit.Id);
                    list.Insert(0, Get(unit.Id));
                }
                else
                {
                    list = GetByRootId(unit.RootId);
                    list.Insert(0, Get(unit.RootId));
                }

                return list;
            }
            else
            {
                return new List<UnitEntity>();
            }
        }

        public static IList<UnitEntity> Query(string name, string rootId, int count = 20)
        {
            var sql = string.Format(@"select top {1} {0} from units
where 1=1 and name like @p_name and root_id = @p_root_id
order by name", COLUMN_SQL, count);

            var db = DatabaseFactory.CreateDatabase();
            
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_name", DbType.String, "%" + name + "%");
            db.AddInParameter(dc, "p_root_id", DbType.String, rootId);

            var list = new List<UnitEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    var entity = new UnitEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<UnitEntity> Query(string rootId, UnitType unitType)
        {
            var sql = string.Format("SELECT {0} FROM units WHERE type=@p_type and root_id=@p_root_id ORDER BY name", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_type", DbType.Int32, (int)unitType);
            db.AddInParameter(dc, "p_root_id", DbType.String, rootId);

            var list = new List<UnitEntity>();
            using (IDataReader reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new UnitEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<UnitEntity> Query(string condition, string rootId, UnitType unitType, PagerInfo pager)
        {
            pager.ComputePageCount(QueryCount(condition, rootId, unitType));

            var list = new List<UnitEntity>();


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

            var sql = string.Format(@"SELECT {0} FROM units WHERE 1=1{1}", COLUMN_SQL, GetConditionSql(condition, unitType));

            sql = @"SELECT * FROM
            (
                SELECT ROW_NUMBER() OVER(" + orderSql + @") pid," + COLUMN_SQL + @"
                FROM (" + sql + @") t            
            ) t1 WHERE t1.pid BETWEEN @p_pageNo * @p_pageSize + 1 AND (@p_pageNo + 1) * @p_pageSize ";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_condition", DbType.String, "%" + condition + "%");
            if (unitType != UnitType.None)
            {
                db.AddInParameter(dc, "p_type", DbType.Int32, (int)unitType);
            }
            db.AddInParameter(dc, "p_pageNo", DbType.Int32, pager.PageIndex);
            db.AddInParameter(dc, "p_pageSize", DbType.Int32, pager.PageSize);
            db.AddInParameter(dc, "p_root_id", DbType.String, rootId);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    var entity = new UnitEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        private static int QueryCount(string condition, string rootId, UnitType unitType)
        {
            var sql = string.Format(@"SELECT COUNT(id) FROM units WHERE 1=1 ", COLUMN_SQL);

            var conditionSql = GetConditionSql(condition, unitType);
            if(!string.IsNullOrEmpty(conditionSql))
            {
                sql += conditionSql;
            }

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_condition", DbType.String, condition);
            if(unitType != UnitType.None)
            {
                db.AddInParameter(dc, "p_type", DbType.Int32, (int)unitType);
            }
            db.AddInParameter(dc, "p_root_id", DbType.String, rootId);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                reader.Read();

                return reader.GetInt32(0);
            }
        }

        private static string GetConditionSql(string condition, UnitType unitType)
        {
            var conditionSql = string.Format(" {0} AND root_id=@p_root_id ", unitType == UnitType.None ? "" : "AND type=@p_type");
            if (!string.IsNullOrEmpty(condition))
            {
                conditionSql += " AND (name LIKE @p_condition OR description LIKE @p_condition OR short_code LIKE @p_condition)";
            }

            return conditionSql;
        }


        public static IList<UnitEntity> GetHospitalsByUserId(string userId)
        {
            var db = DatabaseFactory.CreateDatabase();

            DbCommand dc;

            var sql = string.Empty;
            if(string.Compare(userId, Constant.ADMIN_ID) == 0)
            {
                sql = string.Format("SELECT {0} FROM units WHERE type=@p_type ORDER BY name", COLUMN_SQL);
                dc = db.GetSqlStringCommand(sql);
            }
            else
            {
                sql = string.Format(@"SELECT {0} FROM units 
WHERE 1=1 AND id IN (SELECT unit_root_id FROM user_privilege WHERE user_id = @p_user_id and operate=1) AND type=@p_type ORDER BY name", COLUMN_SQL);
                dc = db.GetSqlStringCommand(sql);
                db.AddInParameter(dc, "p_user_id", DbType.String, userId);
            }
            db.AddInParameter(dc, "p_type", DbType.Int32, (int)(UnitType.Hospital));

            var list = new List<UnitEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    var unit = new UnitEntity();
                    unit.Init(reader);

                    list.Add(unit);
                }
            }

            return list;
        }


        public static IList<UnitEntity> GetApplyUnits(string userId, string hospitalId)
        {
            var sql = string.Format(@"SELECT {0} FROM units 
WHERE id IN (SELECT unit_id FROM user_privilege WHERE user_id=@p_user_id AND unit_root_id=@p_root_id AND operate=1) and id in (select distinct unit_id from hospital_products where hospital_id=@p_root_id)
ORDER BY name", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_user_id", DbType.String, userId);
            db.AddInParameter(dc, "p_root_id", DbType.String, hospitalId);

            var list = new List<UnitEntity>();
            using(var reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    var unit = new UnitEntity();
                    unit.Init(reader);

                    list.Add(unit);
                }
            }

            return list;
        }

        public static IList<UnitEntity> GetByBusinessType(string rootId, UnitBusinessType businessType)
        {
            var sql = string.Format("select {0} from units where root_id=@p_root_id and business_type=@p_business_type order by name", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_root_id", DbType.String, rootId);
            db.AddInParameter(dc, "p_business_type", DbType.Int32, (int)businessType);

            var list = new List<UnitEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    var entity = new UnitEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<UnitEntity> GetByBusinessType(string rootId, string userId, UnitBusinessType businessType)
        {
            var sql = string.Format(@"select {0} from units 
where root_id=@p_root_id and business_type=@p_business_type 
and id in (select unit_id from user_privilege where user_id=@p_user_id and unit_root_id=@p_root_id and operate=1)
order by name", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_root_id", DbType.String, rootId);
            db.AddInParameter(dc, "p_user_id", DbType.String, userId);
            db.AddInParameter(dc, "p_business_type", DbType.Int32, (int)businessType);

            var list = new List<UnitEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new UnitEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<UnitEntity> GetVendorsByHospitalUnit(string unitId)
        {
            var sql = string.Format(@"SELECT {0} FROM units WHERE id IN
(
SELECT b.vendor_id FROM hospital_products a JOIN vendor_products b ON a.product_id=b.product_id
WHERE a.unit_id = @p_unit_id
)
ORDER BY name", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_unit_id", DbType.String, unitId);

            var list = new List<UnitEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var unit = new UnitEntity();
                    unit.Init(reader);

                    list.Add(unit);
                }
            }

            return list;
        }

        public static IList<UnitEntity> GetVendorsByHospital(string hospitalId)
        {
            var sql = string.Format(@"SELECT {0} FROM units WHERE id IN
(
SELECT vendor_id FROM vendor_hospitals where hospital_id=@p_hospital_id
)
ORDER BY name", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            var list = new List<UnitEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var unit = new UnitEntity();
                    unit.Init(reader);

                    list.Add(unit);
                }
            }

            return list;
        }

        public static IList<UnitEntity> GetHospitalsByVendor(string vendorId)
        {
            var sql = string.Format(@"SELECT {0} FROM units WHERE id IN
(
SELECT hospital_id FROM vendor_hospitals where vendor_id = @p_vendor_id
)
ORDER BY name", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, vendorId);

            var list = new List<UnitEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var unit = new UnitEntity();
                    unit.Init(reader);

                    list.Add(unit);
                }
            }

            return list;
        }


    }
}
