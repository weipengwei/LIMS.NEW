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
    public static class GoodsExtraRepsitory
    {
        private const string COLUMN_SQL = @"id, need_check, need_split, split_capacity, mini_split_number, valid_days, expired_date, split_copies, split_unit";

        public static string Create(GoodsExtraEntity goodsExtra, Database db, DbTransaction trans)
        {
            var sql = string.Format(@"
insert into goods_extra(
    {0}
)
values(
    @p_id, @p_need_check, @p_need_split, @p_split_capacity, @p_mini_split_number, @p_valid_days, @p_expired_date, @p_split_copies, @p_split_unit
)", COLUMN_SQL);
            
            var dc = db.GetSqlStringCommand(sql);

            var id = Guid.NewGuid().ToString();

            db.AddInParameter(dc, "p_id", DbType.String, id);
            db.AddInParameter(dc, "p_need_check", DbType.Boolean, goodsExtra.NeedCheck);
            db.AddInParameter(dc, "p_need_split", DbType.Boolean, goodsExtra.NeedSplit);
            db.AddInParameter(dc, "p_split_capacity", DbType.String, goodsExtra.SplitCapacity);
            db.AddInParameter(dc, "p_mini_split_number", DbType.Int32, goodsExtra.MiniSplitNumber);
            db.AddInParameter(dc, "p_valid_days", DbType.Decimal, goodsExtra.ValidDays);
            db.AddInParameter(dc, "p_expired_date", DbType.DateTime, goodsExtra.ExpiredDate);
            db.AddInParameter(dc, "p_split_copies", DbType.Int32, goodsExtra.SplitCopies);
            db.AddInParameter(dc, "p_split_unit", DbType.String, goodsExtra.SplitUnit);

            db.ExecuteNonQuery(dc, trans);

            return id;
        }

        public static GoodsExtraEntity Get(string id)
        {
            var sql = string.Format("select {0} from goods_extra where id=@p_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_id", DbType.String, id);

            using (var reader = db.ExecuteReader(dc))
            {
                if(reader.Read())
                {
                    var entity = new GoodsExtraEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        //public static void UpdateSplitCount(string id, int splitCount, Database db, DbTransaction trans)
        //{
        //    var sql = "update goods_extra set split_count = isnull(split_count, 0) + @p_splict_count where id = @p_id";

        //    var dc = db.GetSqlStringCommand(sql);
        //    db.AddInParameter(dc, "p_splict_count", DbType.Int32, splitCount);
        //    db.AddInParameter(dc, "p_id", DbType.String, id);

        //    db.ExecuteNonQuery(dc, trans);
        //}

        //public static GoodsExtraEntity Get(string id)
        //{
        //    var sql = string.Format(@"select {0} from goods_extra where id = @p_id", COLUMN_SQL);

        //    var db = DatabaseFactory.CreateDatabase();
        //    var dc = db.GetSqlStringCommand(sql);

        //    db.AddInParameter(dc, "p_id", DbType.String, id);
        //    using (var reader = db.ExecuteReader(dc))
        //    {
        //        while(reader.Read())
        //        {
        //            var entity = new GoodsExtraEntity();
        //            entity.Init(reader);

        //            return entity;
        //        }

        //        return null;
        //    }
        //}

        //public static IList<GoodsExtraEntity> GetProductsByUnit(string unitId)
        //{
        //    var sql = string.Format(@"select {0} from goods_extra where apply_unit_id = @p_unit_id", COLUMN_SQL);

        //    var db = DatabaseFactory.CreateDatabase();
        //    var dc = db.GetSqlStringCommand(sql);

        //    db.AddInParameter(dc, "p_unit_id", DbType.String, unitId);

        //    var list = new List<GoodsExtraEntity>();
        //    using (var reader = db.ExecuteReader(dc))
        //    {
        //        while(reader.Read())
        //        {
        //            var entity = new GoodsExtraEntity();
        //            entity.Init(reader);

        //            list.Add(entity);
        //        }
        //    }

        //    return list;
        //}

        //public static void Reduce(string productId, string hospitalId, int count, Database db, DbTransaction trans)
        //{
        //    var goodsProducts = Get(productId, hospitalId, db, trans);

        //    var remainingCount = count;
        //    var temp = 0;
        //    foreach(var id in goodsProducts.Keys)
        //    {
        //        var entity = goodsProducts[id];

        //        if (entity.NeedSplit)
        //        {
        //            if(entity.SplitCount > 0)
        //            {
        //                temp = entity.SplitCount >= remainingCount ? remainingCount : entity.SplitCount;
        //                remainingCount = remainingCount - temp;

        //                Reduce(entity.Id, true, temp, db, trans);
        //            }
        //        }
        //        else
        //        {
        //            if (entity.Count > 0)
        //            {
        //                temp = entity.Count >= remainingCount ? remainingCount : entity.Count;
        //                remainingCount = count - temp;

        //                Reduce(entity.Id, true, temp, db, trans);
        //            }
        //        }

        //        if(remainingCount == 0)
        //        {
        //            break;
        //        }
        //    }
        //}

        //private static void Reduce(string id, bool needSplit, int count, Database db, DbTransaction trans)
        //{
        //    var sql = string.Format("update goods_product set {0}= {0} - @p_count where id = @p_id", needSplit ? "split_count" : "count");

        //    var dc = db.GetSqlStringCommand(sql);
        //    db.AddInParameter(dc, "p_count", DbType.Int32, count);
        //    db.AddInParameter(dc, "p_id", DbType.String, id);

        //    db.ExecuteNonQuery(dc, trans);
        //}

        //private static IDictionary<string, GoodsExtraEntity> Get(string productId, string hospitalId, Database db, DbTransaction trans)
        //{
        //    var sql = string.Format(@"select {0} from goods_product where product_id = @p_product_id and hospital_id = @p_hospital_id", COLUMN_SQL);

        //    var dc = db.GetSqlStringCommand(sql);
        //    db.AddInParameter(dc, "p_product_id", DbType.String, productId);
        //    db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

        //    var dic = new Dictionary<string, GoodsExtraEntity>();
        //    using (var reader = db.ExecuteReader(dc, trans))
        //    {
        //        while(reader.Read())
        //        {
        //            var entity = new GoodsExtraEntity();
        //            entity.Init(reader);

        //            dic[entity.Id] = entity;
        //        }
        //    }

        //    return dic;
        //}
    }
}
