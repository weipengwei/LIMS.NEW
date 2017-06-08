using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using LIMS.Entities;
using LIMS.Models;
using LIMS.Util;


namespace LIMS.Repositories
{
    public static class DonateProductRepository
    {
        private const string COLUMN = "id,hospital_id,unit_id,vendor_id,product_id,base_count,created_id,created_time,updated_id,updated_time";
        private const string ITEM_COLUMN = "id,parent_id,product_id,count,created_id,created_time,updated_id,updated_time";
        
        public static void Get(string hospitalId, string unitId, string vendorId, string productId, 
            out DonateProductEntity donation, out IList<DonateProductItemEntity> items)
        {
            donation = Get(hospitalId, unitId, vendorId, productId);

            if(donation == null)
            {
                items = new List<DonateProductItemEntity>();
                return;
            }

            items = GetItems(donation.Id);
        }

        private static DonateProductEntity Get(string hospitalId, string unitId, string vendorId, string productId)
        {
            var sql = string.Format(@"select {0} from donate_product
where hospital_id=@p_hospital_id and unit_id=@p_unit_id and vendor_id=@p_vendor_id and product_id=@p_product_id", COLUMN);

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(cmd, "p_unit_id", DbType.String, unitId);
            db.AddInParameter(cmd, "p_vendor_id", DbType.String, vendorId);
            db.AddInParameter(cmd, "p_product_id", DbType.String, productId);

            using (var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    var entity = new DonateProductEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        private static IList<DonateProductItemEntity> GetItems(string parentId)
        {
            var sql = string.Format(@"select {0} from donate_product_items where parent_id=@p_parent_id", ITEM_COLUMN);

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_parent_id", DbType.String, parentId);

            var list = new List<DonateProductItemEntity>();
            using (var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new DonateProductItemEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }

                return list;
            }
        }
    }
}
