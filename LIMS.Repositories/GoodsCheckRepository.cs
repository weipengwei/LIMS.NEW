using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;

using LIMS.Util;
using LIMS.Entities;

namespace LIMS.Repositories
{
    public static class GoodsCheckRepository
    {
        private const string COLUMNS = "id,name,belong_to,hospital_id,status,created_id,created_time,closer_id,closed_time";

        public static IList<GoodsCheckEntity> Get(string hospitalId)
        {
            var sql = string.Format("select {0} from goods_check where hospital_id = @p_hospital_id and status = @p_status order by created_time desc", COLUMNS);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(dc, "p_status", DbType.String, GoodsCheckStatus.ACTIVE);

            var list = new List<GoodsCheckEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new GoodsCheckEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<GoodsCheckEntity> GetByHospitals(IList<string> hospitalIds)
        {
            var sql = string.Format("select {0} from goods_check where hospital_id in ('{1}') and status = @p_status order by hospital_id, name", COLUMNS, string.Join("','", hospitalIds));

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            
            db.AddInParameter(dc, "p_status", DbType.String, GoodsCheckStatus.ACTIVE);

            var list = new List<GoodsCheckEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new GoodsCheckEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static bool Exist(string checkId, string hospitalId)
        {
            var sql = string.Format("select count(*) from goods_check where id = @p_id and hospital_id = @p_hospital_id and status = @p_status", COLUMNS);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_id", DbType.String, checkId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(dc, "p_status", DbType.String, GoodsCheckStatus.ACTIVE);

            var exist = false;
            using (var reader = db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    exist = reader.GetInt32(0) > 0;
                }
            }

            return exist;
        }

        public static void Save(GoodsCheckEntity entity)
        {
            entity.Id = Guid.NewGuid().ToString();

            var sql = string.Format(@"insert into goods_check({0})
values(
@p_id, @p_name, @p_belong_to, @p_hospital_id, @p_status, @p_created_id, @p_created_time, @p_closer_id, @p_closed_time
)", COLUMNS);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);
            db.AddInParameter(dc, "p_name", DbType.String, entity.Name);
            db.AddInParameter(dc, "p_belong_to", DbType.String, entity.BelongTo);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, entity.HospitalId);
            db.AddInParameter(dc, "p_status", DbType.String, entity.Status);
            db.AddInParameter(dc, "p_created_id", DbType.String, entity.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, entity.CreatedTime);
            db.AddInParameter(dc, "p_closer_id", DbType.String, DBNull.Value);
            db.AddInParameter(dc, "p_closed_time", DbType.DateTime, DBNull.Value);

            db.ExecuteNonQuery(dc);
        }

        public static void Close(string id, string userId)
        {
            var sql = "update goods_check set status=@p_active, closer_id=@p_closer_id, closed_time=@p_closed_time where id=@p_id";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_id", DbType.String, id);
            db.AddInParameter(dc, "p_active", DbType.String, GoodsCheckStatus.INACTIVE);
            db.AddInParameter(dc, "p_closer_id", DbType.String, userId);
            db.AddInParameter(dc, "p_closed_time", DbType.DateTime, DateTime.Now);

            db.ExecuteNonQuery(dc);
        }
    }
}
