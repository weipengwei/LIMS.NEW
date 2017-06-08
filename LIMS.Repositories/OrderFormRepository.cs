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
    public static class OrderFormRepository
    {
        private const string COLUMN_SQL = @"id,filler_id,apply_time,form_no,hospital_id,apply_unit_id,vendor_id,
receipt_id, status,created_id, created_time, updated_id, updated_time";
        private const string ITEM_COLUMN_SQL = @"
id,order_id,form_no,hospital_id,product_id,order_person,count,donate_count,
price,expect_price,sum,expired_date,expect_date,contact,status,
need_audit,need_check,need_split,split_copies,split_unit,split_capacity,split_package_count,valid_days";
        private const string VENDOR_NAME_SQL = "isnull((select top 1 units.[name] from units where units.[id] = order_form.vendor_id),'') as vendor_name";

        #region Query Order Form
        public static IList<OrderFormEntity> Query(DateRangeCondition condition, PagerInfo pager)
        {
            pager.ComputePageCount(QueryCount(condition));

            var list = new List<OrderFormEntity>();


            var orderSql = " ORDER BY ";
            if (pager.OrderFields.Count > 0)
            {
                int index = 0;
                foreach (var field in pager.OrderFields)
                {
                    orderSql += (index == 0 ? string.Empty : ",") + field.Field + (field.Desc ? " DESC" : "");
                    index++;
                }
            }
            else
            {
                orderSql += "form_no DESC";
            }

            var sql = string.Format(@"SELECT {0},{2} FROM order_form WHERE {1}", COLUMN_SQL, GetConditionSql(condition), VENDOR_NAME_SQL);

            sql = @"SELECT * FROM
            (
                SELECT ROW_NUMBER() OVER(" + orderSql + @") pid," + COLUMN_SQL + ",vendor_name " + @"
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
                    var entity = new OrderFormEntity();
                    entity.Init(reader);
                    entity.VendorName = reader["vendor_name"].ToString();

                    list.Add(entity);
                }
            }

            return list;
        }

        private static int QueryCount(DateRangeCondition condition)
        {
            var sql = "SELECT COUNT(id) FROM order_form WHERE ";

            var conditionSql = GetConditionSql(condition);
            if (!string.IsNullOrEmpty(conditionSql))
            {
                sql += conditionSql;
            }

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            AddParameter(dc, db, condition);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                reader.Read();

                return reader[0] == DBNull.Value ? 0 : reader.GetInt32(0);
            }
        }

        private static void AddParameter(DbCommand dc, Database db, DateRangeCondition condition)
        {
            db.AddInParameter(dc, "p_user_id", DbType.String, condition.UserId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, condition.HospitalId);
            db.AddInParameter(dc, "p_deleted_status", DbType.String, OrderFormStatus.Deleted);

            int formNo;
            if (!string.IsNullOrEmpty(condition.Content) && int.TryParse(condition.Content, out formNo))
            {
                db.AddInParameter(dc, "p_form_no", DbType.Int32, formNo);
            }

            if (condition.BeginDate.HasValue)
            {
                db.AddInParameter(dc, "p_begin_date", DbType.DateTime, condition.BeginDate);
            }

            if (condition.EndDate.HasValue)
            {
                db.AddInParameter(dc, "p_end_date", DbType.DateTime, condition.EndDate);
            }

            if (condition.AdditionalFilters != null)
            {
                if (condition.AdditionalFilters.ContainsKey("VendorName")
                && !string.IsNullOrEmpty(condition.AdditionalFilters["VendorName"].ToString()))
                    db.AddInParameter(dc, "p_vendor_name", DbType.String, "%" + condition.AdditionalFilters["VendorName"].ToString() + "%");

                if (condition.AdditionalFilters.ContainsKey("ProductName")
                && !string.IsNullOrEmpty(condition.AdditionalFilters["ProductName"].ToString()))
                    db.AddInParameter(dc, "p_product_name", DbType.String, "%" + condition.AdditionalFilters["ProductName"].ToString() + "%");
            }
        }

        private static string GetConditionSql(DateRangeCondition condition)
        {
            var conditionSql = " 1=1 and status <> @p_deleted_status and filler_id=@p_user_id";
            conditionSql += " AND apply_unit_id IN (select unit_id from user_privilege where user_id = @p_user_id AND unit_root_id=@p_hospital_id and operate=1)";

            int formNo;
            if (!string.IsNullOrEmpty(condition.Content) && int.TryParse(condition.Content, out formNo))
            {
                conditionSql += " AND form_no = @p_form_no";
            }

            if (condition.BeginDate.HasValue)
            {
                conditionSql += " AND apply_time >= @p_begin_date";
            }

            if (condition.EndDate.HasValue)
            {
                conditionSql += " AND apply_time <= @p_end_date";
            }

            if (condition.AdditionalFilters != null)
            {
                if(condition.AdditionalFilters.ContainsKey("UncompletedOnly")
                && (bool)condition.AdditionalFilters["UncompletedOnly"])
                    conditionSql += " AND status <> '" + OrderFormStatus.Complete + "' ";

                if (condition.AdditionalFilters.ContainsKey("CompletedOnly")
                && (bool)condition.AdditionalFilters["CompletedOnly"])
                    conditionSql += " AND status = '" + OrderFormStatus.Complete + "' ";

                if (condition.AdditionalFilters.ContainsKey("VendorName")
                && !string.IsNullOrEmpty(condition.AdditionalFilters["VendorName"].ToString()))
                    conditionSql += " AND vendor_id IN (select [id] from units where [name] like @p_vendor_name)";

                if (condition.AdditionalFilters.ContainsKey("ProductName")
                && !string.IsNullOrEmpty(condition.AdditionalFilters["ProductName"].ToString()))
                    conditionSql += " AND exists(select 1 from order_form_items item where item.order_id = order_form.id and item.product_id in (select [id] from products where [name] like @p_product_name))";
            }

            return conditionSql;
        }

        public static IList<OrderFormEntity> Query(IList<string> ids)
        {
            var list = new List<OrderFormEntity>();
            if(ids.Count == 0)
            {
                return list;
            }

            IList<string> names;
            string namesSql;
            StringHelper.GenerInParameters("p_id_", ids.Count, out names, out namesSql);

            var sql = string.Format("select {0} from order_form where 1=1 and id in ({1}) and status <> @p_deleted_status", COLUMN_SQL, namesSql);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            
            db.AddInParameter(dc, "p_deleted_status", DbType.String, OrderFormStatus.Deleted);
            for(var i =0; i < ids.Count; i++)
            {
                db.AddInParameter(dc, names[i], DbType.String, ids[i]);
            }

            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new OrderFormEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }
        #endregion

        #region Query Order Form Items
        public static IList<OrderQueryDetailEntity> QueryItems(OrderQueryCondition condition, PagerInfo pager)
        {
            pager.ComputePageCount(QueryItemCount(condition));

            var list = new List<OrderQueryDetailEntity>();


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
                orderSql += "form_no DESC";
            }

            var sql = string.Format(@"SELECT 
        c.name AS unit_name, d.name product_name,
        a.id, order_id, a.form_no, a.product_id, a.hospital_id, a.need_check, a.need_split, 
        a.valid_days, a.contact, a.price, a.count, a.donate_count, a.sum, a.expired_date, 
        a.expect_date, a.order_person, a.status, a.need_audit, a.expect_price,
        b.created_time as registed_time, a.split_copies, a.split_unit,a.split_capacity,a.split_package_count
        FROM order_form_items a 
        JOIN order_form b ON a.order_id = b.id and b.status <> @p_deleted_status
        JOIN units c ON b.apply_unit_id = c.id
        JOIN products d ON a.product_id = d.id
        WHERE {1}", ITEM_COLUMN_SQL, GetItemConditionSql(condition));

            sql = @"SELECT * FROM
                    (
                        SELECT ROW_NUMBER() OVER(" + orderSql + @") pid,*
                        FROM (" + sql + @") t            
                    ) t1 WHERE t1.pid BETWEEN @p_pageNo * @p_pageSize + 1 AND (@p_pageNo + 1) * @p_pageSize ";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            AddItemParameter(dc, db, condition);
            db.AddInParameter(dc, "p_pageNo", DbType.Int32, pager.PageIndex);
            db.AddInParameter(dc, "p_pageSize", DbType.Int32, pager.PageSize);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new OrderQueryDetailEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        private static int QueryItemCount(OrderQueryCondition condition)
        {
            var sql = "SELECT COUNT(*) FROM order_form_items a JOIN order_form b ON a.order_id = b.id and b.status <> @p_deleted_status WHERE ";

            var conditionSql = GetItemConditionSql(condition);
            if (!string.IsNullOrEmpty(conditionSql))
            {
                sql += conditionSql;
            }

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            AddItemParameter(dc, db, condition);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                reader.Read();

                return reader.GetInt32(0);
            }
        }

        private static string GetItemConditionSql(OrderQueryCondition condition)
        {
            var conditionSql = " 1=1 ";
            if (!string.IsNullOrEmpty(condition.UserId))
            {
                conditionSql += @" AND a.product_id IN (select b.product_id from user_privilege a 
                            JOIN vendor_products b ON a.unit_id = b.unit_id where user_id = @p_user_id and operate=1)";
            }

            conditionSql += " AND b.hospital_id=@p_hospital_id";
            conditionSql += " AND b.vendor_id = @p_vendor_id";

            int formNo;
            if (!string.IsNullOrEmpty(condition.Content) && int.TryParse(condition.Content, out formNo))
            {
                conditionSql += " AND b.form_no = @p_form_no";
            }

            if (condition.BeginDate.HasValue)
            {
                conditionSql += " AND b.created_time >= @p_begin_date";
            }

            if (condition.EndDate.HasValue)
            {
                conditionSql += " AND b.created_time <= @p_end_date";
            }

            if (condition.Status.Count > 0)
            {
                conditionSql += string.Format(" AND a.status IN ('{0}')", string.Join("','", condition.Status));
            }

            return conditionSql;
        }

        private static void AddItemParameter(DbCommand dc, Database db, OrderQueryCondition condition)
        {
            if (!string.IsNullOrEmpty(condition.UserId))
            {
                db.AddInParameter(dc, "p_user_id", DbType.String, condition.UserId);
            }

            db.AddInParameter(dc, "p_hospital_id", DbType.String, condition.HospitalId);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, condition.VendorId);
            db.AddInParameter(dc, "p_deleted_status", DbType.String, OrderFormStatus.Deleted);

            int formNo;
            if (!string.IsNullOrEmpty(condition.Content) && int.TryParse(condition.Content, out formNo))
            {
                db.AddInParameter(dc, "p_form_no", DbType.Int32, formNo);
            }

            if (condition.BeginDate.HasValue)
            {
                db.AddInParameter(dc, "p_begin_date", DbType.DateTime, condition.BeginDate);
            }

            if (condition.EndDate.HasValue)
            {
                db.AddInParameter(dc, "p_end_date", DbType.DateTime, condition.EndDate);
            }
        }


        public static IList<OrderFormItemEntity> QueryItems(IList<string> itemIds)
        {
            var list = new List<OrderFormItemEntity>();
            if(itemIds.Count == 0)
            {
                return list;
            }

            IList<string> names;
            string namesSql;
            StringHelper.GenerInParameters("p_id_", itemIds.Count, out names, out namesSql);

            var sql = string.Format(@"select {0} from order_form_items where 1=1 and id in ({1}) order by form_no desc", ITEM_COLUMN_SQL, namesSql);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            for(var i = 0; i < itemIds.Count; i ++)
            {
                db.AddInParameter(dc, names[i], DbType.String, itemIds[i]);
            }

            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new OrderFormItemEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }
        #endregion

        #region Get
        public static OrderFormEntity Get(string id)
        {
            var db = DatabaseFactory.CreateDatabase();

            var sql = string.Format("SELECT {0} FROM order_form WHERE id=@p_id", COLUMN_SQL);
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);

            using (var reader = db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var entity = new OrderFormEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static IList<OrderFormItemEntity> GetItems(string orderId)
        {
            var sql = string.Format("SELECT {0} FROM order_form_items WHERE order_id = @p_order_id order by created_time", ITEM_COLUMN_SQL);
            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_order_id", DbType.String, orderId);

            var list = new List<OrderFormItemEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var item = new OrderFormItemEntity();
                    item.Init(reader);

                    list.Add(item);
                }
            }

            return list;
        }

        public static IList<OrderFormItemEntity> GetItems(int formNo)
        {
            var sql = string.Format("SELECT {0} FROM order_form_items WHERE form_no = @p_form_no order by created_time", ITEM_COLUMN_SQL);
            var db = DatabaseFactory.CreateDatabase();

            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_form_no", DbType.Int32, formNo);

            var list = new List<OrderFormItemEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var item = new OrderFormItemEntity();
                    item.Init(reader);

                    list.Add(item);
                }
            }

            return list;
        }

        public static OrderFormItemEntity GetItem(string id)
        {
            var sql = string.Format("SELECT {0} FROM order_form_items WHERE id = @p_id", ITEM_COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);

            using (var reader = db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var entity = new OrderFormItemEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }
        
        public static IList<OrderFormDonateItemEntity> GetDonateItems(string itemId)
        {
            var list = new List<OrderFormDonateItemEntity>();
            
            var sql = "select id,order_id,item_id,hospital_id,product_id,count,created_id,created_time from order_form_donate_items where item_id=@p_item_id";

            var db = DatabaseFactory.CreateDatabase();
            var cmd = db.GetSqlStringCommand(sql);

            db.AddInParameter(cmd, "p_item_id", DbType.String, itemId);
            
            using (var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new OrderFormDonateItemEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<OrderFormDonateItemEntity> GetDonateItems(string orderId, IList<string> products)
        {
            var list = new List<OrderFormDonateItemEntity>();

            var db = DatabaseFactory.CreateDatabase();

            string sql;
            DbCommand cmd;
            if(products == null || products.Count == 0)
            {
                sql = "select id,order_id,item_id,hospital_id,product_id,count,created_id,created_time from order_form_donate_items where order_id=@p_order_id";
                cmd = db.GetSqlStringCommand(sql);
                db.AddInParameter(cmd, "p_order_id", DbType.String, orderId);
            }
            else
            {
                IList<string> names;
                string namesSql;
                StringHelper.GenerInParameters("p_product_id", products.Count, out names, out namesSql);

                sql = string.Format("select id,order_id,item_id,hospital_id,product_id,count,created_id,created_time from order_form_donate_items where order_id=@p_order_id and product_id in ({0})", namesSql);
                cmd = db.GetSqlStringCommand(sql);
                db.AddInParameter(cmd, "p_order_id", DbType.String, orderId);
                for (var i = 0; i < products.Count; i++)
                {
                    db.AddInParameter(cmd, names[i], DbType.String, products[i]);
                }
            }
            
            using(var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new OrderFormDonateItemEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public static IList<OrderFormDonationEntity> GetDonations(string orderId, IList<string> products)
        {
            var list = new List<OrderFormDonationEntity>();
            
            var db = DatabaseFactory.CreateDatabase();

            string sql;
            DbCommand cmd;
            if (products == null || products.Count == 0)
            {
                sql = "select id,order_id,item_id,hospital_id,vendor_id,product_id,base_count,donate_count,created_id,created_time from order_form_donations where order_id=@p_order_id";
                cmd = db.GetSqlStringCommand(sql);
                db.AddInParameter(cmd, "p_order_id", DbType.String, orderId);
            }
            else
            {
                IList<string> names;
                string namesSql;
                StringHelper.GenerInParameters("p_product_id", products.Count, out names, out namesSql);

                sql = string.Format("select id,order_id,item_id,hospital_id,vendor_id,product_id,base_count,donate_count,created_id,created_time from order_form_donations where order_id=@p_order_id and product_id in ({0})", namesSql);
                cmd = db.GetSqlStringCommand(sql);
                db.AddInParameter(cmd, "p_order_id", DbType.String, orderId);
                for (var i = 0; i < products.Count; i++)
                {
                    db.AddInParameter(cmd, names[i], DbType.String, products[i]);
                }
            }

            using (var reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var entity = new OrderFormDonationEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }
        #endregion

        #region Save Order Form
        public static void Create(OrderFormEntity order)
        {
            var sql = string.Format(@"insert into order_form({0}) values(
@p_id, @p_filler_id, @p_apply_time, @p_form_no, @p_hospital_id, @p_apply_unit_id,@p_vendor_id, 
@p_receipt_id, @p_status, @p_created_id, @p_created_time, @p_updated_id, @p_updated_time)", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            order.Id = Guid.NewGuid().ToString();

            db.AddInParameter(dc, "p_id", DbType.String, order.Id);
            db.AddInParameter(dc, "p_filler_id", DbType.String, order.FillerId);
            db.AddInParameter(dc, "p_apply_time", DbType.DateTime, order.ApplyTime);
            db.AddInParameter(dc, "p_form_no", DbType.Int32, order.FormNo);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, order.HospitalId);
            db.AddInParameter(dc, "p_apply_unit_id", DbType.String, order.ApplyUnitId);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, order.VendorId);
            db.AddInParameter(dc, "p_receipt_id", DbType.String, order.ReceiptId);
            db.AddInParameter(dc, "p_status", DbType.String, order.Status);
            db.AddInParameter(dc, "p_created_id", DbType.String, order.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, order.CreatedTime);
            db.AddInParameter(dc, "p_updated_id", DbType.String, order.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, order.UpdatedTime);

            db.ExecuteNonQuery(dc);
        }

        public static void Update(OrderFormEntity order)
        {
            var sql = @"UPDATE order_form 
SET apply_unit_id=@p_apply_unit_id, vendor_id=@p_vendor_id, receipt_id=@p_receipt_id, 
status=@p_status, updated_id=@p_updated_id, updated_time=@p_updated_time WHERE id=@p_id";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_id", DbType.String, order.Id);
            db.AddInParameter(dc, "p_apply_unit_id", DbType.String, order.ApplyUnitId);
            db.AddInParameter(dc, "p_vendor_id", DbType.String, order.VendorId);
            db.AddInParameter(dc, "p_receipt_id", DbType.String, order.ReceiptId);
            db.AddInParameter(dc, "p_status", DbType.String, order.Status);
            db.AddInParameter(dc, "p_updated_id", DbType.String, order.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, order.UpdatedTime);

            db.ExecuteNonQuery(dc);
        }
        #endregion

        #region Save Item and DonateItems
        public static void SaveItem(OrderFormItemEntity hostItem, IList<OrderFormDonateItemEntity> donateItems, string userId)
        {
            var form = Get(hostItem.OrderId);
            var savedItems = GetItems(hostItem.OrderId);

            var productSetting = HospitalProductRepository.GetByUnit(form.ApplyUnitId);
            
            MergeItem(form, hostItem, savedItems, productSetting);

            DonateProductEntity productDonation;
            IList<DonateProductItemEntity> productDonateItems;
            DonateProductRepository.Get(form.HospitalId, form.ApplyUnitId, form.VendorId, hostItem.ProductId, out productDonation, out productDonateItems);

            var filterDonateItems = FilterDonateItems(form, hostItem, donateItems, userId, productDonation, productDonateItems);

            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(hostItem.Id))
                        {
                            CreateItem(hostItem, db, trans);
                        }
                        else
                        {
                            UpdateItem(hostItem, db, trans);
                        }

                        CreateItems(form, savedItems, filterDonateItems, productSetting, db, trans);

                        CreateDonateItems(hostItem, filterDonateItems, db, trans);

                        var formDonations = GetDonations(form, hostItem, userId, productDonation, productDonateItems);
                        DeleteDonations(hostItem.Id, db, trans);
                        CreateDonations(formDonations, db, trans);

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

        private static void MergeItem(OrderFormEntity form, OrderFormItemEntity hostItem, IList<OrderFormItemEntity> savedItems, IList<HospitalProductEntity> productsSetting)
        {
            hostItem.FormNo = form.FormNo;
            hostItem.HospitalId = form.HospitalId;
            hostItem.NeedAudit = GetAuditConfig(form, hostItem.ProductId);
            hostItem.Status = OrderFormItemStatus.Saved;
            hostItem.Sum = hostItem.Price * hostItem.Count;

            var setting = productsSetting == null ? null : productsSetting.FirstOrDefault(item => string.Compare(item.ProductId, hostItem.ProductId, true) == 0);
            if (setting != null)
            {
                hostItem.NeedCheck = setting.NeedCheck;
                hostItem.NeedSplit = setting.NeedSplit;
                hostItem.SplitCopies = setting.SplitCopies;
                hostItem.SplitUnit = setting.SplitUnit;
                hostItem.ValidDays = setting.ValidDays;
                hostItem.SplitCapacity = setting.SplitCapacity;
                hostItem.SplitPackageCount = setting.MiniSplitNumber;
            }
            
            if(savedItems.Count == 0)
            {
                savedItems.Add(hostItem);
                return;
            }

            var savedItem = savedItems.FirstOrDefault(item => string.Compare(hostItem.ProductId, item.ProductId, true) == 0);
            if(savedItem == null)
            {
                savedItems.Add(hostItem);
                return;
            }
            savedItems.Remove(savedItem);

            hostItem.Id = savedItem.Id;
            hostItem.Price = savedItem.Price;
            hostItem.Count += savedItem.Count;
            hostItem.DonateCount = savedItem.DonateCount;

            savedItems.Add(hostItem);

            hostItem.Sum = hostItem.Price * hostItem.Count;
        }

        private static bool GetAuditConfig(OrderFormEntity form, string productId)
        {
            var auditingList = AuditingProductRepository.Query(form.HospitalId, form.VendorId, form.ApplyTime).ToDictionary(item => item.ProductId);
            return auditingList.Keys.Contains(productId) ? auditingList[productId].IsAudit : false;
        }

        private static IList<OrderFormDonateItemEntity> FilterDonateItems(OrderFormEntity form, OrderFormItemEntity hostItem, IList<OrderFormDonateItemEntity> donateItems, string userId, 
            DonateProductEntity productDonation, IList<DonateProductItemEntity> productDonateItems)
        {
            var filterDonateItems = new List<OrderFormDonateItemEntity>();

            if (donateItems == null || donateItems.Count == 0)
            {
                return filterDonateItems;
            }
            
            if(productDonation == null || productDonateItems == null || productDonateItems.Count == 0)
            {
                var self = donateItems.FirstOrDefault(item => string.Compare(hostItem.ProductId, item.ProductId, true) == 0);
                if(self != null)
                {
                    self.OrderId = form.Id;
                    self.HospitalId = form.HospitalId;
                    self.CreatedId = userId;
                    self.CreatedTime = DateTime.Now;

                    filterDonateItems.Add(self);
                }

                return filterDonateItems;
            }

            var dic = productDonateItems.ToDictionary(item => item.ProductId);

            DonateProductItemEntity productDonateItem;
            foreach(var item in donateItems)
            {
                if (dic.TryGetValue(item.ProductId, out productDonateItem))
                {
                    item.OrderId = form.Id;
                    item.HospitalId = form.HospitalId;
                    item.CreatedId = userId;
                    item.CreatedTime = DateTime.Now;

                    filterDonateItems.Add(item);
                }
                else if(string.Compare(item.ProductId, hostItem.ProductId, true) == 0)
                {
                    item.OrderId = form.Id;
                    item.HospitalId = form.HospitalId;
                    item.CreatedId = userId;
                    item.CreatedTime = DateTime.Now;

                    filterDonateItems.Add(item);
                }
            }

            return filterDonateItems;
        }

        private static IList<OrderFormDonationEntity> GetDonations(OrderFormEntity form, OrderFormItemEntity hostItem, string userId,
            DonateProductEntity productDonation, IList<DonateProductItemEntity> productDonateItems)
        {
            var list = new List<OrderFormDonationEntity>();
            if (productDonation == null || productDonateItems == null || productDonateItems.Count == 0)
            {
                return list;
            }

            foreach (var item in productDonateItems)
            {
                var formDonation = new OrderFormDonationEntity();

                formDonation.OrderId = form.Id;
                formDonation.ItemId = hostItem.Id;
                formDonation.HospitalId = form.HospitalId;
                formDonation.VendorId = form.VendorId;
                formDonation.ProductId = item.ProductId;
                formDonation.BaseCount = productDonation.BaseCount;
                formDonation.DonateCount = hostItem.Count * item.Count / formDonation.BaseCount;
                formDonation.CreatedId = userId;
                formDonation.CreatedTime = DateTime.Now;

                list.Add(formDonation);
            }

            return list;
        }

        private static void CreateItem(OrderFormItemEntity item, Database db, DbTransaction trans)
        {
            var sql = string.Format(@"insert into order_form_items({0}) values(
@p_id,@p_order_id,@p_form_no,@p_hospital_id,@p_product_id,@p_order_person,@p_count,@p_donate_count,
@p_price,@p_expect_price,@p_sum,@p_expired_date,@p_expect_date,@p_contact,@p_status,
@p_need_audit,@p_need_check,@p_need_split,@p_split_copies,@p_split_unit,@p_split_capacity,@p_split_package_count,@p_valid_days)", ITEM_COLUMN_SQL);

            item.Id = Guid.NewGuid().ToString();

            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_id", DbType.String, item.Id);
            db.AddInParameter(dc, "p_order_id", DbType.String, item.OrderId);
            db.AddInParameter(dc, "p_form_no", DbType.Int32, item.FormNo);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, item.HospitalId);
            db.AddInParameter(dc, "p_product_id", DbType.String, item.ProductId);
            db.AddInParameter(dc, "p_order_person", DbType.String, item.OrderPerson);
            db.AddInParameter(dc, "p_count", DbType.Int32, item.Count);
            db.AddInParameter(dc, "p_donate_count", DbType.Int32, item.DonateCount);
            db.AddInParameter(dc, "p_price", DbType.Decimal, item.Price);
            db.AddInParameter(dc, "p_expect_price", DbType.Decimal, DBNull.Value);
            db.AddInParameter(dc, "p_sum", DbType.Decimal, item.Sum);
            db.AddInParameter(dc, "p_expired_date", DbType.DateTime, item.ExpiredDate.HasValue ? (object)item.ExpiredDate.Value : DBNull.Value);
            db.AddInParameter(dc, "p_expect_date", DbType.DateTime, item.ExpectDate.HasValue ? (object)item.ExpectDate.Value : DBNull.Value);
            db.AddInParameter(dc, "p_contact", DbType.String, item.Contact);
            db.AddInParameter(dc, "p_status", DbType.String, item.Status);
            db.AddInParameter(dc, "p_need_audit", DbType.Boolean, item.NeedAudit);
            db.AddInParameter(dc, "p_need_check", DbType.Boolean, item.NeedCheck);
            db.AddInParameter(dc, "p_need_split", DbType.Boolean, item.NeedSplit);
            db.AddInParameter(dc, "p_split_copies", DbType.Int32, item.SplitCopies);
            db.AddInParameter(dc, "p_split_unit", DbType.String, item.SplitUnit);
            db.AddInParameter(dc, "p_split_capacity", DbType.String, item.SplitCapacity);
            db.AddInParameter(dc, "p_split_package_count", DbType.Int32, item.SplitPackageCount);
            db.AddInParameter(dc, "p_valid_days", DbType.Decimal, item.ValidDays);

            db.ExecuteNonQuery(dc, trans);
        }
        
        private static void UpdateItem(OrderFormItemEntity hostItem, Database db, DbTransaction trans)
        {
            var sql = @"update order_form_items
set product_id=@p_product_id,order_person=@p_order_person,count=@p_count,price=@p_price,sum=@p_sum,expired_date=@p_expired_date,
    expect_date=@p_expect_date,contact=@p_contact,status=@p_status,need_audit=@p_need_audit,need_check=@p_need_check,
    need_split=@p_need_split,split_copies=@p_split_copies,split_unit=@p_split_unit,
    split_capacity=@p_split_capacity,split_package_count=@p_split_package_count,valid_days=@p_valid_days
where id=@p_id";

            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_id", DbType.String, hostItem.Id);
            db.AddInParameter(dc, "p_product_id", DbType.String, hostItem.ProductId);
            db.AddInParameter(dc, "p_order_person", DbType.String, hostItem.OrderPerson);
            db.AddInParameter(dc, "p_count", DbType.Int32, hostItem.Count);
            db.AddInParameter(dc, "p_price", DbType.Decimal, hostItem.Price);
            db.AddInParameter(dc, "p_sum", DbType.Decimal, hostItem.Sum);
            db.AddInParameter(dc, "p_expired_date", DbType.DateTime, hostItem.ExpiredDate.HasValue ? (object)hostItem.ExpiredDate.Value : DBNull.Value);
            db.AddInParameter(dc, "p_expect_date", DbType.DateTime, hostItem.ExpectDate.HasValue ? (object)hostItem.ExpectDate.Value : DBNull.Value);
            db.AddInParameter(dc, "p_contact", DbType.String, hostItem.Contact);
            db.AddInParameter(dc, "p_status", DbType.String, hostItem.Status);
            db.AddInParameter(dc, "p_need_audit", DbType.Boolean, hostItem.NeedAudit);
            db.AddInParameter(dc, "p_need_check", DbType.Boolean, hostItem.NeedCheck);
            db.AddInParameter(dc, "p_need_split", DbType.Boolean, hostItem.NeedSplit);
            db.AddInParameter(dc, "p_split_copies", DbType.Int32, hostItem.SplitCopies);
            db.AddInParameter(dc, "p_split_unit", DbType.String, hostItem.SplitUnit);
            db.AddInParameter(dc, "p_split_capacity", DbType.String, hostItem.SplitCapacity);
            db.AddInParameter(dc, "p_split_package_count", DbType.Int32, hostItem.SplitPackageCount);
            db.AddInParameter(dc, "p_valid_days", DbType.Decimal, hostItem.ValidDays);

            db.ExecuteNonQuery(dc, trans);
        }

        private static void UpdateItemFromDonation(OrderFormItemEntity item, Database db, DbTransaction trans)
        {
            var sql = @"update order_form_items
set 
donate_count=@p_donate_count,need_check=@p_need_check,need_split=@p_need_split,
split_copies=@p_split_copies,split_unit=@p_split_unit,
split_capacity=@p_split_capacity,split_package_count=@p_split_package_count,
valid_days=@p_valid_days
where id=@p_id";

            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_id", DbType.String, item.Id);
            db.AddInParameter(dc, "p_donate_count", DbType.Int32, item.DonateCount);
            db.AddInParameter(dc, "p_need_check", DbType.Boolean, item.NeedCheck);
            db.AddInParameter(dc, "p_need_split", DbType.Boolean, item.NeedSplit);
            db.AddInParameter(dc, "p_split_copies", DbType.Int32, item.SplitCopies);
            db.AddInParameter(dc, "p_split_unit", DbType.String, item.SplitUnit);
            db.AddInParameter(dc, "p_split_capacity", DbType.String, item.SplitCapacity);
            db.AddInParameter(dc, "p_split_package_count", DbType.Int32, item.SplitPackageCount);
            db.AddInParameter(dc, "p_valid_days", DbType.Decimal, item.ValidDays);

            db.ExecuteNonQuery(dc, trans);
        }

        private static void CreateItems(OrderFormEntity form, IList<OrderFormItemEntity> savedItems, IList<OrderFormDonateItemEntity> donateItems, IList<HospitalProductEntity> productsSetting, Database db, DbTransaction trans)
        {
            var dic = savedItems.ToDictionary(item => item.ProductId);
            var productMap = productsSetting.ToDictionary(item => item.ProductId);

            OrderFormItemEntity formItem;
            HospitalProductEntity setting;
            foreach (var item in donateItems)
            {
                if(!dic.TryGetValue(item.ProductId, out formItem))
                {
                    formItem = new OrderFormItemEntity();
                    formItem.FormNo = form.FormNo;
                    formItem.OrderId = form.Id;
                    formItem.HospitalId = form.HospitalId;
                    formItem.ProductId = item.ProductId;
                    formItem.Status = OrderFormItemStatus.Saved;
                    formItem.NeedAudit = false;
                }
                formItem.DonateCount += item.Count;

                if (productMap.TryGetValue(formItem.ProductId, out setting))
                {
                    formItem.NeedCheck = setting.NeedCheck;
                    formItem.NeedSplit = setting.NeedSplit;
                    formItem.SplitCopies = setting.SplitCopies;
                    formItem.SplitUnit = setting.SplitUnit;
                    formItem.ValidDays = setting.ValidDays;
                    formItem.SplitCapacity = setting.SplitCapacity;
                    formItem.SplitPackageCount = setting.MiniSplitNumber;
                }

                if (string.IsNullOrEmpty(formItem.Id))
                {
                    CreateItem(formItem, db, trans);
                }
                else
                {
                    UpdateItemFromDonation(formItem, db, trans);
                }
            }
        }

        private static void CreateDonateItems(OrderFormItemEntity hostItem, IList<OrderFormDonateItemEntity> donateItems, Database db, DbTransaction trans)
        {
            var sql = @"insert into order_form_donate_items(id,order_id,item_id,hospital_id,product_id,count,created_id,created_time)
values(@p_id,@p_order_id,@p_item_id,@p_hospital_id,@p_product_id,@p_count,@p_created_id,@p_created_time)";

            foreach (var item in donateItems)
            {
                item.Id = Guid.NewGuid().ToString();
                item.ItemId = hostItem.Id;

                var cmd = db.GetSqlStringCommand(sql);
                db.AddInParameter(cmd, "p_id", DbType.String, item.Id);
                db.AddInParameter(cmd, "p_order_id", DbType.String, item.OrderId);
                db.AddInParameter(cmd, "p_item_id", DbType.String, item.ItemId);
                db.AddInParameter(cmd, "p_hospital_id", DbType.String, item.HospitalId);
                db.AddInParameter(cmd, "p_product_id", DbType.String, item.ProductId);
                db.AddInParameter(cmd, "p_count", DbType.Int32, item.Count);
                db.AddInParameter(cmd, "p_created_id", DbType.String, item.CreatedId);
                db.AddInParameter(cmd, "p_created_time", DbType.DateTime, item.CreatedTime);

                db.ExecuteNonQuery(cmd, trans);
            }
        }

        private static void CreateDonations(IList<OrderFormDonationEntity> donations, Database db, DbTransaction trans)
        {
            var sql = @"insert into order_form_donations(id,order_id,item_id,hospital_id,vendor_id,product_id,base_count,donate_count,created_id,created_time) 
values(@p_id,@p_order_id,@p_item_id,@p_hospital_id,@p_vendor_id,@p_product_id,@p_base_count,@p_donate_count,@p_created_id,@p_created_time)";

            foreach (var item in donations)
            {
                item.Id = Guid.NewGuid().ToString();

                var cmd = db.GetSqlStringCommand(sql);

                db.AddInParameter(cmd, "p_id", DbType.String, item.Id);
                db.AddInParameter(cmd, "p_order_id", DbType.String, item.OrderId);
                db.AddInParameter(cmd, "p_item_id", DbType.String, item.ItemId);
                db.AddInParameter(cmd, "p_hospital_id", DbType.String, item.HospitalId);
                db.AddInParameter(cmd, "p_vendor_id", DbType.String, item.VendorId);
                db.AddInParameter(cmd, "p_product_id", DbType.String, item.ProductId);
                db.AddInParameter(cmd, "p_base_count", DbType.Decimal, item.BaseCount);
                db.AddInParameter(cmd, "p_donate_count", DbType.Decimal, item.DonateCount);
                db.AddInParameter(cmd, "p_created_id", DbType.String, item.CreatedId);
                db.AddInParameter(cmd, "p_created_time", DbType.DateTime, item.CreatedTime);

                db.ExecuteNonQuery(cmd, trans);
            }
        }
        #endregion

        #region Delete Item
        public static void DeleteItem(string itemId)
        {
            var db = DatabaseFactory.CreateDatabase();
            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        UpdateItemDonateCount(itemId, db, trans);
                        DeleteItem(itemId, db, trans);
                        DeleteDonateItems(itemId, db, trans);
                        DeleteDonations(itemId, db, trans);

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

        private static void UpdateItemDonateCount(string itemId, Database db, DbTransaction trans)
        {
            var donateItems = GetDonateItems(itemId);
            if(donateItems.Count == 0)
            {
                return;
            }

            var sql = "update order_form_items set donate_count = donate_count - @p_count where order_id=@p_order_id and product_id=@p_product_id";
            foreach(var item in donateItems)
            {
                var localcmd = db.GetSqlStringCommand(sql);
                db.AddInParameter(localcmd, "p_order_id", DbType.String, item.OrderId);
                db.AddInParameter(localcmd, "p_product_id", DbType.String, item.ProductId);
                db.AddInParameter(localcmd, "p_count", DbType.Int32, item.Count);

                db.ExecuteNonQuery(localcmd, trans);
            }

            IList<string> names;
            string namesSql;
            StringHelper.GenerInParameters("p_product_id", donateItems.Count, out names, out namesSql);

            sql = string.Format("delete order_form_items where order_id=@p_order_id and product_id in ({0}) and count=0 and donate_count=0", namesSql);
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_order_id", DbType.String, donateItems[0].OrderId);
            for (var i = 0;i < donateItems.Count; i++)
            {
                db.AddInParameter(cmd, names[i], DbType.String, donateItems[i].ProductId);
            }

            db.ExecuteNonQuery(cmd, trans);
        }

        private static void DeleteItem(string id, Database db, DbTransaction trans)
        {
            var sql = "update order_form_items set count=0,sum=0 where id=@p_id and donate_count > 0";
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_id", DbType.String, id);
            var rowCount = db.ExecuteNonQuery(cmd, trans);

            sql = @"delete order_form_items where id=@p_id and donate_count=0";
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);
            db.ExecuteNonQuery(dc, trans);
        }

        private static void DeleteDonateItems(string itemId, Database db, DbTransaction trans)
        {
            var sql = "delete order_form_donate_items where item_id=@p_item_id";
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_item_id", DbType.String, itemId);

            db.ExecuteNonQuery(cmd, trans);
        }

        private static void DeleteDonations(string itemId, Database db, DbTransaction trans)
        {
            var sql = "delete order_form_donations where item_id=@p_item_id";
            var cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, "p_item_id", DbType.String, itemId);

            db.ExecuteNonQuery(cmd, trans);
        }
        #endregion


        internal static void UpdateStatus(string id, string itemId, string status, Database db, DbTransaction trans)
        {
            var sql = "update order_form set status = @p_status where id = @p_id";
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);
            db.AddInParameter(dc, "p_status", DbType.String, status);

            db.ExecuteNonQuery(dc, trans);

            UpdateItemStatus(itemId, status, db, trans);
        }

        private static void UpdateStatus(string id, string status, Database db, DbTransaction trans)
        {
            var sql = "update order_form set status = @p_status where id = @p_id";
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);
            db.AddInParameter(dc, "p_status", DbType.String, status);
            db.ExecuteNonQuery(dc, trans);

            if(string.Compare(status, OrderFormStatus.Waiting, true) == 0)
            {
                sql = "update order_form_items set status = @p_status where order_id = @p_id and need_audit=0";
                dc = db.GetSqlStringCommand(sql);
                db.AddInParameter(dc, "p_id", DbType.String, id);
                db.AddInParameter(dc, "p_status", DbType.String, OrderFormItemStatus.Waiting);
                db.ExecuteNonQuery(dc, trans);

                sql = "update order_form_items set status = @p_status where order_id = @p_id and need_audit=1";
                dc = db.GetSqlStringCommand(sql);
                db.AddInParameter(dc, "p_id", DbType.String, id);
                db.AddInParameter(dc, "p_status", DbType.String, OrderFormItemStatus.Auditing);
                db.ExecuteNonQuery(dc, trans);
            }
            else
            {
                var itemStatus = OrderFormItemStatus.Dispatching;
                if(string.Compare(status, OrderFormStatus.Saved, true) == 0)
                {
                    itemStatus = OrderFormItemStatus.Saved;
                }

                sql = "update order_form_items set status = @p_status where order_id = @p_id";
                dc = db.GetSqlStringCommand(sql);
                db.AddInParameter(dc, "p_id", DbType.String, id);
                db.AddInParameter(dc, "p_status", DbType.String, status);
                db.ExecuteNonQuery(dc, trans);
            }
        }

        internal static void UpdateItemStatus(string itemId, string status, Database db, DbTransaction trans)
        {
            var sql = "update order_form_items set status=@p_status where id=@p_id";
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_id", DbType.String, itemId);
            db.AddInParameter(dc, "p_status", DbType.String, status);

            db.ExecuteNonQuery(dc, trans);
        }

        #region Pass Form
        public static void Pass(OrderFormEntity form)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var list = GetHospitalDonations(form);
                        UpdateDonations(list, db, trans);
                        UpdateStatus(form.Id, OrderFormStatus.Waiting, db, trans);

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

        private static IList<HospitalProductDonationEntity> GetHospitalDonations(OrderFormEntity form)
        {
            var list = new Dictionary<string, HospitalProductDonationEntity>();

            var donateItems = GetDonateItems(form.Id, null);
            var donations = GetDonations(form.Id, null);

            HospitalProductDonationEntity entity;
            foreach (var item in donateItems)
            {
                if (!list.TryGetValue(item.ProductId, out entity))
                {
                    entity = new HospitalProductDonationEntity
                    {
                        HospitalId = form.HospitalId,
                        UnitId = form.ApplyUnitId,
                        VendorId = form.VendorId,
                        ProductId = item.ProductId
                    };
                    list[item.ProductId] = entity;
                }

                entity.UsedCount += item.Count;
            }

            foreach (var item in donations)
            {
                if (!list.TryGetValue(item.ProductId, out entity))
                {
                    entity = new HospitalProductDonationEntity
                    {
                        HospitalId = form.HospitalId,
                        UnitId = form.ApplyUnitId,
                        VendorId = form.VendorId,
                        ProductId = item.ProductId
                    };
                    list[item.ProductId] = entity;
                }

                entity.TotalCount += item.DonateCount;
            }

            return list.Values.ToList();
        }

        private static void UpdateDonations(IList<HospitalProductDonationEntity>  changingList, Database db, DbTransaction trans)
        {
            HospitalProductDonationRepository.UpdateDonations(changingList, db, trans);
        }
        #endregion

        #region Cancel
        public static void Cancel(OrderFormEntity form)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var list = GetHospitalDonations(form);
                        foreach(var item in list)
                        {
                            item.TotalCount = item.TotalCount * -1;
                            item.UsedCount = item.UsedCount * -1;
                        }

                        UpdateDonations(list, db, trans);
                        UpdateStatus(form.Id, OrderFormStatus.Saved, db, trans);

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
        #endregion


        //        public static void Add(OrderFormEntity order, IList<OrderFormItemEntity> detailList)
        //        {

        //            var db = DatabaseFactory.CreateDatabase();

        //            using (var conn = db.CreateConnection())
        //            {
        //                conn.Open();
        //                using (var trans = conn.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        Add(order, db, trans);
        //                        AddDetailList(order.Id, detailList, db, trans);

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

        //        private static void Add(OrderFormEntity order, Database db, DbTransaction trans)
        //        {
        //            var sql = string.Format(@"INSERT INTO orders({0}) VALUES(
        //@p_id, @p_filler_id, @p_apply_time, @p_form_no, @p_hospital_id, @p_apply_unit_id,
        //@p_vendor_id, @p_receipt_id, @p_status, 
        //@p_created_id, @p_created_time, @p_updated_id, @p_updated_time)", COLUMN_SQL);

        //            var dc = db.GetSqlStringCommand(sql);
        //            db.AddInParameter(dc, "p_id", DbType.String, order.Id);
        //            db.AddInParameter(dc, "p_filler_id", DbType.String, order.FillerId);
        //            db.AddInParameter(dc, "p_apply_time", DbType.DateTime, order.ApplyTime);
        //            db.AddInParameter(dc, "p_form_no", DbType.Int32, order.FormNo);
        //            db.AddInParameter(dc, "p_hospital_id", DbType.String, order.HospitalId);
        //            db.AddInParameter(dc, "p_apply_unit_id", DbType.String, order.ApplyUnitId);
        //            db.AddInParameter(dc, "p_vendor_id", DbType.String, order.VendorId);
        //            db.AddInParameter(dc, "p_receipt_id", DbType.String, order.ReceiptId);
        //            db.AddInParameter(dc, "p_status", DbType.Int32, order.Status);
        //            db.AddInParameter(dc, "p_created_id", DbType.String, order.CreatedId);
        //            db.AddInParameter(dc, "p_created_time", DbType.DateTime, order.CreatedTime);
        //            db.AddInParameter(dc, "p_updated_id", DbType.String, order.UpdatedId);
        //            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, order.UpdatedTime);

        //            db.ExecuteNonQuery(dc, trans);
        //        }

        //        public static void Update(OrderFormEntity order, IList<OrderFormItemEntity> detailList)
        //        {
        //            var db = DatabaseFactory.CreateDatabase();

        //            using (var conn = db.CreateConnection())
        //            {
        //                conn.Open();
        //                using (var trans = conn.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        Update(order, db, trans);
        //                        DeleteDetails(order.Id, db, trans);
        //                        AddDetailList(order.Id, detailList, db, trans);

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

        //        private static void Update(OrderFormEntity order, Database db, DbTransaction trans)
        //        {
        //            var sql = @"UPDATE orders 
        //SET apply_unit_id=@p_apply_unit_id, vendor_id=@p_vendor_id, receipt_id=@p_receipt_id, 
        //status=@p_status, updated_id=@p_updated_id, updated_time=@p_updated_time WHERE id=@p_id";

        //            var dc = db.GetSqlStringCommand(sql);
        //            db.AddInParameter(dc, "p_id", DbType.String, order.Id);
        //            db.AddInParameter(dc, "p_apply_unit_id", DbType.String, order.ApplyUnitId);
        //            db.AddInParameter(dc, "p_vendor_id", DbType.String, order.VendorId);
        //            db.AddInParameter(dc, "p_receipt_id", DbType.String, order.ReceiptId);
        //            db.AddInParameter(dc, "p_status", DbType.Int32, order.Status);
        //            db.AddInParameter(dc, "p_updated_id", DbType.String, order.UpdatedId);
        //            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, order.UpdatedTime);

        //            db.ExecuteNonQuery(dc, trans);
        //        }

        //        private static void AddDetailList(string orderId, IList<OrderFormItemEntity> detailList, Database db, DbTransaction trans)
        //        {
        //            if(detailList == null)
        //            {
        //                return;
        //            }

        //            var sql = string.Format(@"INSERT INTO order_detail({0}) VALUES(
        //@p_id, @p_order_id, @p_form_no, @p_hospital_id, @p_product_id, @p_need_check,
        //@p_need_split, @p_split_capacity, @p_mini_split_number, @p_valid_days, @p_contact_id, @p_price, @p_expected_price, @p_count, 
        //@p_expired_date, @p_expected_date, @p_order_person, @p_status, @p_need_auditing, @p_split_copies, @p_split_unit)", ITEM_COLUMN_SQL);

        //            foreach (var item in detailList)
        //            {
        //                item.Id = Guid.NewGuid().ToString();
        //                item.OrderId = orderId;

        //                var dc = db.GetSqlStringCommand(sql);
        //                db.AddInParameter(dc, "p_id", DbType.String, item.Id);
        //                db.AddInParameter(dc, "p_order_id", DbType.String, item.OrderId);
        //                db.AddInParameter(dc, "p_form_no", DbType.Int32, item.FormNo);
        //                db.AddInParameter(dc, "p_hospital_id", DbType.String, item.HospitalId);
        //                db.AddInParameter(dc, "p_product_id", DbType.String, item.ProductId);
        //                db.AddInParameter(dc, "p_need_check", DbType.Boolean, item.NeedCheck);
        //                db.AddInParameter(dc, "p_need_split", DbType.Boolean, item.NeedSplit);
        //                db.AddInParameter(dc, "p_split_capacity", DbType.String, item.SplitCapacity);
        //                db.AddInParameter(dc, "p_mini_split_number", DbType.Int32, item.MiniSplitNumber);
        //                db.AddInParameter(dc, "p_valid_days", DbType.Decimal, item.ValidDays);
        //                db.AddInParameter(dc, "p_contact_id", DbType.String, item.Contact);
        //                db.AddInParameter(dc, "p_price", DbType.Decimal, item.Price);
        //                db.AddInParameter(dc, "p_expected_price", DbType.Decimal, item.ExpectedPrice);
        //                db.AddInParameter(dc, "p_count", DbType.Int32, item.Count);
        //                db.AddInParameter(dc, "p_expired_date", DbType.DateTime, item.ExpiredDate);//item.ExpiredDate.HasValue ? item.ExpiredDate.Value : DateTime.MaxValue);
        //                db.AddInParameter(dc, "p_expected_date", DbType.DateTime, item.ExpectedDate); //item.ExpectedDate.HasValue ? item.ExpectedDate.Value : DateTime.MaxValue);
        //                db.AddInParameter(dc, "p_order_person", DbType.String, item.OrderPerson);
        //                db.AddInParameter(dc, "p_status", DbType.Int32, item.Status);
        //                db.AddInParameter(dc, "p_need_auditing", DbType.Boolean, item.Status == OrderDetailStatus.WAITING_AUDIT ? true : false);
        //                db.AddInParameter(dc, "p_split_copies", DbType.Int32, item.SplitCopies);
        //                db.AddInParameter(dc, "p_split_unit", DbType.String, item.SplitUnit);

        //                db.ExecuteNonQuery(dc, trans);
        //            }
        //        }

        //        public static void AddDetail(OrderFormItemEntity detail)
        //        {
        //            var db = DatabaseFactory.CreateDatabase();

        //            var order = Get(detail.OrderId);
        //            if(order == null)
        //            {
        //                throw new Exception("The order form does not exist.");
        //            }

        //            using (var conn = db.CreateConnection())
        //            {
        //                conn.Open();
        //                using (var trans = conn.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        var sql = string.Format(@"INSERT INTO order_detail({0}) VALUES(
        //@p_id, @p_order_id, @p_form_no, @p_hospital_id, @p_product_id, @p_need_check,
        //@p_need_split, @p_split_capacity, @p_mini_split_number, @p_valid_days, @p_contact_id, @p_price, @p_expected_price, @p_count, 
        //@p_expired_date, @p_expected_date, @p_order_person, @p_status, @p_need_auditing,@p_split_copies, @p_split_unit)", ITEM_COLUMN_SQL);

        //                        detail.Id = Guid.NewGuid().ToString();
        //                        var dc = db.GetSqlStringCommand(sql);
        //                        db.AddInParameter(dc, "p_id", DbType.String, detail.Id);
        //                        db.AddInParameter(dc, "p_order_id", DbType.String, detail.OrderId);
        //                        db.AddInParameter(dc, "p_form_no", DbType.Int32, detail.FormNo);
        //                        db.AddInParameter(dc, "p_hospital_id", DbType.String, detail.HospitalId);
        //                        db.AddInParameter(dc, "p_product_id", DbType.String, detail.ProductId);
        //                        db.AddInParameter(dc, "p_need_check", DbType.Boolean, detail.NeedCheck);
        //                        db.AddInParameter(dc, "p_need_split", DbType.Boolean, detail.NeedSplit);
        //                        db.AddInParameter(dc, "p_split_capacity", DbType.String, detail.SplitCapacity);
        //                        db.AddInParameter(dc, "p_mini_split_number", DbType.Int32, detail.MiniSplitNumber);
        //                        db.AddInParameter(dc, "p_valid_days", DbType.Decimal, detail.ValidDays);
        //                        db.AddInParameter(dc, "p_contact_id", DbType.String, detail.Contact);
        //                        db.AddInParameter(dc, "p_price", DbType.Decimal, detail.Price);
        //                        db.AddInParameter(dc, "p_expected_price", DbType.Decimal, detail.ExpectedPrice);
        //                        db.AddInParameter(dc, "p_count", DbType.Int32, detail.Count);
        //                        db.AddInParameter(dc, "p_expired_date", DbType.DateTime, detail.ExpiredDate);//item.ExpiredDate.HasValue ? item.ExpiredDate.Value : DateTime.MaxValue);
        //                        db.AddInParameter(dc, "p_expected_date", DbType.DateTime, detail.ExpectedDate); //item.ExpectedDate.HasValue ? item.ExpectedDate.Value : DateTime.MaxValue);
        //                        db.AddInParameter(dc, "p_order_person", DbType.String, detail.OrderPerson);
        //                        db.AddInParameter(dc, "p_status", DbType.Int32, detail.Status);
        //                        db.AddInParameter(dc, "p_need_auditing", DbType.Boolean, detail.Status == OrderDetailStatus.WAITING_AUDIT ? true : false);
        //                        db.AddInParameter(dc, "p_split_copies", DbType.Int32, detail.SplitCopies);
        //                        db.AddInParameter(dc, "p_split_unit", DbType.String, detail.SplitUnit);

        //                        db.ExecuteNonQuery(dc, trans);

        //                        trans.Commit();

        //                        FormApproversRepository.CreateApprovers(detail.OrderId, detail.FormNo, FormType.OrderAudit, detail.HospitalId, order.FillerId, order.ApplyUnitId);
        //                    }
        //                    catch
        //                    {
        //                        trans.Rollback();
        //                        throw;
        //                    }
        //                }
        //            }
        //        }

        //        private static void DeleteDetails(string orderId, Database db, DbTransaction trans)
        //        {
        //            var sql = "DELETE order_detail WHERE order_id=@p_order_id";
        //            var dc = db.GetSqlStringCommand(sql);
        //            db.AddInParameter(dc, "p_order_id", DbType.String, orderId);

        //            db.ExecuteNonQuery(dc, trans);
        //        }

        //        public static void DeleteDetail(string id)
        //        {
        //            var db = DatabaseFactory.CreateDatabase();

        //            using (var conn = db.CreateConnection())
        //            {
        //                conn.Open();
        //                using (var trans = conn.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        var sql = "DELETE order_detail WHERE id=@p_id";
        //                        var dc = db.GetSqlStringCommand(sql);
        //                        db.AddInParameter(dc, "p_id", DbType.String, id);
        //                        db.ExecuteNonQuery(dc, trans);


        //                        sql = "delete form_approvers where form_id = @p_form_id and form_type = @p_form_type";
        //                        dc = db.GetSqlStringCommand(sql);
        //                        db.AddInParameter(dc, "p_form_id", DbType.String, id);
        //                        db.AddInParameter(dc, "p_form_type", DbType.Int32, (int)FormType.OrderAudit);
        //                        db.ExecuteNonQuery(dc, trans);

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

        //        






        //        






        //        public static bool NeedMakeInspection(string orderDetailId)
        //        {
        //            var entity = GetDetail(orderDetailId);
        //            if(entity == null)
        //            {
        //                throw new Exception("The order detail dose not exist.");
        //            }

        //            return entity.NeedCheck;
        //        }

        //        public static bool NeedAudit(string orderDetailId)
        //        {
        //            //TODO:
        //            return false;
        //        }


        //        public static void AdjustPrice(string detailId, decimal price)
        //        {
        //            var detail = GetDetail(detailId);
        //            var order = Get(detail.OrderId);

        //            if(detail == null)
        //            {
        //                throw new Exception("The order detail does not exist!");
        //            }

        //            var db = DatabaseFactory.CreateDatabase();

        //            using (var conn = db.CreateConnection())
        //            {
        //                conn.Open();
        //                using (var trans = conn.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        var sql = "update order_detail set expected_price=@p_adjust_price, status=@p_status where id=@p_detail_id";

        //                        var dc = db.GetSqlStringCommand(sql);

        //                        detail.Status = OrderFormItemStatus.Auditing;

        //                        db.AddInParameter(dc, "p_detail_id", DbType.String, detailId);
        //                        db.AddInParameter(dc, "p_adjust_price", DbType.Decimal, price);
        //                        db.AddInParameter(dc, "p_status", DbType.String, detail.Status);

        //                        db.ExecuteNonQuery(dc, trans);

        //                        trans.Commit();

        //                        FormApproversRepository.CreateApprovers(detail.OrderId, detail.FormNo, FormType.OrderAudit, detail.HospitalId, order.FillerId, order.ApplyUnitId);
        //                    }
        //                    catch
        //                    {
        //                        trans.Rollback();
        //                        throw;
        //                    }
        //                }
        //            }
        //        }


        public static void AutoDelete(string id, string userId)
        {
            //var order = Get(id);

            //if(order.DetailList == null || order.DetailList.Count == 0)
            //{
            //    var sql = "update orders set status = @p_status, updated_time=@p_datetime, updated_id=@p_user_id where id = @p_id and status <> @p_status";

            //    var db = DatabaseFactory.CreateDatabase();
            //    var dc = db.GetSqlStringCommand(sql);
            //    db.AddInParameter(dc, "p_id", DbType.String, id);
            //    db.AddInParameter(dc, "p_status", DbType.Int32, OrderStatus.Delete);
            //    db.AddInParameter(dc, "p_datetime", DbType.DateTime, DateTime.Now);
            //    db.AddInParameter(dc, "p_user_id", DbType.String, userId);

            //    db.ExecuteNonQuery(dc);
            //}
        }

//        public static IList<FormApproverEntity> QueryAuditOrders(AuditQueryCondition condition, PagerInfo pager)
//        {
//            pager.ComputePageCount(QueryCount(condition));

//            var list = new List<FormApproverEntity>();

//            var orderSql = " ORDER BY ";
//            if (pager.OrderFields.Count > 0)
//            {
//                foreach (var field in pager.OrderFields)
//                {
//                    orderSql += field.Field + (field.Desc ? " DESC" : "") + ",";
//                }
//            }
//            else
//            {
//                orderSql += "form_no desc";
//            }

//            var sql = string.Format(@"SELECT {0} FROM order_detail WHERE 1=1{1}", COLUMN_SQL, GetConditionSql(condition, rootId, unitId));

//            sql = @"SELECT * FROM
//            (
//                SELECT ROW_NUMBER() OVER(" + orderSql + @") pid," + COLUMN_SQL + @"
//                FROM (" + sql + @") t            
//            ) t1 WHERE t1.pid BETWEEN @p_pageNo * @p_pageSize + 1 AND (@p_pageNo + 1) * @p_pageSize ";

//            var db = DatabaseFactory.CreateDatabase();
//            var dc = db.GetSqlStringCommand(sql);
//            db.AddInParameter(dc, "p_approver_id", DbType.String, condition.UserId);
//            db.AddInParameter(dc, "p_hospital_id", DbType.String, condition.HospitalId);

//            db.AddInParameter(dc, "p_pageNo", DbType.Int32, pager.PageIndex);
//            db.AddInParameter(dc, "p_pageSize", DbType.Int32, pager.PageSize);

//            using (IDataReader reader = db.ExecuteReader(dc))
//            {
//                while (reader.Read())
//                {
//                    var entity = new FormApproverEntity();
//                    entity.Init(reader);

//                    list.Add(entity);
//                }
//            }

//            return list;
//        }

//        private static int QueryCount(AuditQueryCondition condition)
//        {
//            var sql = string.Format(@"SELECT COUNT(id) FROM order_detail WHERE 1=1 ", COLUMN_SQL);

//            var conditionSql = GetConditionSql(condition);
//            if (!string.IsNullOrEmpty(conditionSql))
//            {
//                sql += conditionSql;
//            }

//            var db = DatabaseFactory.CreateDatabase();
//            var dc = db.GetSqlStringCommand(sql);
//            db.AddInParameter(dc, "p_approver_id", DbType.String, condition.UserId);
//            db.AddInParameter(dc, "p_hospital_id", DbType.String, condition.HospitalId);

//            using (IDataReader reader = db.ExecuteReader(dc))
//            {
//                reader.Read();

//                return reader.GetInt32(0);
//            }
//        }

//        private static string GetConditionSql(AuditQueryCondition condition)
//        {
//            var conditionSql = @" and hospital_id = @p_hospital_id and 
//id in (select order_detail_id from order_detail_approvers where approver_id = @p_approver_id)";
            
//            return conditionSql;
//        }
    }
}
 