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
    public static class ReceiptInfoRepository
    {
        private static string COLUMN_SQL = "id, hospital_id, title, tax, created_id, created_time, updated_id, updated_time";

        public static void Add(ReceiptInfoEntity receipt)
        {
            var sql = string.Format(@"INSERT INTO receipt_info({0})
VALUES(@p_id, @p_hospital_id, @p_title, @p_tax, 
@p_created_id, @p_created_time, @p_updated_id, @p_updated_time)", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, receipt.Id);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, receipt.HospitalId);
            db.AddInParameter(dc, "p_title", DbType.String, receipt.Title);
            db.AddInParameter(dc, "p_tax", DbType.String, receipt.Tax);
            db.AddInParameter(dc, "p_created_id", DbType.String, receipt.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, receipt.CreatedTime);
            db.AddInParameter(dc, "p_updated_id", DbType.String, receipt.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, receipt.UpdatedTime);

            db.ExecuteNonQuery(dc);
        }

        public static void Update(ReceiptInfoEntity receipt)
        {
            var sql = string.Format(@"UPDATE receipt_info
SET title = @p_title, tax = @p_tax, updated_id = @p_updated_id, updated_time = @p_updated_time
WHERE id=@p_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, receipt.Id);
            db.AddInParameter(dc, "p_title", DbType.String, receipt.Title);
            db.AddInParameter(dc, "p_tax", DbType.String, receipt.Tax);
            db.AddInParameter(dc, "p_updated_id", DbType.String, receipt.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, receipt.UpdatedTime);

            db.ExecuteNonQuery(dc);
        }

        public static IList<ReceiptInfoEntity> GetByHospital(string hospitalId)
        {
            var sql = string.Format(@"SELECT {0} FROM receipt_info WHERE hospital_id = @p_hospital_id ORDER BY TITLE", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            var list = new List<ReceiptInfoEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    var entity = new ReceiptInfoEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static ReceiptInfoEntity Get(string id)
        {
            var sql = string.Format(@"SELECT {0} FROM receipt_info WHERE id = @p_id ", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);

            var list = new List<ReceiptInfoEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new ReceiptInfoEntity();
                    entity.Init(reader);

                    return entity;
                }

                return null;
            }
        }
    }
}
