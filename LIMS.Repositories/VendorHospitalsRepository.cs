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
    public static class VendorHospitalsRepository
    {
        private const string COLUMN_SQL = "id, vendor_id, hospital_id, created_id, created_time, updated_id, updated_time";

        public static IList<VendorHospitalEntity> GetByVendor(string vendorId)
        {
            var sql = string.Format("select {0} from vendor_hospitals where vendor_id=@p_vendor_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, vendorId);

            var list = new List<VendorHospitalEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    var entity = new VendorHospitalEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static void Save(string unitId, IList<VendorHospitalEntity> entities)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        Delete(unitId, db, trans);
                        BatchAdd(entities, db, trans);

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

        private static void Delete(string vendorId, Database db, DbTransaction trans)
        {
            var sql = "DELETE vendor_hospitals WHERE vendor_id = @p_vendor_id";
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, vendorId);

            db.ExecuteNonQuery(dc, trans);
        }

        private static void BatchAdd(IList<VendorHospitalEntity> entities, Database db, DbTransaction trans)
        {
            foreach (var item in entities)
            {
                var sql = string.Format(@"INSERT INTO vendor_hospitals({0})
VALUES(@p_id, @p_vendor_id, @p_hospital_id, @p_created_id, @p_created_time, @p_updated_id, @p_updated_time)", COLUMN_SQL);

                var dc = db.GetSqlStringCommand(sql);
                db.AddInParameter(dc, "p_id", DbType.String, item.Id);
                db.AddInParameter(dc, "p_vendor_id", DbType.String, item.VendorId);
                db.AddInParameter(dc, "p_hospital_id", DbType.String, item.HospitalId);
                db.AddInParameter(dc, "p_created_id", DbType.String, item.CreatedId);
                db.AddInParameter(dc, "p_created_time", DbType.DateTime, item.CreatedTime);
                db.AddInParameter(dc, "p_updated_id", DbType.String, item.UpdatedId);
                db.AddInParameter(dc, "p_updated_time", DbType.DateTime, item.UpdatedTime);

                db.ExecuteNonQuery(dc, trans);
            }
        }
    }
}
