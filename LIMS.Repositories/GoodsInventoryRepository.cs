using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using LIMS.Entities;
using LIMS.Util;
using LIMS.Models;

namespace LIMS.Repositories
{
    public static class GoodsInventoryRepository
    {
        private const string COLUMN_SQL = @"
id,serial_id,batch_no,product_id,storeroom_id,expired_date,hospital_id,vendor_id,
original_count,split_count,usable_count,apply_count,granted_count,created_id,created_time";

        internal static void Create(GoodsInventoryEntity entity, Database db, DbTransaction trans)
        {
            var sql = string.Format(@"insert into goods_inventory({0})
values(
    @p_id,@p_serial_id,@p_batch_no,@p_product_id,@p_storeroom_id,@p_expired_date,@p_hospital_id,@p_vendor_id,
    @p_original_count,@p_split_count,@p_usable_count,@p_apply_count,@p_granted_count,@p_created_id,@p_created_time
)", COLUMN_SQL);

            var cmd = db.GetSqlStringCommand(sql);
            entity.Id = Guid.NewGuid().ToString();

            db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);
            db.AddInParameter(cmd, "p_serial_id", DbType.String, entity.SerialId);
            db.AddInParameter(cmd, "p_batch_no", DbType.String, entity.BatchNo);
            db.AddInParameter(cmd, "p_product_id", DbType.String, entity.ProductId);
            db.AddInParameter(cmd, "p_storeroom_id", DbType.String, entity.StoreroomId);
            db.AddInParameter(cmd, "p_expired_date", DbType.DateTime, entity.ExpiredDate);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, entity.HospitalId);
            db.AddInParameter(cmd, "p_vendor_id", DbType.String, entity.VendorId);
            db.AddInParameter(cmd, "p_original_count", DbType.Int32, entity.OriginalCount);
            db.AddInParameter(cmd, "p_split_count", DbType.Int32, entity.SplitCount);
            db.AddInParameter(cmd, "p_usable_count", DbType.Int32, entity.UsableCount);
            db.AddInParameter(cmd, "p_apply_count", DbType.Int32, entity.ApplyCount);
            db.AddInParameter(cmd, "p_granted_count", DbType.Int32, entity.GrantedCount);
            db.AddInParameter(cmd, "p_created_id", DbType.String, entity.CreatedId);
            db.AddInParameter(cmd, "p_created_time", DbType.DateTime, entity.CreatedTime);

            db.ExecuteNonQuery(cmd, trans);
        }

        public static int CountLeft(string productId, string storeroomId, string hospitalId)
        {
            var sql = "select sum(apply_count-granted_count) from goods_inventory where hospital_id=@p_hospital_id and storeroom_id=@p_storeroom_id and product_id=@p_product_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_product_id", DbType.String, productId);
            db.AddInParameter(cmd, "p_storeroom_id", DbType.String, storeroomId);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);

            var totalCount = 0;
            using(var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    totalCount = reader[0] == DBNull.Value ? 0 : reader.GetInt32(0);
                }
            }

            sql = "select sum(apply_count) from goods_inventory_runtime where hospital_id=@p_hospital_id and product_id=@p_product_id";

            cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_product_id", DbType.String, productId);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);

            var runtimeCount = 0;
            using (var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    runtimeCount = reader[0] == DBNull.Value ? 0 : reader.GetInt32(0);
                }
            }

            var left = totalCount - runtimeCount;
            return left < 0 ? 0 : left;
        }

        public static int SumLeft(string applyId, string productId, string hospitalId)
        {
            var sql = "select sum(apply_count-granted_count) from goods_inventory_runtime where hospital_id=@p_hospital_id and product_id=@p_product_id and apply_id=@p_apply_id";
            
            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_product_id", DbType.String, productId);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, applyId);

            var grantedCount = 0;
            using (var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    grantedCount = reader[0] == DBNull.Value ? 0 : reader.GetInt32(0);
                }
            }

            return grantedCount;
        }

        public static GoodsInventoryEntity QueryEarlyExpiredDateInventory(string productId, DateTime expiredDate, string hospitalId)
        {
            var sql = string.Format(@"select top 1 {0} from goods_inventory 
where hospital_id=@p_hospital_id and product_id=@p_product_id 
    and expired_date<@p_expired_date and expired_date>@p_today 
order by expired_date
", COLUMN_SQL);
            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(cmd, "p_product_id", DbType.String, productId);
            db.AddInParameter(cmd, "p_expired_date", DbType.DateTime, expiredDate);
            db.AddInParameter(cmd, "p_today", DbType.DateTime, DateTime.Today);

            using(var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    var entity = new GoodsInventoryEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static GoodsInventoryEntity QueryEarlyExpiredDateInventory(string productId, string storeroomId, DateTime expiredDate, string hospitalId)
        {
            var sql = string.Format(@"select top 1 {0} from goods_inventory 
where hospital_id=@p_hospital_id and product_id=@p_product_id and storeroom_id=@p_storeroom_id
    and expired_date<@p_expired_date and expired_date>@p_today 
order by expired_date
", COLUMN_SQL);
            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(cmd, "p_product_id", DbType.String, productId);
            db.AddInParameter(cmd, "p_storeroom_id", DbType.String, storeroomId);
            db.AddInParameter(cmd, "p_expired_date", DbType.DateTime, expiredDate);
            db.AddInParameter(cmd, "p_today", DbType.DateTime, DateTime.Today);

            using (var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    var entity = new GoodsInventoryEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        internal static void CreateRuntime(GoodsInventoryRuntimeEntity entity, Database db, DbTransaction trans)
        {
            var sql = @"insert into 
goods_inventory_runtime(id,hospital_id,apply_id,product_id,apply_count,granted_count)
values(@p_id,@p_hospital_id,@p_apply_id,@p_product_id,@p_apply_count,@p_granted_count)";

            entity.Id = Guid.NewGuid().ToString();

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, entity.HospitalId);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, entity.ApplyId);
            db.AddInParameter(cmd, "p_product_id", DbType.String, entity.ProductId);
            db.AddInParameter(cmd, "p_apply_count", DbType.Int32, entity.ApplyCount);
            db.AddInParameter(cmd, "p_granted_count", DbType.Int32, entity.GrantedCount);

            db.ExecuteNonQuery(cmd, trans);
        }

        internal static void DeleteRuntime(string applyId, string hospitalId, Database db, DbTransaction trans)
        {
            var sql = "delete goods_inventory_runtime where apply_id=@p_apply_id";
            
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, applyId);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);

            db.ExecuteNonQuery(cmd, trans);
        }

        public static IList<GoodsInventoryRuntimeEntity> GetRuntime(string applyId, string hospitalId)
        {
            var sql = "select id,hospital_id,apply_id,product_id,apply_count,granted_count from goods_inventory_runtime where apply_id=@p_apply_id and hospital_id=@p_hospital_id";

            var list = new List<GoodsInventoryRuntimeEntity>();
            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_apply_id", DbType.String, applyId);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);
            using (var reader = db.ExecuteReader(cmd))
            {
                while(reader.Read())
                {
                    var entity = new GoodsInventoryRuntimeEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        internal static bool UpdateGrantRuntime(string applyId, string hospitalId, string productId, int count, Database db, DbTransaction trans)
        {
            var sql = @"update goods_inventory_runtime set granted_count=granted_count+@p_count 
where apply_id=@p_apply_id and product_id=@p_product_id and hospital_id=@p_hospital_id";

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, applyId);
            db.AddInParameter(cmd, "p_product_id", DbType.String, productId);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(cmd, "p_count", DbType.Int32, count);

            return db.ExecuteNonQuery(cmd, trans) > 0;
        }

        internal static bool AppendRuntime(string applyId, string hospitalId, string productId, int applyCount, int grantedCount, Database db, DbTransaction trans)
        {
            var sql = @"update goods_inventory_runtime set apply_count=apply_count+@p_apply_count,granted_count=granted_count+@p_granted_count 
where apply_id=@p_apply_id and product_id=@p_product_id and hospital_id=@p_hospital_id";

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, applyId);
            db.AddInParameter(cmd, "p_product_id", DbType.String, productId);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(cmd, "p_apply_count", DbType.Int32, applyCount);
            db.AddInParameter(cmd, "p_granted_count", DbType.Int32, grantedCount);

            return db.ExecuteNonQuery(cmd, trans) > 0;
        }

        internal static bool DeleteRuntime(string applyId, string hospitalId, string productId, Database db, DbTransaction trans)
        {
            var sql = @"delete goods_inventory_runtime where apply_id=@p_apply_id and product_id=@p_product_id and hospital_id=@p_hospital_id";

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, applyId);
            db.AddInParameter(cmd, "p_product_id", DbType.String, productId);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);

            return db.ExecuteNonQuery(cmd, trans) > 0;
        }

        internal static void Reduce(string serialId, string hospitalId, int count, Database db, DbTransaction trans)
        {
            var sql = "update goods_inventory set granted_count+=@p_granted_count where serial_id=@p_serial_id and hospital_id=@p_hospital_id";

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_granted_count", DbType.Int32, count);
            db.AddInParameter(cmd, "p_serial_id", DbType.String, serialId);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);

            db.ExecuteNonQuery(cmd, trans);
        }


        public static IList<ProductInventoryEntity> QueryProductInventory(ProductInventoryCondition condition)
        {
            var sql = string.Format(@"select * 
                    from (
                        select
                            sum(original_count) original_count, sum(split_count) split_count, 
	                        sum(apply_count) apply_count, sum(granted_count) granted_count,
	                        a.storeroom_id,a.vendor_id,a.product_id,b.need_split,a.expired_date,ri.received_date
                        from goods_inventory a 
                                join goods_serial b 
                                    on a.serial_id = b.id and a.hospital_id = b.hospital_id and a.hospital_id = @p_hospital_id
                                join(select rf.hospital_id
                                , case when rfi.receive_id is null then  rf.serial_id else rfi.serial_id end serial_id
                                , case when rfi.receive_id is null then  rf.confirmed_time else rfi.confirmed_time end received_date
                                from receive_form rf left join receive_form_items rfi on rf.id = rfi.receive_id
                                where rf.is_confirmed = 1 and rf.hospital_id = @p_hospital_id) ri 
                                    on a.serial_id = ri.serial_id and a.hospital_id = b.hospital_id
                        where 1=1 {0} {1}
                        group by b.need_split,a.storeroom_id,a.product_id,a.vendor_id,a.expired_date,ri.received_date) ap",
                        string.IsNullOrEmpty(condition.StoreroomId) ? string.Empty : "and a.storeroom_id=@p_storeroom_id",
                        string.IsNullOrEmpty(condition.ProductId) ? string.Empty : "and a.product_id=@p_product_id");
            sql+=(string.IsNullOrEmpty(condition.NotIncludeZero) ? string.Empty : " where (need_split>0 and original_count-split_count>0) or apply_count-granted_count>0");
            sql += " order by storeroom_id,expired_date,product_id,need_split";
            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_hospital_id", DbType.String, condition.HospitalId);
            if (!string.IsNullOrEmpty(condition.StoreroomId))
            {
                db.AddInParameter(cmd, "p_storeroom_id", DbType.String, condition.StoreroomId);
            }
            if (!string.IsNullOrEmpty(condition.ProductId))
            {
                db.AddInParameter(cmd, "p_product_id", DbType.String, condition.ProductId);
            }

            var list = new List<ProductInventoryEntity>();
            using (var reader = db.ExecuteReader(cmd))
            {
                while(reader.Read())
                {
                    var originalCount = ReadItem(reader, "original_count");
                    var splitCount = ReadItem(reader, "split_count");
                    var applyCount = ReadItem(reader, "apply_count");
                    var grantedCount = ReadItem(reader, "granted_count");

                    var entity = new ProductInventoryEntity();
                    entity.NeedSplit = reader.GetBoolean(reader.GetOrdinal("need_split"));
                    entity.ProductId = reader["product_id"].ToString();
                    entity.StoreroomId = reader["storeroom_id"].ToString();
                    entity.VendorID = reader["vendor_id"].ToString();
                    entity.ExpiredDate = reader.GetDateTime(reader.GetOrdinal("expired_date"));
                    entity.ReceivedDate = reader.GetDateTime(reader.GetOrdinal("received_date"));

                    entity.SplitableCount = entity.NeedSplit ? originalCount - splitCount : 0;
                    entity.ApplyCount = applyCount - grantedCount;

                    list.Add(entity);
                }
            }

            return list;
        }

        private static int ReadItem(IDataReader reader, string name)
        {
            return reader[name] == DBNull.Value ? 0 : reader.GetInt32(reader.GetOrdinal(name));
        }





        public static GoodsInventoryEntity Get(string productId, string hospitalId)
        {
            var sql = string.Format("select {0} from goods_inventory where product_id=@p_product_id and hospital_id=@p_hospital_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_product_id", DbType.String, productId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            using(var reader = db.ExecuteReader(dc))
            {
                if(reader.Read())
                {
                    var entity = new GoodsInventoryEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static IList<GoodsInventoryEntity> Get(IList<string> productIds, string hospitalId)
        {
            return Get(productIds, hospitalId, null, null);
        }

        public static IList<GoodsInventoryEntity> Get(IList<string> productIds, string hospitalId, Database db, DbTransaction trans)
        {
            var sql = string.Format("select {0} from goods_inventory {2} where product_id in ('{1}') and hospital_id=@p_hospital_id", COLUMN_SQL, string.Join("','", productIds), TransHelper.UpdateLock(trans));

            if(db == null)
            {
                db = DatabaseFactory.CreateDatabase();
            } 
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            var list = new List<GoodsInventoryEntity>();
            var reader = trans == null ? db.ExecuteReader(dc) : db.ExecuteReader(dc, trans);
            try
            {
                while (reader.Read())
                {
                    var entity = new GoodsInventoryEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }
            finally
            {
                reader.Close();
            }

            return list;
        }

        public static bool Save(string productId, string hospitalId, Database db, DbTransaction trans)
        {
            if(!Exist(productId, hospitalId, db, trans))
            {
                return Add(productId, hospitalId, db, trans);
            }
            return true;
        }

        private static bool Add(string productId, string hospitalId, Database db, DbTransaction trans)
        {
            var sql = string.Format(@"insert into goods_inventory({0}) 
values(@p_id, @p_product_id, @p_hospital_id, @p_original_count, @p_usable_count, @p_granted_count, @p_split_count)", COLUMN_SQL);

            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_id", DbType.String, Guid.NewGuid().ToString());
            db.AddInParameter(dc, "p_product_id", DbType.String, productId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(dc, "p_original_count", DbType.Int32, 0);
            db.AddInParameter(dc, "p_usable_count", DbType.Int32, 0);
            db.AddInParameter(dc, "p_granted_count", DbType.Int32, 0);
            db.AddInParameter(dc, "p_split_count", DbType.Int32, 0);

            return db.ExecuteNonQuery(dc, trans) > 0;
        }
        
        private static bool Exist(string productId, string hospitalId, Database db, DbTransaction trans)
        {
            var sql = string.Format("select count(*) from goods_inventory {0} where product_id=@p_product_id and hospital_id=@p_hospital_id", TransHelper.UpdateLock(trans));

            var dc = db.GetSqlStringCommand(sql);
            
            db.AddInParameter(dc, "p_product_id", DbType.String, productId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            using (var reader = db.ExecuteReader(dc, trans))
            {
                reader.Read();

                return reader.GetInt32(0) > 0;
            }
        }

        public static bool AddOriginalCount(string productId, string hospitalId, int count, Database db, DbTransaction trans)
        {
            var sql = "update goods_inventory set original_count = original_count + @p_count where product_id=@p_product_id and hospital_id=@p_hospital_id";

            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_product_id", DbType.String, productId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(dc, "p_count", DbType.Int32, count);

            if(db.ExecuteNonQuery(dc, trans) == 0)
            {
                Add(productId, hospitalId, db, trans);
                return db.ExecuteNonQuery(dc, trans) > 0;
            }

            return true;
        }

        public static bool AddUsableCount(string productId, string hospitalId, int count, Database db, DbTransaction trans)
        {
            var sql = "update goods_inventory set usable_count = usable_count + @p_count where product_id=@p_product_id and hospital_id=@p_hospital_id";

            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_product_id", DbType.String, productId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(dc, "p_count", DbType.Int32, count);
            
            return db.ExecuteNonQuery(dc, trans) > 0;
        }

        public static bool AddGrantedCount(string productId, string hospitalId, int count, Database db, DbTransaction trans)
        {
            var sql = "update goods_inventory set granted_count = granted_count + @p_count where product_id=@p_product_id and hospital_id=@p_hospital_id";

            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_product_id", DbType.String, productId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(dc, "p_count", DbType.Int32, count);

            return db.ExecuteNonQuery(dc, trans) > 0;
        }
        
        public static bool ReduceGrantedCount(string productId, string hospitalId, int count, Database db, DbTransaction trans)
        {
            var sql = "update goods_inventory set granted_count = granted_count - @p_count where product_id=@p_product_id and hospital_id=@p_hospital_id";

            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_product_id", DbType.String, productId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(dc, "p_count", DbType.Int32, count);

            return db.ExecuteNonQuery(dc, trans) > 0;
        }
        
        public static bool IncreaseSplit(string productId, string hospitalId, Database db, DbTransaction trans)
        {
            var sql = "update goods_inventory set split_count = isnull(split_count, 0) + 1 where product_id=@p_product_id and hospital_id=@p_hospital_id";

            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_product_id", DbType.String, productId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

            return db.ExecuteNonQuery(dc, trans) > 0;
        }
        #region Query
        public static IList<GoodsInventoryEntity> Query(DateRangeCondition condition)
        {
            var sql = string.Format(GetBaseQuerySql(condition), "a.id,a.product_id,a.hospital_id,a.original_count,a.usable_count,a.granted_count,a.split_count,b.name,b.full_name");
            sql += " order by name";

            var list = new List<GoodsInventoryEntity>();
            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            AddParameter(dc, db, condition);

            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new GoodsInventoryEntity();
                    entity.Init(reader);
                    //entity.ProductName = reader["name"].ToString();
                    //entity.ProductFullName = reader["full_name"].ToString();

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<GoodsInventoryEntity> Query(DateRangeCondition condition, PagerInfo pager)
        {
            pager.ComputePageCount(QueryCount(condition));

            var list = new List<GoodsInventoryEntity>();
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

            var sql = string.Format(GetBaseQuerySql(condition), "a.id,a.product_id,a.hospital_id,a.original_count,a.usable_count,a.granted_count,a.split_count,b.name,b.full_name");
            sql = @"SELECT * FROM
            (
                SELECT ROW_NUMBER() OVER(" + orderSql + @") pid,*
                FROM (" + sql + @") t 
            ) t1 WHERE t1.pid BETWEEN @p_pageNo * @p_pageSize + 1 AND (@p_pageNo + 1) * @p_pageSize";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            AddParameter(dc, db, condition);

            db.AddInParameter(dc, "p_pageNo", DbType.Int32, pager.PageIndex);
            db.AddInParameter(dc, "p_pageSize", DbType.Int32, pager.PageSize);

            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new GoodsInventoryEntity();
                    entity.Init(reader);
                    //entity.ProductName = reader["name"].ToString();
                    //entity.ProductFullName = reader["full_name"].ToString();

                    list.Add(entity);
                }
            }

            return list;
        }

        private static string GetBaseQuerySql(DateRangeCondition condition)
        {
            var sql = "SELECT {0} FROM goods_inventory a left join products b on a.product_id = b.id WHERE " + GetConditionSql(condition);
            return sql;
        }

        private static int QueryCount(DateRangeCondition condition)
        {
            var sql = string.Format(GetBaseQuerySql(condition), "COUNT(a.id)");

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            AddParameter(dc, db, condition);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                reader.Read();

                return reader.GetInt32(0);
            }
        }

        private static string GetConditionSql(DateRangeCondition condition)
        {
            var conditionSql = @" 1=1 and a.hospital_id = @p_hospital_id ";
            
            if (!string.IsNullOrEmpty(condition.Content))
            {
                conditionSql += " and (b.name like @p_name or b.full_name like @p_name)";
            }

            return conditionSql;
        }

        private static void AddParameter(DbCommand dc, Database db, DateRangeCondition condition)
        {
            db.AddInParameter(dc, "p_hospital_id", DbType.String, condition.HospitalId);

            if (!string.IsNullOrEmpty(condition.Content))
            {
                db.AddInParameter(dc, "p_name", DbType.String, "%" + condition.Content + "%");
            }
        }
        #endregion


        public static GoodsInventoryRuntimeEntity GetRuntime(string applyId, string hospitalId, string productId, Database db, DbTransaction dbTrans)
        {
            var sql = "select id,hospital_id,apply_id,product_id,apply_count,granted_count from goods_inventory_runtime where apply_id=@p_apply_id and hospital_id=@p_hospital_id and product_id=@p_product_id";

            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_apply_id", DbType.String, applyId);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(cmd, "p_product_id", DbType.String, productId);
            using (var reader = db.ExecuteReader(cmd, dbTrans))
            {
                while (reader.Read())
                {
                    var entity = new GoodsInventoryRuntimeEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static GoodsInventoryRuntimeEntity SaveRuntime(GoodsInventoryRuntimeEntity entity, Database db, DbTransaction trans)
        {
            var sql = string.Empty;

            if (string.IsNullOrEmpty(entity.Id))
            {
                sql = @"insert into 
goods_inventory_runtime(id,hospital_id,apply_id,product_id,apply_count,granted_count)
values(@p_id,@p_hospital_id,@p_apply_id,@p_product_id,@p_apply_count,@p_granted_count)";

                entity.Id = Guid.NewGuid().ToString();
            }
            else
            {
                sql = @"update goods_inventory_runtime set
hospital_id=@p_hospital_id
,apply_id=@p_apply_id
,product_id=@p_product_id
,apply_count=@p_apply_count
,granted_count=@p_granted_count
where id=@p_id";      
            }

            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, entity.HospitalId);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, entity.ApplyId);
            db.AddInParameter(cmd, "p_product_id", DbType.String, entity.ProductId);
            db.AddInParameter(cmd, "p_apply_count", DbType.Int32, entity.ApplyCount);
            db.AddInParameter(cmd, "p_granted_count", DbType.Int32, entity.GrantedCount);

            db.ExecuteNonQuery(cmd, trans);

            return entity;
        }

        public static int GetCount(string hospitalId, string storeroomId, string productId)
        {
            var sql = @"select sum(apply_count-granted_count) 
from goods_inventory 
where hospital_id=@p_hospital_id 
and storeroom_id=@p_storeroom_id 
and product_id=@p_product_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_product_id", DbType.String, productId);
            db.AddInParameter(cmd, "p_storeroom_id", DbType.String, storeroomId);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);

            var totalCount = 0;
            using (var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    totalCount = reader[0] == DBNull.Value ? 0 : reader.GetInt32(0);
                }
            }

            return totalCount;
        }

        public static int GetApplyCount(string hospitalId, string storeroomId, string productId)
        {
            var sql = @"select sum(apply_count) 
from goods_inventory_runtime gir
where hospital_id = @p_hospital_id 
and product_id = @p_product_id
and (exists(select 1 from apply_form af where af.id = gir.apply_id and af.hospital_id = gir.hospital_id and af.storeroom_id = @p_storeroom_id)
or exists(select 1 from move_form mf where mf.id = gir.apply_id and mf.hospital_id = gir.hospital_id and mf.from_storeroom = @p_storeroom_id))
";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_product_id", DbType.String, productId);
            db.AddInParameter(cmd, "p_storeroom_id", DbType.String, storeroomId);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);

            var totalCount = 0;
            using (var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    totalCount = reader[0] == DBNull.Value ? 0 : reader.GetInt32(0);
                }
            }

            return totalCount;
        }


        public static void Adjust(string serialId, int usableCount, int applyCount, Database db, DbTransaction trans)
        {
            var sql = "update goods_inventory set usable_count=usable_count-@p_usable_count, apply_count=apply_count-@p_apply_count where serial_id=@p_serial_id";

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_usable_count", DbType.Int32, usableCount);
            db.AddInParameter(cmd, "p_apply_count", DbType.Int32, applyCount);
            db.AddInParameter(cmd, "p_serial_id", DbType.String, serialId);

            db.ExecuteNonQuery(cmd, trans);
        }
    }
}
