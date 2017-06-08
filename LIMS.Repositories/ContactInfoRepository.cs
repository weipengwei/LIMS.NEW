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
    public static class ContactInfoRepository
    {
        public static string COLUMN_SQL = @"id, vestee_id, contact_person, address, 
contact_way_1, contact_way_2, contact_way_3, contact_way_4,
created_id, created_time, updated_id, updated_time";

        public static void Add(ContactInfoEntity contactInfo, Database db, DbTransaction trans)
        {
            var sql = string.Format(@"INSERT INTO contact_info({0})
VALUES(@p_id, @p_vestee_id, @p_contact_person, @p_address,
@p_contact_way_1, @p_contact_way_2, @p_contact_way_3, @p_contact_way_4,
@p_created_id, @p_created_time, @p_updated_id, @p_updated_time)", COLUMN_SQL);
            
            DbCommand dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, contactInfo.Id);
            db.AddInParameter(dc, "p_vestee_id", DbType.String, contactInfo.VesteeId);
            db.AddInParameter(dc, "p_contact_person", DbType.String, contactInfo.ContactPerson);
            db.AddInParameter(dc, "p_address", DbType.String, contactInfo.Address);
            db.AddInParameter(dc, "p_contact_way_1", DbType.String, contactInfo.ContactWay1);
            db.AddInParameter(dc, "p_contact_way_2", DbType.String, contactInfo.ContactWay2);
            db.AddInParameter(dc, "p_contact_way_3", DbType.String, contactInfo.ContactWay3);
            db.AddInParameter(dc, "p_contact_way_4", DbType.String, contactInfo.ContactWay4);
            db.AddInParameter(dc, "p_created_id", DbType.String, contactInfo.CreatedId);
            db.AddInParameter(dc, "p_created_time", DbType.DateTime, contactInfo.CreatedTime);
            db.AddInParameter(dc, "p_updated_id", DbType.String, contactInfo.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, contactInfo.UpdatedTime);

            db.ExecuteNonQuery(dc, trans);
        }

        public static void Update(ContactInfoEntity contactInfo, Database db, DbTransaction trans)
        {
            var sql = @"UPDATE contact_info
SET contact_person = @p_contact_person, address = @p_address, vestee_id = @p_vestee_id,
    contact_way_1 = @p_contact_way_1, contact_way_2 = @p_contact_way_2, 
    contact_way_3 = @p_contact_way_3, contact_way_4 = @p_contact_way_4,
    updated_id = @p_updated_id, updated_time = @p_updated_time
WHERE id=@p_id";

            DbCommand dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_contact_person", DbType.String, contactInfo.ContactPerson);
            db.AddInParameter(dc, "p_address", DbType.String, contactInfo.Address);
            db.AddInParameter(dc, "p_vestee_id", DbType.String, contactInfo.VesteeId);
            db.AddInParameter(dc, "p_contact_way_1", DbType.String, contactInfo.ContactWay1);
            db.AddInParameter(dc, "p_contact_way_2", DbType.String, contactInfo.ContactWay2);
            db.AddInParameter(dc, "p_contact_way_3", DbType.String, contactInfo.ContactWay3);
            db.AddInParameter(dc, "p_contact_way_4", DbType.String, contactInfo.ContactWay4);
            db.AddInParameter(dc, "p_updated_id", DbType.String, contactInfo.UpdatedId);
            db.AddInParameter(dc, "p_updated_time", DbType.DateTime, contactInfo.UpdatedTime);
            db.AddInParameter(dc, "p_id", DbType.String, contactInfo.Id);

            db.ExecuteNonQuery(dc, trans);
        }

        public static ContactInfoEntity Get(string id)
        {
            var sql = string.Format("SELECT {0} FROM contact_info WHERE id=@p_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_id", DbType.String, id);

            using (IDataReader reader = db.ExecuteReader(dc))
            {
                if(reader.Read())
                {
                    var entity = new ContactInfoEntity();
                    entity.Init(reader);

                    return entity;
                }
                else
                {
                    return null;
                }
            }
        }

        public static IList<ContactInfoEntity> GetByVesteeId(string vesteeId)
        {
            var sql = string.Format("SELECT {0} FROM contact_info WHERE vestee_id=@p_vestee_id", COLUMN_SQL);

            var db = DatabaseFactory.CreateDatabase();
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_vestee_id", DbType.String, vesteeId);

            var list = new List<ContactInfoEntity>();
            using (IDataReader reader = db.ExecuteReader(dc))
            {
                while (reader.Read())
                {
                    var entity = new ContactInfoEntity();
                    entity.Init(reader);

                    list.Add(entity);
                }
            }

            return list;
        }
    }
}
