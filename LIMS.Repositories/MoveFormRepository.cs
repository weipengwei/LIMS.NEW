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
    public class MoveFormRepository : RepositoryBase
    {
        private const string MOVE_COLUMNS = @"
[id]
,[hospital_id]
,[form_no]
,[requested_by]
,[requested_time]
,[from_storeroom]
,[to_storeroom]
,[status]
,[created_id]
,[created_time]
,[updated_id]
,[updated_time]
,[moveout_by]
,[moveout_time]
,[movein_by]
,[movein_time] ";

        private const string ITEM_COLUMNS = @"
[id]
,[move_id]
,[form_no]
,[product_id]
,[count] ";

        private const string SERIAL_COLUMNS = @"
[id]
,[move_id]
,[form_no]
,[product_id]
,[from_serial]
,[to_serial]
,[count]
,[package_count] ";

        #region Save
        public MoveFormEntity Save(MoveFormEntity move)
        {
            string sql = string.Empty;

            if (string.IsNullOrEmpty(move.Id))
            {
                sql = string.Format(@"insert into move_form({0}) values(
@p_id
,@p_hospital_id
,@p_form_no
,@p_requested_by
,@p_requested_time
,@p_from_storeroom
,@p_to_storeroom
,@p_status
,@p_created_id
,@p_created_time
,@p_updated_id
,@p_updated_time
,@p_moveout_by
,@p_moveout_time
,@p_movein_by
,@p_movein_time)", MOVE_COLUMNS);

                move.Id = Guid.NewGuid().ToString();
                move.Status = MoveFormStatus.Saved;
                move.UpdatedId = string.Empty;
                move.UpdatedTime = DateTime.MaxValue;
                move.MoveoutBy = string.Empty;
                move.MoveoutTime = null;
                move.MoveinBy = string.Empty;
                move.MoveinTime = null;
            }
            else
            {
                sql = @"update move_form set 
hospital_id     = @p_hospital_id
,form_no        = @p_form_no
,requested_by   = @p_requested_by
,requested_time = @p_requested_time
,from_storeroom = @p_from_storeroom
,to_storeroom   = @p_to_storeroom
,status         = @p_status
,created_id     = @p_created_id
,created_time   = @p_created_time
,updated_id     = @p_updated_id
,updated_time   = @p_updated_time
,moveout_by     = @p_moveout_by
,moveout_time   = @p_moveout_time
,movein_by      = @p_movein_by
,movein_time    = @p_movein_time
where id = @p_id ";
            }

            DbCommand dc = Db.GetSqlStringCommand(sql);

            Db.AddInParameter(dc, "p_id", DbType.String, move.Id);
            Db.AddInParameter(dc, "p_hospital_id", DbType.String, move.HospitalId);
            Db.AddInParameter(dc, "p_form_no", DbType.Int32, move.FormNo);
            Db.AddInParameter(dc, "p_requested_by", DbType.String, move.RequestedBy);
            Db.AddInParameter(dc, "p_requested_time", DbType.DateTime, move.RequestedTime);
            Db.AddInParameter(dc, "p_from_storeroom", DbType.String, move.FromStoreroomId);
            Db.AddInParameter(dc, "p_to_storeroom", DbType.String, move.ToStoreroomId);
            Db.AddInParameter(dc, "p_status", DbType.String, move.Status);
            Db.AddInParameter(dc, "p_created_id", DbType.String, move.CreatedId);
            Db.AddInParameter(dc, "p_created_time", DbType.DateTime, move.CreatedTime);
            Db.AddInParameter(dc, "p_updated_id", DbType.String, move.UpdatedId);
            Db.AddInParameter(dc, "p_updated_time", DbType.DateTime, move.UpdatedTime);
            Db.AddInParameter(dc, "p_moveout_by", DbType.String, move.MoveoutBy);
            Db.AddInParameter(dc, "p_moveout_time", DbType.DateTime, move.MoveoutTime);
            Db.AddInParameter(dc, "p_movein_by", DbType.String, move.MoveinBy);
            Db.AddInParameter(dc, "p_movein_time", DbType.DateTime, move.MoveinTime);

            Db.ExecuteNonQuery(dc, DbTrans);

            return move;
        }

        public MoveFormItemEntity SaveItem(MoveFormItemEntity moveItem)
        {
            string sql = string.Empty;

            if (string.IsNullOrEmpty(moveItem.Id))
            {
                sql = string.Format(@"insert into move_form_items({0}) values(
@p_id
,@p_move_id
,@p_form_no
,@p_product_id
,@p_count)", ITEM_COLUMNS);

                moveItem.Id = Guid.NewGuid().ToString();
            }
            else
            {
                sql = @"update move_form_items set 
move_id         = @p_move_id
,form_no        = @p_form_no
,product_id     = @p_product_id
,count          = @p_count  
where id = @p_id";
            }

            DbCommand dc = Db.GetSqlStringCommand(sql);

            Db.AddInParameter(dc, "p_id", DbType.String, moveItem.Id);
            Db.AddInParameter(dc, "p_move_id", DbType.String, moveItem.MoveId);
            Db.AddInParameter(dc, "p_form_no", DbType.Int32, moveItem.FormNo);
            Db.AddInParameter(dc, "p_product_id", DbType.String, moveItem.ProductId);
            Db.AddInParameter(dc, "p_count", DbType.Int32, moveItem.Count);

            Db.ExecuteNonQuery(dc, DbTrans);

            return moveItem;
        }

        public MoveFormSerialEntity SaveSerial(MoveFormSerialEntity moveSerial)
        {
            string sql = string.Empty;

            if (string.IsNullOrEmpty(moveSerial.Id))
            {
                sql = string.Format(@"insert into move_form_serial({0}) values(
@p_id
,@p_move_id
,@p_form_no
,@p_product_id
,@p_from_serial
,@p_to_serial
,@p_count
,@p_package_count)", SERIAL_COLUMNS);

                moveSerial.Id = Guid.NewGuid().ToString();
            }
            else
            {
                sql = @"update move_form_serial set 
move_id        = @p_move_id
,form_no       = @p_form_no
,product_id    = @p_product_id
,from_serial   = @p_from_serial
,to_serial     = @p_to_serial
,count         = @p_count  
,package_count = @p_package_count
where id = @p_id";
            }

            DbCommand dc = Db.GetSqlStringCommand(sql);

            Db.AddInParameter(dc, "p_id", DbType.String, moveSerial.Id);
            Db.AddInParameter(dc, "p_move_id", DbType.String, moveSerial.MoveId);
            Db.AddInParameter(dc, "p_form_no", DbType.Int32, moveSerial.FormNo);
            Db.AddInParameter(dc, "p_product_id", DbType.String, moveSerial.ProductId);
            Db.AddInParameter(dc, "p_from_serial", DbType.String, moveSerial.FromSerial);
            Db.AddInParameter(dc, "p_to_serial", DbType.String, moveSerial.ToSerial);
            Db.AddInParameter(dc, "p_count", DbType.Int32, moveSerial.Count);
            Db.AddInParameter(dc, "p_package_count", DbType.Int32, moveSerial.PackageCount);

            Db.ExecuteNonQuery(dc, DbTrans);

            return moveSerial;
        }
        #endregion

        #region Find
        public IList<string> FindBarcodes(string id, string productId)
        {
            var form = FindOne(id);
            if(form.Status == MoveFormStatus.Movedin)
            {
                return GetBarcodesBySerial(id, productId);
            }
            else
            {
                return GetBarcodesFromRuntime(id, productId);
            }
        }

        private IList<string> GetBarcodesBySerial(string id, string productId)
        {
            var serials = FindSerialByProduct(id, productId);
            var barcodes = GoodsSerialRepository.GetBarcodes(serials.Select(item => item.ToSerial).ToList(), true);

            return barcodes.Select(item => item.Barcode).OrderBy(item => item).ToList();
        }

        private IList<string> GetBarcodesFromRuntime(string id, string productId)
        {
            return GoodsRepsitory.FindRuntimes(id, productId).Select(item => item.Barcode).OrderBy(item => item).ToList();
        }

        public MoveFormEntity FindOne(string id)
        {
            var sql = string.Format("select {0} from move_form where id = @p_id", MOVE_COLUMNS);
            var dc = Db.GetSqlStringCommand(sql);
            Db.AddInParameter(dc, "p_id", DbType.String, id);

            using (var reader = Db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var entity = new MoveFormEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public MoveFormItemEntity FindItemByProduct(string formId, string productId)
        {
            var sql = string.Format("select {0} from move_form_items where move_id = @p_move_id and product_id = @p_product_id", ITEM_COLUMNS);
            var dc = Db.GetSqlStringCommand(sql);
            Db.AddInParameter(dc, "p_move_id", DbType.String, formId);
            Db.AddInParameter(dc, "p_product_id", DbType.String, productId);

            using (var reader = Db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var entity = new MoveFormItemEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public MoveFormItemEntity FindOneItem(string formId, string itemId)
        {
            var sql = string.Format("select {0} from move_form_items where move_id = @p_move_id and id = @p_id", ITEM_COLUMNS);
            var dc = Db.GetSqlStringCommand(sql);
            Db.AddInParameter(dc, "p_move_id", DbType.String, formId);
            Db.AddInParameter(dc, "p_id", DbType.String, itemId);

            using (var reader = Db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var entity = new MoveFormItemEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public MoveFormSerialEntity FindFromSerial(string formId, string fromSerial)
        {
            var sql = string.Format("select {0} from move_form_serial where move_id = @p_move_id and from_serial = @p_from_serial", SERIAL_COLUMNS);
            var dc = Db.GetSqlStringCommand(sql);
            Db.AddInParameter(dc, "p_move_id", DbType.String, formId);
            Db.AddInParameter(dc, "p_from_serial", DbType.String, fromSerial);

            using (var reader = Db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var entity = new MoveFormSerialEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public MoveFormSerialEntity FindToSerial(string formId, string toSerial)
        {
            var sql = string.Format("select {0} from move_form_serial where move_id = @p_move_id and to_serial = @p_to_serial", SERIAL_COLUMNS);
            var dc = Db.GetSqlStringCommand(sql);
            Db.AddInParameter(dc, "p_move_id", DbType.String, formId);
            Db.AddInParameter(dc, "p_to_serial", DbType.String, toSerial);

            using (var reader = Db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var entity = new MoveFormSerialEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        private IList<MoveFormSerialEntity> FindSerialByProduct(string formId, string productId)
        {
            var sql = string.Format("select {0} from move_form_serial where move_id = @p_move_id and product_id = @p_product_id", SERIAL_COLUMNS);
            var dc = Db.GetSqlStringCommand(sql);
            Db.AddInParameter(dc, "p_move_id", DbType.String, formId);
            Db.AddInParameter(dc, "p_product_id", DbType.String, productId);

            var list = new List<MoveFormSerialEntity>();
            using (var reader = Db.ExecuteReader(dc))
            {
                if (reader.Read())
                {
                    var entity = new MoveFormSerialEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        public List<MoveFormEntity> FindAll(DateRangeCondition condition, PagerInfo pager)
        {
            pager.ComputePageCount(QueryCount(condition));

            var list = new List<MoveFormEntity>();

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

            var sql = string.Format(@"SELECT {0} FROM move_form WHERE status != 'deleted' and {1}", MOVE_COLUMNS, GetConditionSql(condition));

            sql = @"SELECT * FROM
            (
                SELECT ROW_NUMBER() OVER(" + orderSql + @") pid," + MOVE_COLUMNS + @"
                FROM (" + sql + @") t            
            ) t1 WHERE t1.pid BETWEEN @p_pageNo * @p_pageSize + 1 AND (@p_pageNo + 1) * @p_pageSize ";

            var dc = Db.GetSqlStringCommand(sql);

            AddParameter(dc, condition);

            Db.AddInParameter(dc, "p_pageNo", DbType.Int32, pager.PageIndex);
            Db.AddInParameter(dc, "p_pageSize", DbType.Int32, pager.PageSize);

            using (IDataReader reader = Db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new MoveFormEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        private int QueryCount(DateRangeCondition condition)
        {
            var sql = "SELECT COUNT(id) FROM move_form WHERE status != 'deleted' and ";

            sql += GetConditionSql(condition);

            var dc = Db.GetSqlStringCommand(sql);

            AddParameter(dc, condition);

            using (IDataReader reader = Db.ExecuteReader(dc))
            {
                reader.Read();

                return reader.GetInt32(0);
            }
        }

        private string GetConditionSql(DateRangeCondition condition)
        {
            var conditionSql = @" 1=1 and to_storeroom in (
select unit_id from user_privilege 
where user_id = @p_user_id and unit_root_id = @p_hospital_id and operate=1)";

            int formNo;
            if (!string.IsNullOrEmpty(condition.Content) && int.TryParse(condition.Content, out formNo))
            {
                conditionSql += " and form_no = @p_form_no";
            }

            if (condition.BeginDate.HasValue)
            {
                conditionSql += " AND requested_time >= @p_begin_date";
            }

            if (condition.EndDate.HasValue)
            {
                conditionSql += " AND requested_time <= @p_end_date";
            }

            return conditionSql;
        }

        private void AddParameter(DbCommand dc, DateRangeCondition condition)
        {
            Db.AddInParameter(dc, "p_user_id", DbType.String, condition.UserId);
            Db.AddInParameter(dc, "p_hospital_id", DbType.String, condition.HospitalId);

            int formNo;
            if (!string.IsNullOrEmpty(condition.Content) && int.TryParse(condition.Content, out formNo))
            {
                Db.AddInParameter(dc, "p_form_no", DbType.Int32, formNo);
            }

            if (condition.BeginDate.HasValue)
            {
                Db.AddInParameter(dc, "p_begin_date", DbType.DateTime, condition.BeginDate);
            }

            if (condition.EndDate.HasValue)
            {
                Db.AddInParameter(dc, "p_end_date", DbType.DateTime, condition.EndDate);
            }
        }

        public List<MoveFormItemEntity> FindAllItems(string moveID)
        {
            var sql = string.Format("SELECT {0} FROM move_form_items WHERE move_id = @p_move_id", ITEM_COLUMNS);
            
            var dc = Db.GetSqlStringCommand(sql);
            Db.AddInParameter(dc, "p_move_id", DbType.String, moveID);

            var list = new List<MoveFormItemEntity>();
            using (var reader = Db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var item = new MoveFormItemEntity();
                    item.Init(reader);

                    list.Add(item);
                }
            }

            return list;
        }

        public List<MoveFormSerialEntity> FindAllSerials(string moveID)
        {
            var sql = string.Format("SELECT {0} FROM move_form_serial WHERE move_id = @p_move_id", SERIAL_COLUMNS);

            var dc = Db.GetSqlStringCommand(sql);
            Db.AddInParameter(dc, "p_move_id", DbType.String, moveID);

            var list = new List<MoveFormSerialEntity>();
            using (var reader = Db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var item = new MoveFormSerialEntity();
                    item.Init(reader);

                    list.Add(item);
                }
            }

            return list;
        }
        #endregion

        public void DeleteItem(string hospitalId, string moveId, string productId)
        {
            var sql = @"
delete goods_runtime from goods_runtime r, move_form_serial s
where r.apply_id = s.move_id
and r.serial_id = s.to_serial
and r.hospital_id = @p_hospital_id
and s.move_id = @p_move_id
and s.product_id = @p_product_id;

delete goods_inventory_runtime 
where apply_id = @p_move_id 
and hospital_id = @p_hospital_id 
and product_id = @p_product_id;

delete move_form_serial
where move_id = @p_move_id
and product_id = @p_product_id;

delete move_form_items
where move_id = @p_move_id
and product_id = @p_product_id;
";
            var cmd = Db.GetSqlStringCommand(sql);
            Db.AddInParameter(cmd, "p_move_id", DbType.String, moveId);
            Db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);
            Db.AddInParameter(cmd, "p_product_id", DbType.String, productId);

            Db.ExecuteNonQuery(cmd, DbTrans);
        }

        public void CommitMovein(string hospitalId, string userId, string moveId)
        {
            var sql = @"
declare @toStoreroomId varchar(100)

select @toStoreroomId = to_storeroom from move_form where id = @p_move_id

update gsb 
set gsb.[out] = 1
from goods_serial_barcodes gsb, move_form_serial mfs, goods_runtime gr
where gsb.out = 0
and gsb.serial_id = mfs.from_serial
and gsb.barcode = gr.barcode
and gr.hospital_id = @p_hospital_id
and gr.serial_id = mfs.to_serial
and gr.apply_id = mfs.move_id
and mfs.move_id = @p_move_id;

insert into goods_serial_barcodes(id, serial_id, barcode, is_printed, out)
select newid(), mfs.to_serial, gr.barcode, gsb.is_printed, 0
from goods_serial_barcodes gsb, move_form_serial mfs, goods_runtime gr 
where gsb.out = 1
and gsb.serial_id = mfs.from_serial
and gsb.barcode = gr.barcode
and gr.hospital_id = @p_hospital_id
and gr.serial_id = mfs.to_serial
and gr.apply_id = mfs.move_id
and mfs.move_id = @p_move_id;

insert into goods_serial(id, serial_no, parent_id, product_id, dispatched_count, hospital_id, vendor_id,
need_audit, need_check, need_split, split_copies, split_unit, split_capacity, split_package_count, valid_days, batch_no, expired_date,
logistics_code, logistics_content, is_closed, created_id, created_time, updated_id, updated_time)
select mfs.to_serial, gs.serial_no, null, gs.product_id, mfs.count, gs.hospital_id, gs.vendor_id,
gs.need_audit, gs.need_check, gs.need_split, gs.split_copies, gs.split_unit, gs.split_capacity, gs.split_package_count, gs.valid_days, gs.batch_no, gs.expired_date,
gs.logistics_code, gs.logistics_content, gs.is_closed, @p_user_id, getdate(), null, null 
from goods_serial gs, move_form_serial mfs
where gs.id = mfs.from_serial
and gs.hospital_id = @p_hospital_id
and mfs.move_id = @p_move_id;

update g
set g.serial_id = mfs.to_serial, g.storeroom_id = @toStoreroomId
from goods g, move_form_serial mfs, goods_runtime gr
where g.serial_id = mfs.from_serial
and g.barcode = gr.barcode
and g.hospital_id = gr.hospital_id
and gr.hospital_id = @p_hospital_id
and gr.serial_id = mfs.to_serial
and gr.apply_id = mfs.move_id
and mfs.move_id = @p_move_id;

update gi
set gi.usable_count = gi.usable_count - mfs.count,
	gi.apply_count = gi.apply_count - mfs.count * mfs.package_count
from goods_inventory gi, move_form_serial mfs
where gi.serial_id = mfs.from_serial
and gi.product_id = mfs.product_id
and gi.hospital_id = @p_hospital_id
and mfs.move_id = @p_move_id;


insert into goods_inventory(id, serial_id, batch_no, product_id, storeroom_id, expired_date, hospital_id, vendor_id,
original_count, split_count, usable_count, apply_count, granted_count, created_id, created_time)
select newid(), mfs.to_serial, gi.batch_no, gi.product_id, @toStoreroomId, gi.expired_date, gi.hospital_id, gi.vendor_id,
mfs.count, gi.split_count, mfs.count, mfs.count * mfs.package_count,  0, @p_user_id, getdate() 
from goods_inventory gi, move_form_serial mfs
where gi.serial_id = mfs.from_serial
and gi.product_id = mfs.product_id
and gi.hospital_id = @p_hospital_id
and mfs.move_id = @p_move_id;

delete goods_runtime 
where hospital_id = @p_hospital_id
and apply_id = @p_move_id;

delete goods_inventory_runtime
where hospital_id = @p_hospital_id
and apply_id = @p_move_id;

insert into goods_serial_form(serial_id, form_id, form_kind, created_id, created_time)
select to_serial, move_id, 'moved_in', @p_user_id, getdate()
from move_form_serial
where move_id = @p_move_id;

insert into goods_serial_forms(serial_id, form_id, form_kind, created_id, created_time)
select to_serial, move_id, 'moved_in', @p_user_id, getdate()
from move_form_serial
where move_id = @p_move_id;

";
            var cmd = Db.GetSqlStringCommand(sql);
            Db.AddInParameter(cmd, "p_move_id", DbType.String, moveId);
            Db.AddInParameter(cmd, "p_hospital_id", DbType.String, hospitalId);
            Db.AddInParameter(cmd, "p_user_id", DbType.String, userId);

            Db.ExecuteNonQuery(cmd, DbTrans);
        }
    }
}
