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
    public static class AuditingProductRepository
    {
        private const string COLUMN_SQL = @"id,hospital_id,vendor_id,product_id,is_audit,
version,created_id,created_time,updated_id,updated_time";

        public static IList<AuditingProductEntity> Query(string hospitalId, string vendorId)
        {
            var sql = string.Format(@"select {0} from auditing_products 
where hospital_id=@p_hospital_id and vendor_id=@p_vendor_id order by product_id, version desc", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, vendorId);

            var list = new List<AuditingProductEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    var entity = new AuditingProductEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static void Save(AuditingProductEntity entity)
        {
            var sql = string.Format(@"insert into auditing_products({0})
values(@p_id,@p_hospital_id,@p_vendor_id,@p_product_id,@p_is_audit,
@p_version,@p_created_id,@p_created_time,@p_updated_id,@p_updated_time)", COLUMN_SQL);

            entity.Id = Guid.NewGuid().ToString();

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, entity.HospitalId);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, entity.VendorId);
            db.AddInParameter(dc, "p_product_id", DbType.String, entity.ProductId);
            db.AddInParameter(dc, "p_is_audit", DbType.Boolean, entity.IsAudit);
            db.AddInParameter(dc, "p_version", DbType.Int32, entity.Version);
            db.AddInParameter(dc, "p_created_id", DbType.String, entity.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, entity.CreatedTime);
            db.AddInParameter(dc, "p_updated_id", DbType.String, entity.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, entity.UpdatedTime);

            db.ExecuteNonQuery(dc);
        }

        public static IList<AuditingProductEntity> Query(string hospitalId, string vendorId, DateTime applyTime)
        {
            var sql = string.Format(@"select {0} from auditing_products 
where hospital_id=@p_hospital_id and vendor_id=@p_vendor_id 
and created_time <= @p_apply_time and is_audit=1
order by product_id, version desc", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, vendorId);
            db.AddInParameter(dc, "p_apply_time", DbType.DateTime, applyTime);

            var list = new List<AuditingProductEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new AuditingProductEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }
    }
}
