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
    public static class IdentityCreatorRepository
    {
        private const string COLUMNS = "identity_key,dimension,seed,step";
        private static object m_Lock = new object();

        public static int Get(string key, int count)
        {
            return Get(key, "*", count);
        }

        public static int Get(string key, string dimension, int count)
        {
            var db = DatabaseFactory.CreateDatabase();

            int identity = -1;
            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {

                        var entity = GetCurrentSeed(key, dimension, db, trans);
                        identity = entity.Seed;

                        var newSeed = entity.Seed + count * entity.Step;
                        UpdateSeed(key, dimension, newSeed, db, trans);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return identity;
        }

        private static IdentityCreatorEntity GetCurrentSeed(string key, string dimension, Database db, DbTransaction trans)
        {
            var sql = string.Format("SELECT * FROM identity_creator WITH (HOLDLOCK, UPDLOCK) WHERE identity_key=@p_key and dimension=@p_dimension", COLUMNS);
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_key", DbType.String, key);
            db.AddInParameter(dc, "p_dimension", DbType.String, dimension);

            using (var reader = db.ExecuteReader(dc, trans))
            {
                if(reader.Read())
                {
                    var entity = new IdentityCreatorEntity();
                    entity.Init(reader);

                    return entity;
                }
            }

            return Create(key, dimension, db, trans);
        }

        private static IdentityCreatorEntity Create(string key, string dimension, Database db, DbTransaction trans)
        {
            var entity = new IdentityCreatorEntity
            {
                IdentityKey = key,
                Dimension = dimension,
                Seed = 1,
                Step = 1
            };

            var sql = string.Format("insert into identity_creator({0}) values(@p_identity_key,@p_dimension,@p_seed,@p_step)", COLUMNS);
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_identity_key", DbType.String, entity.IdentityKey);
            db.AddInParameter(dc, "p_dimension", DbType.String, entity.Dimension);
            db.AddInParameter(dc, "p_seed", DbType.String, entity.Seed);
            db.AddInParameter(dc, "p_step", DbType.String, entity.Step);

            db.ExecuteNonQuery(dc, trans);

            return entity;
        }

        private static void UpdateSeed(string key, string dimension, int newSeed, Database db, DbTransaction trans)
        {
            var sql = "UPDATE identity_creator SET seed=@p_seed WHERE identity_key=@p_key and dimension=@p_dimension";
            var dc = db.GetSqlStringCommand(sql);
            db.AddInParameter(dc, "p_key", DbType.String, key);
            db.AddInParameter(dc, "p_dimension", DbType.String, dimension);
            db.AddInParameter(dc, "p_seed", DbType.Int32, newSeed); 

            db.ExecuteNonQuery(dc, trans);
        }
    }
}
