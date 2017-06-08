using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;

using LIMS.Entities;

namespace LIMS.Repositories
{
    public static class GoodsCheckDataRepository
    {
        private const string COLUMNS = "id,barcode,check_id,check_user,check_datetime,status";

        public static void Scan(GoodsCheckDataEntity data)
        {
            var sql = @"insert into goods_check_data(barcode,check_id,check_user,check_datetime,status)
values(@p_barcode,@p_check_id,@p_check_user,@p_check_datetime,@p_status)";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_barcode", DbType.String, data.Barcode);
            db.AddInParameter(dc, "p_check_id", DbType.String, data.CheckId);
            db.AddInParameter(dc, "p_check_user", DbType.String, data.CheckUser);
            db.AddInParameter(dc, "p_check_datetime", DbType.DateTime, data.CheckDateTime);
            db.AddInParameter(dc, "p_status", DbType.String, data.Status);

            db.ExecuteNonQuery(dc);
        }
    }
}
