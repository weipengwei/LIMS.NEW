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
    public static class FormApproversRepository
    {
        private const string COLUMN_SQL = @"id,hospital_id,form_type,form_id,form_no,approver_id,approved_time,status,sequence,remark";

        public static void CreateApprovers(string formId, int formNo, FormType formType, string hospitalId, string applyerId, string applyUnitId)
        {
            var approveList = FormApproveListRepository.Get(formType);

            var users = new List<string>();
            foreach (var item in approveList)
            {
                if (item.ApproverType == ApproverType.Applyer)
                {
                    users.Add(applyerId);
                }
                else if (item.ApproverType == ApproverType.ApplyUnitManager || item.ApproverType == ApproverType.ChooseUnitManager)
                {
                    var managers = UserRepository.GetManagers(item.ApproverId);
                    foreach (var manager in managers)
                    {
                        users.Add(manager.Id);
                    }
                }
                else if (item.ApproverType == ApproverType.ChoosePerson)
                {
                    users.Add(item.ApproverId);
                }
            }

            var approvers = new List<FormApproverEntity>();
            var sequence = SequenceHelper.TimeSequence();
            foreach (var id in users)
            {
                approvers.Add(new FormApproverEntity
                {
                    HospitalId = hospitalId,
                    FormType = (int)formType,
                    FormId = formId,
                    FormNo = formNo,
                    ApproverId = id,
                    Sequence = sequence
                });
                sequence++;
            }

            CreateApprovers(approvers);
        }
        
        private static void CreateApprovers(IList<FormApproverEntity> approvers)
        {
            var sql = string.Format(@"insert into form_approvers(
{0}
) values(
@p_id, @p_hospital_id, @p_form_type, @p_form_id, @p_form_no, @p_approver_id, @p_approved_time, @p_status, @p_sequence, @p_remark
)", COLUMN_SQL);

            var index = 1;
            var dic = new Dictionary<string, string>();
            
            var db = DatabaseFactory.CreateDatabase();
            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var approver in approvers)
                        {
                            if (dic.Keys.Contains(approver.ApproverId))
                            {
                                continue;
                            }

                            dic[approver.ApproverId] = approver.ApproverId;

                            var dc = db.GetSqlStringCommand(sql);
                            db.AddInParameter(dc, "p_id", DbType.String, Guid.NewGuid().ToString());
                            db.AddInParameter(dc, "p_hospital_id", DbType.String, approver.HospitalId);
                            db.AddInParameter(dc, "p_form_type", DbType.String, approver.FormType);
                            db.AddInParameter(dc, "p_form_id", DbType.String, approver.FormId);
                            db.AddInParameter(dc, "p_form_no", DbType.Int32, approver.FormNo);
                            db.AddInParameter(dc, "p_approver_id", DbType.String, approver.ApproverId);
                            db.AddInParameter(dc, "p_approved_time", DbType.DateTime, DBNull.Value);
                            if (index == 1)
                            {
                                db.AddInParameter(dc, "p_status", DbType.Int16, FormAuditStatus.Approving);
                            }
                            else
                            {
                                db.AddInParameter(dc, "p_status", DbType.Int16, FormAuditStatus.Waiting);
                            }
                            db.AddInParameter(dc, "p_sequence", DbType.Int64, approver.Sequence);
                            db.AddInParameter(dc, "p_remark", DbType.String, "");

                            index++;

                            db.ExecuteNonQuery(dc, trans);
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
        }

        public static IList<FormApproverEntity> GetApprovers(string formId, int formType)
        {
            var sql = string.Format(@"select {0} from form_approvers 
where form_id=@p_form_id and form_type=@p_form_type
order by sequence", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);

            db.AddInParameter(dc, "p_form_id", DbType.String, formId);
            db.AddInParameter(dc, "p_form_type", DbType.Int32, formType);

            var list = new List<FormApproverEntity>();
            using (var reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    var approver = new FormApproverEntity();
                    approver.Init(reader);

                    list.Add(approver);
                }
            }

            return list;
        }

        public static FormApproverEntity Get(string id)
        {
            var sql = string.Format("select {0} from form_approvers where id=@p_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);

            using (var reader = db.ExecuteReader(dc))
            {
                while(reader.Read())
                {
                    var entity = new FormApproverEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return null;
        }

        public static void Approve(string id, bool yesOrNo, string remark)
        {
            var entity = Get(id);
            if(entity == null)
            {
                throw new Exception("The approver does not exist.");
            }

            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var sql = "update form_approvers set status=@p_status, remark=@p_remark, approved_time=@p_approved_time where id=@p_id";
                        var dc = db.GetSqlStringCommand(sql);
                        
                        db.AddInParameter(dc, "p_id", DbType.String, id);
                        db.AddInParameter(dc, "p_status", DbType.Int32, yesOrNo ? FormAuditStatus.Approved : FormAuditStatus.Reject);
                        db.AddInParameter(dc, "p_remark", DbType.String, remark);
                        db.AddInParameter(dc, "p_approved_time", DbType.DateTime, DateTime.Now);
                        db.ExecuteNonQuery(dc, trans);

                        //if(entity.FormType == (int)FormType.OrderAudit)
                        //{
                        //    if (yesOrNo)
                        //    {
                        //        if(IsAllAgreed(entity.FormId, entity.FormType))
                        //        {
                        //            OrderFormRepository.UpdateDetailStatus(entity.FormId, OrderDetailStatus.WAITING_SEND, db, trans);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        OrderFormRepository.UpdateDetailStatus(entity.FormId, OrderDetailStatus.REJECT, db, trans);
                        //    }
                        //}

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

        private static  bool IsAllAgreed(string formId, int formType)
        {
            var approvers = GetApprovers(formId, formType);

            var result = true;
            foreach(var item in approvers)
            {
                if(item.Status != FormAuditStatus.Approved)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
        

        #region Page Query
        public static IList<FormApproverEntity> Query(AuditQueryCondition condition, PagerInfo pager)
        {
            pager.ComputePageCount(QueryCount(condition));

            var list = new List<FormApproverEntity>();

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
                orderSql += "form_no";
            }

            var sql = string.Format(@"SELECT {0} FROM form_approvers WHERE 1=1{1}", COLUMN_SQL, GetConditionSql(condition));

            sql = @"SELECT * FROM
            (
                SELECT ROW_NUMBER() OVER(" + orderSql + @") pid," + COLUMN_SQL + @"
                FROM (" + sql + @") t            
            ) t1 WHERE t1.pid BETWEEN @p_pageNo * @p_pageSize + 1 AND (@p_pageNo + 1) * @p_pageSize ";

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            AddParameters(db, dc, condition);

            db.AddInParameter(dc, "p_pageNo", DbType.Int32, pager.PageIndex);
            db.AddInParameter(dc, "p_pageSize", DbType.Int32, pager.PageSize);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new FormApproverEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }

        private static int QueryCount(AuditQueryCondition condition)
        {
            var sql = string.Format(@"SELECT COUNT(id) FROM form_approvers WHERE 1=1{1}", COLUMN_SQL, GetConditionSql(condition));

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            AddParameters(db, dc, condition);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                reader.Read();

                return reader.GetInt32(0);
            }
        }

        private static void AddParameters(Database db, DbCommand dc, AuditQueryCondition condition)
        {
            db.AddInParameter(dc, "p_approver_id", DbType.String, condition.UserId);
            db.AddInParameter(dc, "p_hospital_id", DbType.String, condition.HospitalId);
            db.AddInParameter(dc, "p_status", DbType.String, condition.Status);

            if(condition.FormNo.HasValue)
            {
                db.AddInParameter(dc, "p_form_no", DbType.String, condition.FormNo);
            }
        }

        private static string GetConditionSql(AuditQueryCondition condition)
        {
            var conditionSql = @" and hospital_id = @p_hospital_id and approver_id = @p_approver_id 
and status = @p_status";

           if(condition.FormNo.HasValue)
            {
                conditionSql += " and form_no = @p_form_no";
            }
            
            return conditionSql;
        }
        #endregion
    }
}
 