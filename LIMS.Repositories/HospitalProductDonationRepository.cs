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
    public static class HospitalProductDonationRepository
    {
        private const string COLUMNS = "id,hospital_id,unit_id,vendor_id,product_id,total_count,used_count";

        public static IList<HospitalProductDonationEntity> GetDonationByProducts(string hospitalId, string unitId, string vendorId, IList<string> products)
        {
            var list = new List<HospitalProductDonationEntity>();
            if (products == null || products.Count == 0)
            {
                return list;
            }

            IList<string> names;
            string namesSql;
            StringHelper.GenerInParameters("p_product", products.Count, out names, out namesSql);

            var sql = string.Format(@"select {0} from hospital_product_donations 
where hospital_id=@p_hospital_id and unit_id=@p_unit_id and vendor_id=@p_vendor_id and product_id in ({1})", COLUMNS, namesSql);

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(cmd, "p_unit_id", DbType.String, unitId);
            db.AddInParameter(cmd, "p_vendor_id", DbType.String, vendorId);
            for (var i = 0; i < products.Count; i++)
            {
                db.AddInParameter(cmd, names[i], DbType.String, products[i]);
            }

            using (var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new HospitalProductDonationEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        internal static void UpdateDonations(IList<HospitalProductDonationEntity> changingList, Database db, DbTransaction trans)
        {
            if (changingList == null || changingList.Count == 0)
            {
                return;
            }

            var hospitalId = changingList[0].HospitalId;
            var unitId = changingList[0].UnitId;
            var vendorId = changingList[0].VendorId;
            var products = changingList.ToDictionary(item => item.ProductId);

            var olds = GetDonationByProducts(hospitalId, unitId, vendorId, products.Keys.ToList()).Where(item=>products.ContainsKey(item.ProductId)).ToList();

            var computedList = new Dictionary<string, string>();
            HospitalProductDonationEntity entity;
            foreach (var old in olds)
            {
                if (products.TryGetValue(old.ProductId, out entity))
                {
                    AppendDonation(old.Id, entity.TotalCount, entity.UsedCount, db, trans);
                    computedList[old.ProductId] = old.ProductId;
                }
            }

            foreach(var item in changingList)
            {
                if (!computedList.ContainsKey(item.ProductId))
                {
                    CreateDonation(item, db, trans);
                }
            }
        }

        private static void AppendDonation(string id, decimal donateCount, decimal usedCount, Database db, DbTransaction trans)
        {
            var sql = "update hospital_product_donations set total_count+=@p_total_count, used_count+=@p_used_count where id=@p_id";

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, id);
            db.AddInParameter(cmd, "p_total_count", DbType.Decimal, donateCount);
            db.AddInParameter(cmd, "p_used_count", DbType.Decimal, usedCount);

            db.ExecuteNonQuery(cmd, trans);
        }

        private static void CreateDonation(HospitalProductDonationEntity donation, Database db, DbTransaction trans)
        {
            var sql = string.Format(@"insert into hospital_product_donations({0}) values
(@p_id,@p_hospital_id,@p_unit_id,@p_vendor_id,@p_product_id,@p_total_count,@p_used_count)", COLUMNS);

            donation.Id = Guid.NewGuid().ToString();

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, donation.Id);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, donation.HospitalId);
            db.AddInParameter(cmd, "p_unit_id", DbType.String, donation.UnitId);
            db.AddInParameter(cmd, "p_vendor_id", DbType.String, donation.VendorId);
            db.AddInParameter(cmd, "p_product_id", DbType.String, donation.ProductId);
            db.AddInParameter(cmd, "p_total_count", DbType.Decimal, donation.TotalCount);
            db.AddInParameter(cmd, "p_used_count", DbType.Decimal, donation.UsedCount);

            db.ExecuteNonQuery(cmd, trans);
        }

    }
}
