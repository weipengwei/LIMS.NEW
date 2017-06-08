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
    public static class ApplyFormRepository
    {
        private const string COLUMN_SQL = "id,form_no,filler_id,applyer,apply_unit_id,apply_date,storeroom_id,hospital_id,status,show_split,created_id,created_time,updated_id,updated_time";

        #region Load Apply Form or Items
        public static ApplyFormEntity Get(string id)
        {
            var sql = string.Format("select {0} from apply_form where id=@p_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_id", DbType.String, id);

            using (var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    var entity = new ApplyFormEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static IList<ApplyFormItemEntity> GetItems(string applyId)
        {
            var sql = string.Format("select id,apply_id,product_id,count from apply_form_items where apply_id=@p_apply_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_apply_id", DbType.String, applyId);

            var list = new List<ApplyFormItemEntity>();
            using (var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new ApplyFormItemEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static ApplyFormItemEntity GetItem(string id)
        {
            var sql = string.Format("select id,apply_id,product_id,count from apply_form_items where id=@p_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_id", DbType.String, id);

            using (var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    var entity = new ApplyFormItemEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static ApplyFormItemEntity GetItem(string applyId, string productId)
        {
            var sql = string.Format("select id,apply_id,product_id,count from apply_form_items where apply_id=@p_apply_id and product_id=@p_product_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_apply_id", DbType.String, applyId);
            db.AddInParameter(cmd, "p_product_id", DbType.String, productId);

            using (var reader = db.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    var entity = new ApplyFormItemEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }
        #endregion


        #region Edit Apply Form
        public static void Create(ApplyFormEntity entity)
        {
            var sql = @"
insert into apply_form(id,form_no,filler_id,applyer,apply_unit_id,apply_date,hospital_id,storeroom_id,status,created_id,created_time)
values(@p_id,@p_form_no,@p_filler_id,@p_applyer,@p_apply_unit_id,@p_apply_date,@p_hospital_id,@p_storeroom_id,@p_status,@p_created_id,@p_created_time)";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            entity.Id = Guid.NewGuid().ToString();

            db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);
            db.AddInParameter(cmd, "p_form_no", DbType.Int32, entity.FormNo);
            db.AddInParameter(cmd, "p_filler_id", DbType.String, entity.FillerId);
            db.AddInParameter(cmd, "p_apply_date", DbType.DateTime, entity.ApplyDate);
            db.AddInParameter(cmd, "p_applyer", DbType.String, entity.Applyer);
            db.AddInParameter(cmd, "p_apply_unit_id", DbType.String, entity.ApplyUnitId);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, entity.HospitalId);
            db.AddInParameter(cmd, "p_storeroom_id", DbType.String, entity.StoreroomId == null ? string.Empty : entity.StoreroomId);
            db.AddInParameter(cmd, "p_status", DbType.String, entity.Status);
            db.AddInParameter(cmd, "p_created_id", DbType.String, entity.CreatedId);
            db.AddInParameter(cmd, "p_created_time", DbType.DateTime, entity.CreatedTime);

            db.ExecuteNonQuery(cmd);
        }

        public static void Update(ApplyFormEntity entity)
        {
            var sql = @"update apply_form 
set applyer=@p_applyer,apply_unit_id=@p_apply_unit_id,storeroom_id=@p_storeroom_id,updated_id=@p_updated_id,updated_time=@p_updated_time
where id=@p_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);
            db.AddInParameter(cmd, "p_applyer", DbType.String, entity.Applyer);
            db.AddInParameter(cmd, "p_apply_unit_id", DbType.String, entity.ApplyUnitId);
            db.AddInParameter(cmd, "p_storeroom_id", DbType.String, entity.StoreroomId);
            db.AddInParameter(cmd, "p_updated_id", DbType.String, entity.UpdatedId);
            db.AddInParameter(cmd, "p_updated_time", DbType.DateTime, entity.UpdatedTime);

            db.ExecuteNonQuery(cmd);
        }

        public static void CreateItem(ApplyFormItemEntity entity)
        {
            var sql = @"insert into apply_form_items(id,apply_id,product_id,count) 
values(@p_id,@p_apply_id,@p_product_id,@p_count)";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            entity.Id = Guid.NewGuid().ToString();

            db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, entity.ApplyId);
            db.AddInParameter(cmd, "p_product_id", DbType.String, entity.ProductId);
            db.AddInParameter(cmd, "p_count", DbType.Int32, entity.Count);

            db.ExecuteNonQuery(cmd);
        }

        public static bool UpdateItem(ApplyFormItemEntity entity)
        {
            var sql = @"update apply_form_items set count = count+@p_count where id=@p_id and product_id=@p_product_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);
            db.AddInParameter(cmd, "p_product_id", DbType.String, entity.ProductId);
            db.AddInParameter(cmd, "p_count", DbType.Int32, entity.Count);

            return db.ExecuteNonQuery(cmd) > 0;
        }

        internal static bool UpdateItem(ApplyFormItemEntity entity, Database db, DbTransaction trans)
        {
            var sql = @"update apply_form_items set count = @p_count where id=@p_id and product_id=@p_product_id";
            
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);
            db.AddInParameter(cmd, "p_product_id", DbType.String, entity.ProductId);
            db.AddInParameter(cmd, "p_count", DbType.Int32, entity.Count);

            return db.ExecuteNonQuery(cmd, trans) > 0;
        }

        public static void DeleteItems(IList<string> ids)
        {
            if(ids == null || ids.Count == 0)
            {
                return;
            }

            var sql = string.Format("delete apply_form_items where id in ('{0}')", string.Join("','", ids));

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.ExecuteNonQuery(cmd);
        }

        public static void DeleteItem(string id)
        {
            var sql = "delete apply_form_items where id=@p_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, id);

            db.ExecuteNonQuery(cmd);
        }

        private static void DeleteItem(string id, Database db, DbTransaction trans)
        {
            var sql = "delete apply_form_items where id=@p_id";
            
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, id);

            db.ExecuteNonQuery(cmd);
        }

        public static bool ValidateInventory(string applyId, string storeroomId, string hospitalId, out IList<ApplyFormItemEntity> invalidList)
        {
            invalidList = new List<ApplyFormItemEntity>();

            var list = GetItems(applyId);
            foreach(var item in list)
            {
                var inventory = GoodsInventoryRepository.CountLeft(item.ProductId, storeroomId, hospitalId);
                if(inventory < item.Count)
                {
                    invalidList.Add(item);
                }
            }

            return invalidList.Count == 0;
        }
        #endregion


        #region Confirm
        public static void Confirm(string id, string hospitalId, string userId)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        Confirm(id, userId, db, trans);
                        CreateRuntime(id, hospitalId, db, trans);

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

        private static void Confirm(string id, string userId, Database db, DbTransaction trans)
        {
            var sql = @"update apply_form 
set status=@p_status,updated_id=@p_updated_id,updated_time=@p_updated_time
where id=@p_id";

            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_id", DbType.String, id);
            db.AddInParameter(cmd, "p_status", DbType.String, ApplyFormStatus.Granting);
            db.AddInParameter(cmd, "p_updated_id", DbType.String, userId);
            db.AddInParameter(cmd, "p_updated_time", DbType.DateTime, DateTime.Now);

            db.ExecuteNonQuery(cmd, trans);
        }

        private static void CreateRuntime(string id, string hospitalId, Database db, DbTransaction trans)
        {
            var items = GetItems(id);

            var list = new List<GoodsInventoryRuntimeEntity>();
            foreach (var item in items)
            {
                GoodsInventoryRepository.CreateRuntime(new GoodsInventoryRuntimeEntity
                {
                    HospitalId = hospitalId,
                    ApplyId = item.ApplyId,
                    ProductId = item.ProductId,
                    ApplyCount = item.Count,
                    GrantedCount = 0
                }, db, trans);
            }
        }
        #endregion


        #region Query
        public static IList<ApplyFormEntity> Query(ApplyQueryCondition condition, PagerInfo pager)
        {
            var list = new List<ApplyFormEntity>();

            pager.ComputePageCount(QueryCount(condition));

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
                orderSql += "form_no desc";
            }

            var sql = string.Format(@"SELECT {0} FROM apply_form WHERE {1}", COLUMN_SQL, GetConditionSql(condition));

            sql = @"SELECT * FROM
                    (
                        SELECT ROW_NUMBER() OVER(" + orderSql + @") pid," + COLUMN_SQL + @"
                        FROM (" + sql + @") t            
                    ) t1 WHERE t1.pid BETWEEN @p_pageNo * @p_pageSize + 1 AND (@p_pageNo + 1) * @p_pageSize ";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            AddParameter(dc, db, condition);
            db.AddInParameter(dc, "p_pageNo", DbType.Int32, pager.PageIndex);
            db.AddInParameter(dc, "p_pageSize", DbType.Int32, pager.PageSize);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new ApplyFormEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        private static int QueryCount(ApplyQueryCondition condition)
        {
            var sql = "SELECT COUNT(id) FROM apply_form WHERE ";

            sql += GetConditionSql(condition);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            AddParameter(dc, db, condition);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                reader.Read();

                return reader.GetInt32(0);
            }
        }

        private static string GetConditionSql(ApplyQueryCondition condition)
        {
            var conditionSql = " 1 =1 and filler_id=@p_user_id and hospital_id=@p_hospital_id";
            if(condition is GrantV1QueryCondition)
            {
                conditionSql = @" 1=1 
and (storeroom_id in (select unit_id from user_privilege where user_id = @p_user_id and unit_root_id = @p_hospital_id and operate=1)
    or filler_id=@p_user_id and hospital_id=@p_hospital_id)";
            }
            else if(condition is GrantQueryCondition)
            {
                conditionSql = @" 1=1 
and storeroom_id in (select unit_id from user_privilege where user_id = @p_user_id and unit_root_id = @p_hospital_id and operate=1)";
            }
            
            if (condition.FormNo.HasValue)
            {
                conditionSql += " and form_no = @p_form_no";
            }

            if (condition.NotIncludeNotGrant)
            {
                conditionSql += string.Format(" and status not in (@p_saved, @p_cancelled)");
            }

            if (!string.IsNullOrEmpty(condition.Status))
            {
                conditionSql += string.Format(" and status=@p_status", condition.Status);
            }
            else
            {
                if (condition is GrantQueryCondition)
                {
                    conditionSql += " and (status=@p_granting or status=@p_granted)";
                }
            }

            return conditionSql;
        }

        private static void AddParameter(DbCommand dc, Database db, ApplyQueryCondition condition)
        {
            db.AddInParameter(dc, "p_user_id", DbType.String, condition.UserId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, condition.HospitalId);
            
            if (condition.FormNo.HasValue)
            {
                db.AddInParameter(dc, "p_form_no", DbType.Int32, condition.FormNo.Value);
            }

            if (condition.NotIncludeNotGrant)
            {
                db.AddInParameter(dc, "p_saved", DbType.String, ApplyFormStatus.Saved);
                db.AddInParameter(dc, "p_cancelled", DbType.String, ApplyFormStatus.Cancelled);
            }

            if (!string.IsNullOrEmpty(condition.Status))
            {
                db.AddInParameter(dc, "p_status", DbType.String, condition.Status);
            }
            else
            {
                if (condition is GrantQueryCondition)
                {
                    db.AddInParameter(dc, "p_granting", DbType.String, ApplyFormStatus.Granting);
                    db.AddInParameter(dc, "p_granted", DbType.String, ApplyFormStatus.Granted);
                }
            }
        }
        #endregion


        #region Cancel
        public static void Cancel(string id, string hospitalId, string userId)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        Cancel(id, hospitalId, userId, db, trans);

                        GoodsInventoryRepository.DeleteRuntime(id, hospitalId, db, trans);
                        GoodsRepsitory.DeleteRuntime(id, hospitalId, db, trans);

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

        private static void Cancel(string id, string hospitalId, string userId, Database db, DbTransaction trans)
        {
            var sql = @"update apply_form 
set status=@p_status,updated_id=@p_updated_id,updated_time=@p_updated_time
where id=@p_id and hospital_id=@p_hospital_id";

            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_id", DbType.String, id);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(cmd, "p_status", DbType.String, ApplyFormStatus.Cancelled);
            db.AddInParameter(cmd, "p_updated_id", DbType.String, userId);
            db.AddInParameter(cmd, "p_updated_time", DbType.DateTime, DateTime.Now);

            db.ExecuteNonQuery(cmd, trans);
        }
        #endregion


        #region Grant
        public static void Grant(string id, GoodsEntity goods, int count, string hospitalId)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        GoodsInventoryRepository.UpdateGrantRuntime(id, hospitalId, goods.ProductId, count, db, trans);
                        GoodsRepsitory.CreateRuntime(new GoodsRuntimeEntity
                        {
                            ApplyId = id,
                            HospitalId = hospitalId,
                            SerialId = goods.SerialId,
                            Barcode = goods.Barcode,
                            Count = count,
                            ProductId = goods.ProductId
                        }, db, trans);

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

        public static ApplyFormItemEntity GrantV1(string id, GoodsEntity goods, int originCount, int count, string hospitalId)
        {
            var originItem = GetItem(id, goods.ProductId);

            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var appendCount = count - originCount;

                        if(originItem == null)
                        {
                            originItem = new ApplyFormItemEntity
                            {
                                ApplyId = id,
                                ProductId = goods.ProductId,
                                Count = count
                            };
                        }
                        else
                        {
                            originItem.Count += appendCount;
                        }

                        if(string.IsNullOrEmpty(originItem.Id))
                        {
                            CreateItem(originItem, db, trans);
                            GoodsInventoryRepository.CreateRuntime(new GoodsInventoryRuntimeEntity
                            {
                                ApplyId = id,
                                ProductId = goods.ProductId,
                                HospitalId = hospitalId,
                                ApplyCount = count,
                                GrantedCount = count
                            }, db, trans);
                        }
                        else
                        {
                            UpdateItem(originItem, db, trans);
                            GoodsInventoryRepository.AppendRuntime(id, hospitalId, goods.ProductId, appendCount, appendCount, db, trans);
                        }

                        UpdateStoreroom(id, goods.StoreroomId, db, trans);
                        GoodsRepsitory.DeleteRuntime(id, goods.Barcode, hospitalId, db, trans);
                        GoodsRepsitory.CreateRuntime(new GoodsRuntimeEntity
                        {
                            ApplyId = id,
                            HospitalId = hospitalId,
                            SerialId = goods.SerialId,
                            Barcode = goods.Barcode,
                            Count = count,
                            ProductId = goods.ProductId
                        }, db, trans);

                        trans.Commit();

                        return originItem;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public static ApplyFormItemEntity GrantV1(string id, GoodsEntity goods, string hospitalId)
        {
            var originItem = GetItem(id, goods.ProductId);

            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var count = 1;

                        if (originItem == null)
                        {
                            originItem = new ApplyFormItemEntity
                            {
                                ApplyId = id,
                                ProductId = goods.ProductId,
                                Count = count
                            };
                        }
                        else
                        {
                            originItem.Count += count;
                        }

                        if (string.IsNullOrEmpty(originItem.Id))
                        {
                            CreateItem(originItem, db, trans);
                            GoodsInventoryRepository.CreateRuntime(new GoodsInventoryRuntimeEntity
                            {
                                ApplyId = id,
                                ProductId = goods.ProductId,
                                HospitalId = hospitalId,
                                ApplyCount = count,
                                GrantedCount = count
                            }, db, trans);
                        }
                        else
                        {
                            UpdateItem(originItem, db, trans);
                            GoodsInventoryRepository.AppendRuntime(id, hospitalId, goods.ProductId, count, count, db, trans);
                        }

                        UpdateStoreroom(id, goods.StoreroomId, db, trans);
                        //GoodsRepsitory.DeleteRuntime(id, goods.Barcode, hospitalId, db, trans);
                        GoodsRepsitory.CreateRuntime(new GoodsRuntimeEntity
                        {
                            ApplyId = id,
                            HospitalId = hospitalId,
                            SerialId = goods.SerialId,
                            Barcode = goods.Barcode,
                            Count = count,
                            ProductId = goods.ProductId
                        }, db, trans);

                        trans.Commit();

                        return originItem;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public static ApplyFormItemEntity UndoGrant(string id, GoodsEntity goods, string hospitalId)
        {
            var originItem = GetItem(id, goods.ProductId);
            var grantedCount = GoodsRepsitory.SumRuntime(goods.Barcode, id, hospitalId);

            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        GoodsRepsitory.DeleteRuntime(id, goods.Barcode, hospitalId, db, trans);

                        var reduceCount = grantedCount * -1;
                        originItem.Count += reduceCount;

                        if(originItem.Count > 0)
                        {
                            UpdateItem(originItem, db, trans);
                            GoodsInventoryRepository.AppendRuntime(id, hospitalId, goods.ProductId, reduceCount, reduceCount, db, trans);
                        }
                        else
                        {
                            DeleteItem(originItem.Id);
                            GoodsInventoryRepository.DeleteRuntime(id, hospitalId, goods.ProductId, db, trans);
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return originItem;
        }

        private static void UpdateStoreroom(string id, string storeroomId, Database db, DbTransaction trans)
        {
            var sql = "update apply_form set storeroom_id=@p_storeroom_id where id=@p_id";

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_storeroom_id", DbType.String, storeroomId);
            db.AddInParameter(cmd, "p_id", DbType.String, id);

            db.ExecuteNonQuery(cmd, trans);
        }
        
        private static void CreateItem(ApplyFormItemEntity entity, Database db, DbTransaction trans)
        {
            var sql = @"insert into apply_form_items(id,apply_id,product_id,count) 
values(@p_id,@p_apply_id,@p_product_id,@p_count)";

            var cmd = db.GetSqlStringCommand(sql);

            entity.Id = Guid.NewGuid().ToString();

            db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, entity.ApplyId);
            db.AddInParameter(cmd, "p_product_id", DbType.String, entity.ProductId);
            db.AddInParameter(cmd, "p_count", DbType.Int32, entity.Count);

            db.ExecuteNonQuery(cmd, trans);
        }
    #endregion


        #region Pass
        public static void PassV1(ApplyFormEntity entity)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        Update(entity, db, trans);

                        BuildBarcodes(entity.Id, db, trans);

                        GoodsInventoryRepository.DeleteRuntime(entity.Id, entity.HospitalId, db, trans);
                        GoodsRepsitory.DeleteRuntime(entity.Id, entity.HospitalId, db, trans);

                        Pass(entity.Id, entity.HospitalId, entity.UpdatedId, db, trans);

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

        private static void Update(ApplyFormEntity entity, Database db, DbTransaction trans)
        {
            var sql = @"update apply_form 
set applyer=@p_applyer,apply_unit_id=@p_apply_unit_id,updated_id=@p_updated_id,updated_time=@p_updated_time
where id=@p_id";
            
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_id", DbType.String, entity.Id);
            db.AddInParameter(cmd, "p_applyer", DbType.String, entity.Applyer);
            db.AddInParameter(cmd, "p_apply_unit_id", DbType.String, entity.ApplyUnitId);
            db.AddInParameter(cmd, "p_updated_id", DbType.String, entity.UpdatedId);
            db.AddInParameter(cmd, "p_updated_time", DbType.DateTime, entity.UpdatedTime);

            db.ExecuteNonQuery(cmd, trans);
        }

        public static void Pass(string id, string hospitalId, string userId)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        BuildBarcodes(id, db, trans);

                        GoodsInventoryRepository.DeleteRuntime(id, hospitalId, db, trans);
                        GoodsRepsitory.DeleteRuntime(id, hospitalId, db, trans);

                        Pass(id, hospitalId, userId, db, trans);

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

        private static void Pass(string id, string hospitalId, string userId, Database db, DbTransaction trans)
        {
            var sql = @"update apply_form 
set status=@p_status,updated_id=@p_updated_id,updated_time=@p_updated_time
where id=@p_id and hospital_id=@p_hospital_id";

            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_id", DbType.String, id);
            db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);
            db.AddInParameter(cmd, "p_status", DbType.String, ApplyFormStatus.Granted);
            db.AddInParameter(cmd, "p_updated_id", DbType.String, userId);
            db.AddInParameter(cmd, "p_updated_time", DbType.DateTime, DateTime.Now);

            db.ExecuteNonQuery(cmd, trans);
        }

        private static void BuildBarcodes(string id, Database db, DbTransaction trans)
        {
            var list = GoodsRepsitory.GetRuntime(id);

            var sql = @"insert into apply_form_barcodes(id,hospital_id,apply_id,barcode,count,serial_id,product_id)
values(@p_id,@p_hospital_id,@p_apply_id,@p_barcode,@p_count,@p_serial_id,@p_product_id)";


            foreach(var item in list)
            {
                var cmd = db.GetSqlStringCommand(sql);
                
                db.AddInParameter(cmd, "p_id", DbType.String, item.Id);
                db.AddInParameter(cmd, "p_hospital_id", DbType.String, item.HospitalId);
                db.AddInParameter(cmd, "p_apply_id", DbType.String, item.ApplyId);
                db.AddInParameter(cmd, "p_barcode", DbType.String, item.Barcode);
                db.AddInParameter(cmd, "p_count", DbType.Int32, item.Count);
                db.AddInParameter(cmd, "p_serial_id", DbType.String, item.SerialId);
                db.AddInParameter(cmd, "p_product_id", DbType.String, item.ProductId);

                db.ExecuteNonQuery(cmd, trans);

                GoodsRepsitory.Reduce(item.Barcode, item.Count, item.HospitalId, db, trans);
                GoodsInventoryRepository.Reduce(item.SerialId, item.HospitalId, item.Count, db, trans);
            }
        }
        #endregion


        #region Split Barcode
        public static bool ShowSplit(string id)
        {
            var barcodes = GetApplyBarcodes(id);
            var serials = GetApplyGoodsSerials(id, barcodes);

            var showSplit = serials.Count > 0;
            if (showSplit)
            {
                UpdateSplitStatus(id, showSplit);
            }

            return showSplit;
        }

        private static IList<GoodsSerialEntity> GetApplyGoodsSerials(string id, IList<ApplyFormBarcodeEntity> barcodes)
        {
            var serials = CacheHelper.Get(FormSerialKey(id), () =>
            {
                var serialsId = barcodes.Select(item => item.SerialId).Distinct().ToList();

                return GoodsSerialRepository.GetSplitSerials(serialsId);
            });

            return serials;
        }

        public static IList<ApplyFormBarcodeEntity> GetApplyBarcodes(string id)
        {
            var sql = "select id,apply_id,hospital_id,barcode,count,serial_id,product_id from apply_form_barcodes where apply_id=@p_id";

            var database = DatabaseFactory.CreateDatabase();
            var cmd = database.GetSqlStringCommand(sql);
            database.AddInParameter(cmd, "p_id", DbType.String, id);

            var barcodes = new List<ApplyFormBarcodeEntity>();
            using(var reader = database.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new ApplyFormBarcodeEntity();
                    entity.Init(reader);

                    barcodes.Add(entity);
                }
            }

            return barcodes;
        }

        private static void UpdateSplitStatus(string id, bool showSplit)
        {
            var sql = "update apply_form set show_split=@p_show_split where id=@p_id";

            var database = DatabaseFactory.CreateDatabase();
            var cmd = database.GetSqlStringCommand(sql);
            database.AddInParameter(cmd, "p_id", DbType.String, id);
            database.AddInParameter(cmd, "p_show_split", DbType.Boolean, showSplit);

            database.ExecuteNonQuery(cmd);
        }

        public static IList<PrintBarcodeModel> GetBarcodeChildren(string id)
        {
            var barcodes = GetApplyBarcodes(id);
            var serials = GetApplyGoodsSerials(id, barcodes).ToDictionary(item => item.Id);

            var map = barcodes.GroupBy(item => item.Barcode).ToDictionary(item => item.Key, item => item.ToList());

            var sql = "select id,apply_id,barcode,child_barcode,expired_date,is_printed,created_id,created_time from apply_form_barcode_children where apply_id=@p_apply_id";
            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, id);

            var list = new List<PrintBarcodeModel>();
            using(var reader = db.ExecuteReader(cmd))
            {
                while(reader.Read())
                {
                    var child = new ApplyFormBarcodeChildEntity();
                    child.Init(reader);

                    List<ApplyFormBarcodeEntity> barcodeList;
                    GoodsSerialEntity serial;
                    
                    if(map.TryGetValue(child.Barcode, out barcodeList) && serials.TryGetValue(barcodeList[0].SerialId, out serial))
                    list.Add(new PrintBarcodeModel
                    {
                        Id = child.Id.ToString(),
                        Barcode = child.ChildBarcode,
                        ProductId = serial.ProductId,
                        VendorId = serial.VendorId,
                        ExpiredDate = child.ExpiredDate,
                        BatchNo = serial.BatchNo,
                        SerialNo = serial.SerialNo,
                        IsPrinted = child.IsPrinted
                    });
                }
            }

            return list;
        }

        public static IList<PrintBarcodeModel> SplitGoods(string id, string userId)
        {
            var barcodes = GetApplyBarcodes(id);
            var serials = GetApplyGoodsSerials(id, barcodes).ToDictionary(item => item.Id);

            var db = DatabaseFactory.CreateDatabase();

            var list = new List<PrintBarcodeModel>();

            try
            {
                foreach (var barcode in barcodes)
                {
                    GoodsSerialEntity serial;
                    if (serials.TryGetValue(barcode.SerialId, out serial))
                    {
                        var children = GeneralChildBarcodes(serial, barcode, userId, db);

                        list.AddRange(children.Select(child => new PrintBarcodeModel
                        {
                            Id = child.Id.ToString(),
                            Barcode = child.ChildBarcode,
                            ProductId = serial.ProductId,
                            VendorId = serial.VendorId,
                            ExpiredDate = child.ExpiredDate,
                            BatchNo = serial.BatchNo,
                            SerialNo = serial.SerialNo,
                            IsPrinted = child.IsPrinted
                        }).ToList());
                    }
                }
            }
            catch
            {
                DeleteBarcodeChildren(id, db);

                throw;
            }

            return list;
        }

        private static IList<ApplyFormBarcodeChildEntity> GeneralChildBarcodes(GoodsSerialEntity serial, ApplyFormBarcodeEntity originBarcode, string userId, Database db)
        {
            var list = new List<ApplyFormBarcodeChildEntity>();

            var totalCount = originBarcode.Count * serial.SplitCopies;
            var baseKey = IdentityCreatorRepository.Get(IdentityKey.GOODS_BARCODE, totalCount);
            for (var i = 0; i < totalCount; i++)
            {
                var child = new ApplyFormBarcodeChildEntity
                {
                    Id = Guid.NewGuid(),
                    ApplyId = originBarcode.ApplyId,
                    Barcode = originBarcode.Barcode,
                    ChildBarcode = StringHelper.Barcode(baseKey + i),
                    ExpiredDate = serial.ExpiredDate.Value.AddDays((double)serial.ValidDays),
                    IsPrinted = false,
                    CreatedId = userId,
                    CreatedTime = DateTime.Now
                };

                CreateChildBarcode(child, db);

                list.Add(child);
            }

            return list;
        }

        private static void CreateChildBarcode(ApplyFormBarcodeChildEntity child, Database db)
        {
            var sql = @"insert into apply_form_barcode_children(id,apply_id,barcode,child_barcode,expired_date,is_printed,created_id,created_time) 
values(@p_id,@p_apply_id,@p_barcode,@p_child_barcode,@p_expired_date,@p_is_printed,@p_created_id,@p_created_time)";

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.Guid, child.Id);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, child.ApplyId);
            db.AddInParameter(cmd, "p_barcode", DbType.String, child.Barcode);
            db.AddInParameter(cmd, "p_child_barcode", DbType.String, child.ChildBarcode);
            db.AddInParameter(cmd, "p_expired_date", DbType.DateTime, child.ExpiredDate);
            db.AddInParameter(cmd, "p_is_printed", DbType.Boolean, child.IsPrinted);
            db.AddInParameter(cmd, "p_created_id", DbType.String, child.CreatedId);
            db.AddInParameter(cmd, "p_created_time", DbType.DateTime, child.CreatedTime);

            db.ExecuteNonQuery(cmd);
        }

        private static void DeleteBarcodeChildren(string id, Database db)
        {
            var sql = "delete apply_form_barcode_children where apply_id=@p_apply_id";

            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_apply_id", DbType.String, id);

            db.ExecuteNonQuery(cmd);
        }

        public static void UpdatePrint(IList<string> ids)
        {
            if(ids == null || ids.Count == 0)
            {
                return;
            }

            IList<string> names;
            string namesSql;
            StringHelper.GenerInParameters("p_id", ids.Count, out names, out namesSql);

            var sql = string.Format("update apply_form_barcode_children set is_printed=@p_is_printed where id in ({0})", namesSql);
            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);
            
            db.AddInParameter(cmd, "p_is_printed", DbType.Boolean, true);
            for (var i = 0; i < ids.Count; i++)
            {
                db.AddInParameter(cmd, names[i], DbType.String, ids[i]);
            }

            db.ExecuteNonQuery(cmd);
        }
        #endregion

        private static string FormSerialKey(string id)
        {
            return string.Format("APPLY_FORM_{0}", id);
        }


        //        #region Save
        //        public static void Save(ApplyFormEntity entity)
        //        {
        //            var db = DatabaseFactory.CreateDatabase();

        //            using (var conn = db.CreateConnection())
        //            {
        //                conn.Open();
        //                using (var trans = conn.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        var productIds = entity.Details.Select(item => item.ProductId).ToList();
        //                        var inventoryList = GoodsInventoryRepository.Get(productIds, entity.HospitalId, db, trans);
        //                        var productInventory = inventoryList.ToDictionary(item => item.ProductId);

        //                        IList<ApplyFormDetailEntity> originDetails = null;
        //                        if (string.IsNullOrEmpty(entity.Id))
        //                        {
        //                            AddForm(entity, db, trans);
        //                        }
        //                        else
        //                        {
        //                            var form = Get(entity.Id, db, trans);
        //                            if (form.Status == (int)ApplyFormStatus.Granting || form.Status == (int)ApplyFormStatus.Granted)
        //                            {
        //                                throw new InvalidException("领用单已开始发放或发放完成,不能再进行编辑！");
        //                            }

        //                            UpdateForm(entity, db, trans);
        //                            if (form.Status == (int)ApplyFormStatus.Applied)
        //                            {
        //                                originDetails = GetDetails(entity.Id, db, trans);
        //                            }
        //                        }

        //                        IList<ApplyFormDetailEntity> lackDetails;
        //                        if (!HandleDetails(entity, productInventory, originDetails, db, trans, out lackDetails))
        //                        {
        //                            var message = new StringBuilder();
        //                            foreach (var item in lackDetails)
        //                            {
        //                                var inventory = productInventory[item.ProductId];
        //                                message.AppendFormat("产品{0}(库存:{1}),", item.ProductName, inventory.UsableCount - inventory.GrantedCount);
        //                            }
        //                            throw new InvalidException(string.Format("{0}已经不够申请！", message.ToString().TrimEnd(',')));
        //                        }

        //                        trans.Commit();
        //                    }
        //                    catch
        //                    {
        //                        trans.Rollback();
        //                        throw;
        //                    }
        //                }
        //            }
        //        }

        //        private static void AddForm(ApplyFormEntity entity, Database db, DbTransaction trans)
        //        {
        //            entity.Id = Guid.NewGuid().ToString();

        //            var sql = string.Format(@"insert into apply_form({0})
        //values(
        //@p_id, @p_form_no, @p_filter_id, @p_applyer, @p_apply_unit_id, @p_apply_date, @p_hospital_id, @p_status
        //)", COLUMN_SQL);

        //            var dc = db.GetSqlStringCommand(sql);
        //            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);
        //            db.AddInParameter(dc, "p_form_no", DbType.Int32, entity.FormNo);
        //            db.AddInParameter(dc, "p_filter_id", DbType.String, entity.FilterId);
        //            db.AddInParameter(dc, "p_applyer", DbType.String, entity.Applyer);
        //            db.AddInParameter(dc, "p_apply_unit_id", DbType.String, entity.ApplyUnitId);
        //            db.AddInParameter(dc, "p_apply_date", DbType.String, entity.ApplyDate);
        //            db.AddInParameter(dc, "p_hospital_id", DbType.String, entity.HospitalId);
        //            db.AddInParameter(dc, "p_status", DbType.Int32, entity.Status);

        //            db.ExecuteNonQuery(dc, trans);
        //        }

        //        private static void UpdateForm(ApplyFormEntity entity, Database db, DbTransaction trans)
        //        {
        //            var sql = @"update apply_form
        //set filter_id = @p_filter_id, applyer = @p_applyer, apply_unit_id = @p_apply_unit_id, status = @p_status
        //where id = @p_id";

        //            var dc = db.GetSqlStringCommand(sql);
        //            db.AddInParameter(dc, "p_id", DbType.String, entity.Id);
        //            db.AddInParameter(dc, "p_filter_id", DbType.String, entity.FilterId);
        //            db.AddInParameter(dc, "p_applyer", DbType.String, entity.Applyer);
        //            db.AddInParameter(dc, "p_apply_unit_id", DbType.String, entity.ApplyUnitId);
        //            db.AddInParameter(dc, "p_status", DbType.Int32, ApplyFormStatus.Applied);

        //            db.ExecuteNonQuery(dc, trans);
        //        }

        //        private static bool HandleDetails(ApplyFormEntity form, IDictionary<string, GoodsInventoryEntity> productInventory, IList<ApplyFormDetailEntity> originDetails,
        //            Database db, DbTransaction trans, out IList<ApplyFormDetailEntity> lackDetails)
        //        {
        //            GoodsInventoryEntity inventory;
        //            var notAppliedList = new List<ApplyFormDetailEntity>();
        //            if (originDetails != null && originDetails.Count > 0)
        //            {
        //                foreach (var item in originDetails)
        //                {
        //                    if (!productInventory.TryGetValue(item.ProductId, out inventory))
        //                    {
        //                        notAppliedList.Add(item);
        //                    }
        //                    else
        //                    {
        //                        inventory.GrantedCount -= item.ApplyCount;
        //                    }
        //                }
        //            }

        //            lackDetails = new List<ApplyFormDetailEntity>();
        //            foreach (var item in form.Details)
        //            {
        //                if (!productInventory.TryGetValue(item.ProductId, out inventory))
        //                {
        //                    throw new InvalidException(string.Format("{0}发生库存不存在异常，请联系统管理员！", item.ProductName));
        //                }

        //                if (item.ApplyCount + inventory.GrantedCount > inventory.UsableCount)
        //                {
        //                    lackDetails.Add(item);
        //                }
        //            }

        //            if (lackDetails.Count == 0)
        //            {
        //                DeleteDetails(form.Id, db, trans);
        //                if (originDetails != null && originDetails.Count > 0)
        //                {
        //                    foreach (var item in originDetails)
        //                    {
        //                        GoodsInventoryRepository.ReduceGrantedCount(item.ProductId, form.HospitalId, item.ApplyCount, db, trans);
        //                    }
        //                }

        //                SaveDetails(form.Id, form.FormNo, form.HospitalId, form.Details, db, trans);
        //                return true;
        //            }
        //            return false;
        //        }

        //        private static void DeleteDetails(string applyId, Database db, DbTransaction trans)
        //        {
        //            var sql = "delete apply_form_detail where apply_id = @p_apply_id";

        //            var dc = db.GetSqlStringCommand(sql);
        //            db.AddInParameter(dc, "p_apply_id", DbType.String, applyId);

        //            db.ExecuteNonQuery(dc, trans);
        //        }

        //        private static void SaveDetails(string applyId, int formNo, string hospitalId, IList<ApplyFormDetailEntity> details, Database db, DbTransaction trans)
        //        {
        //            var sql = string.Format(@"insert into apply_form_detail({0})
        //values(@p_id, @p_apply_id, @p_form_no, @p_product_id, @p_apply_count, 0)", DETAIL_COLUMN_ID);

        //            foreach (var detail in details)
        //            {
        //                detail.Id = Guid.NewGuid().ToString();
        //                detail.ApplyId = applyId;
        //                detail.FormNo = formNo;

        //                var dc = db.GetSqlStringCommand(sql);
        //                db.AddInParameter(dc, "p_id", DbType.String, detail.Id);
        //                db.AddInParameter(dc, "p_apply_id", DbType.String, detail.ApplyId);
        //                db.AddInParameter(dc, "p_form_no", DbType.Int32, detail.FormNo);
        //                db.AddInParameter(dc, "p_product_id", DbType.String, detail.ProductId);
        //                db.AddInParameter(dc, "p_apply_count", DbType.Int32, detail.ApplyCount);

        //                db.ExecuteNonQuery(dc, trans);

        //                GoodsInventoryRepository.AddGrantedCount(detail.ProductId, hospitalId, detail.ApplyCount, db, trans);
        //            }
        //        }
        //        #endregion

        //        #region Get

        //public static ApplyFormEntity Get(string id, bool includeDetail = true)
        //{
        //    var db = DatabaseFactory.CreateDatabase();

        //    var entity = Get(id, db, null);
        //    if (entity == null)
        //    {
        //        return null;
        //    }

        //    //if (includeDetail)
        //    //{
        //    //    entity.Details = GetDetails(id, db, null);
        //    //}

        //    return entity;
        //}

        //private static ApplyFormEntity Get(string id, Database db, DbTransaction trans)
        //{
        //    var sql = string.Format("select {0} from apply_form {1} where id = @p_id", COLUMN_SQL, TransHelper.UpdateLock(trans));

        //    var dc = db.GetSqlStringCommand(sql);
        //    db.AddInParameter(dc, "p_id", DbType.String, id);

        //    var reader = trans == null ? db.ExecuteReader(dc) : db.ExecuteReader(dc, trans);
        //    try
        //    {
        //        if (reader.Read())
        //        {
        //            var entity = new ApplyFormEntity();
        //            entity.Init(reader);

        //            return entity;
        //        }
        //    }
        //    finally
        //    {
        //        reader.Close();
        //    }

        //    return null;
        //}

        //        private static IList<ApplyFormDetailEntity> GetDetails(string applyId, Database db, DbTransaction trans)
        //        {
        //            var sql = string.Format("select {0} from apply_form_detail {1} where apply_id = @p_apply_id", DETAIL_COLUMN_ID, TransHelper.UpdateLock(trans));

        //            var dc = db.GetSqlStringCommand(sql);
        //            db.AddInParameter(dc, "p_apply_id", DbType.String, applyId);

        //            var list = new List<ApplyFormDetailEntity>();
        //            var reader = trans == null ? db.ExecuteReader(dc) : db.ExecuteReader(dc, trans);
        //            try
        //            {
        //                while (reader.Read())
        //                {
        //                    var entity = new ApplyFormDetailEntity();
        //                    entity.Init(reader);

        //                    list.Add(entity);
        //                }
        //            }
        //            finally
        //            {
        //                reader.Close();
        //            }

        //            return list;
        //        }

        //        private static ApplyFormDetailEntity GetDetail(string id, Database db, DbTransaction trans)
        //        {
        //            var sql = string.Format("select {0} from apply_form_detail {1} where id = @p_id", DETAIL_COLUMN_ID, TransHelper.UpdateLock(trans));

        //            var dc = db.GetSqlStringCommand(sql);
        //            db.AddInParameter(dc, "p_id", DbType.String, id);

        //            var reader = trans == null ? db.ExecuteReader(dc) : db.ExecuteReader(dc, trans);
        //            try
        //            {
        //                if (reader.Read())
        //                {
        //                    var entity = new ApplyFormDetailEntity();
        //                    entity.Init(reader);

        //                    return entity;
        //                }
        //            }
        //            finally
        //            {
        //                reader.Close();
        //            }

        //            return null;
        //        }

        //        public static ApplyFormDetailEntity GetDetail(string applyId, string productId)
        //        {
        //            var sql = string.Format("select {0} from apply_form_detail where apply_id = @p_apply_id and product_id = @p_product_id", DETAIL_COLUMN_ID);

        //            var db = DatabaseFactory.CreateDatabase();
        //            var dc = db.GetSqlStringCommand(sql);
        //            db.AddInParameter(dc, "p_apply_id", DbType.String, applyId);
        //            db.AddInParameter(dc, "p_product_id", DbType.String, productId);

        //            using (var reader = db.ExecuteReader(dc))
        //            {
        //                if (reader.Read())
        //                {
        //                    var entity = new ApplyFormDetailEntity();
        //                    entity.Init(reader);

        //                    return entity;
        //                }
        //            }

        //            return null;
        //        }
        //        #endregion

        //        #region Cancel
        //        public static bool Cancel(string id)
        //        {
        //            bool canceled = false;

        //            var db = DatabaseFactory.CreateDatabase();
        //            using (var conn = db.CreateConnection())
        //            {
        //                conn.Open();
        //                using (var trans = conn.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        var form = Get(id, db, trans);
        //                        if (form == null)
        //                        {
        //                            throw new Exception("The apply form has not existed.");
        //                        }

        //                        if (form.Status == (int)ApplyFormStatus.Applied)
        //                        {
        //                            var details = GetDetails(id, db, trans);

        //                            Cancel(id, db, trans);
        //                            HandleDetails(form, details, db, trans);

        //                            canceled = true;
        //                        }

        //                        trans.Commit();
        //                    }
        //                    catch
        //                    {
        //                        trans.Rollback();
        //                        throw;
        //                    }
        //                }
        //            }

        //            return canceled;
        //        }

        //        private static void Cancel(string id, Database db, DbTransaction trans)
        //        {
        //            var sql = "update apply_form set status = @p_status where id = @p_id";

        //            var dc = db.GetSqlStringCommand(sql);
        //            db.AddInParameter(dc, "p_id", DbType.String, id);
        //            db.AddInParameter(dc, "p_status", DbType.Int32, ApplyFormStatus.Cancel);

        //            db.ExecuteNonQuery(dc, trans);
        //        }

        //        private static void HandleDetails(ApplyFormEntity form, IList<ApplyFormDetailEntity> details, Database db, DbTransaction trans)
        //        {
        //            foreach (var item in details)
        //            {
        //                GoodsInventoryRepository.ReduceGrantedCount(item.ProductId, form.HospitalId, item.ApplyCount, db, trans);
        //            }
        //        }
        //        #endregion

        //        #region Granting
        //        public static void Granting(string id)
        //        {
        //            var db = DatabaseFactory.CreateDatabase();
        //            using (var conn = db.CreateConnection())
        //            {
        //                conn.Open();
        //                using (var trans = conn.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        var form = Get(id, db, trans);
        //                        if (form == null)
        //                        {
        //                            throw new Exception("The apply form has not existed.");
        //                        }

        //                        if (form.Status == (int)ApplyFormStatus.Cancel)
        //                        {
        //                            throw new InvalidException("领用单已取消！");
        //                        }

        //                        if (form.Status == (int)ApplyFormStatus.Granted)
        //                        {
        //                            throw new InvalidException("领用单已发放完成！");
        //                        }

        //                        Granting(id, db, trans);

        //                        trans.Commit();
        //                    }
        //                    catch
        //                    {
        //                        trans.Rollback();
        //                        throw;
        //                    }
        //                }
        //            }
        //        }

        //        private static bool Granting(string id, Database db, DbTransaction trans)
        //        {
        //            var sql = "update apply_form set status = @p_status where id = @p_id";

        //            var dc = db.GetSqlStringCommand(sql);

        //            db.AddInParameter(dc, "p_id", DbType.String, id);
        //            db.AddInParameter(dc, "p_status", DbType.Int32, ApplyFormStatus.Granting);

        //            return db.ExecuteNonQuery(dc, trans) > 0;
        //        }
        //        #endregion

        //        #region Grant Scan

        //        public static void GrantScan(string barcode, ApplyFormDetailEntity formDetail, string userId)
        //        {
        //            var db = DatabaseFactory.CreateDatabase();

        //            using (var conn = db.CreateConnection())
        //            {
        //                conn.Open();
        //                using (var trans = conn.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        var detail = GetDetail(formDetail.Id, db, trans);
        //                        if (detail.GrantCount >= detail.ApplyCount)
        //                        {
        //                            throw new InvalidException("领用单扫描的该产品数量已达到领用数量！");
        //                        }

        //                        GrantScan(formDetail.Id, db, trans);
        //                        GoodsStateRepository.SetState(barcode, FormType.Apply, formDetail.ApplyId, userId, db, trans);

        //                        trans.Commit();
        //                    }
        //                    catch
        //                    {
        //                        trans.Rollback();
        //                        throw;
        //                    }
        //                }
        //            }
        //        }

        //        private static void GrantScan(string id, Database db, DbTransaction trans)
        //        {
        //            var sql = "update apply_form_detail set grant_count += 1 where id = @p_id";

        //            var dc = db.GetSqlStringCommand(sql);
        //            db.AddInParameter(dc, "p_id", DbType.String, id);

        //            db.ExecuteNonQuery(dc, trans);
        //        }
        //        #endregion


        //        public static void ConfirmGrant(string id, string hospitalId)
        //        {
        //            var db = DatabaseFactory.CreateDatabase();
        //            using (var conn = db.CreateConnection())
        //            {
        //                conn.Open();
        //                using (var trans = conn.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        if(ConfirmGrant(id, hospitalId, db, trans))
        //                        {
        //                            AdjustGrantCount(id, hospitalId, db, trans);

        //                            GoodsRepsitory.CompleteByApplyForm(id, db, trans);
        //                        }

        //                        trans.Commit();
        //                    }
        //                    catch
        //                    {
        //                        trans.Rollback();
        //                        throw;
        //                    }
        //                }
        //            }
        //        }

        //        private static bool ConfirmGrant(string id, string hospitalId, Database db, DbTransaction trans)
        //        {
        //            var sql = "update apply_form set status = @p_status where id = @p_id and status=@p_granting and hospital_id=@p_hospital_id";

        //            var dc = db.GetSqlStringCommand(sql);

        //            db.AddInParameter(dc, "p_status", DbType.Int32, ApplyFormStatus.Granted);
        //            db.AddInParameter(dc, "p_granting", DbType.Int32, ApplyFormStatus.Granting);
        //            db.AddInParameter(dc, "p_id", DbType.String, id);
        //            db.AddInParameter(dc, "p_hospital_id", DbType.String, hospitalId);

        //            return db.ExecuteNonQuery(dc, trans) > 0;
        //        }

        //        private static void AdjustGrantCount(string id, string hospitalId, Database db, DbTransaction trans)
        //        {
        //            var details = GetDetails(id, db, trans);

        //            foreach(var detail in details)
        //            {
        //                if(detail.ApplyCount > detail.GrantCount)
        //                {
        //                    GoodsInventoryRepository.ReduceGrantedCount(detail.ProductId, hospitalId, detail.ApplyCount - detail.GrantCount, db, trans);
        //                }
        //            }
        //        }
    }
}
