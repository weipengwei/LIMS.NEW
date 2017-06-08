using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace LIMS.Repositories
{
    public class RepositoryBase
    {
        [ThreadStatic]
        static Database _Db;

        [ThreadStatic]
        static DbTransaction _DbTrans;

        [ThreadStatic]
        static int _Count;

        public Database Db
        { get{ return _Db;}}

        public DbTransaction DbTrans
        { get { return _DbTrans; } }

        protected RepositoryBase()
        {
            NewTrans();
        }

        public static void ClearTrans()
        {
            if (_DbTrans != null)
            {
                if (_DbTrans.Connection != null)
                {
                    _DbTrans.Connection.Close();
                    _DbTrans.Connection.Dispose();
                }

                _DbTrans.Dispose();
            }

            _DbTrans = null;
            _Count = 0;
        }

        public void NewTrans()
        {
            if (_Db == null)
                _Db = DatabaseFactory.CreateDatabase();

            if (_DbTrans == null)
            {
                DbConnection conn = _Db.CreateConnection();
                conn.Open();
                _DbTrans = conn.BeginTransaction();
            }

            _Count += 1;
        }

        public void CommitTrans()
        {
            ReleaseTrans(true);
            _Count -= 1;
        }

        public void RollbackTrans()
        {
            ReleaseTrans(false);
            _Count -= 1;
        }

        public static void ReleaseTrans(bool commit)
        {
            ReleaseTrans(commit, false);
        }

        public static void ReleaseTrans(bool commit, bool force)
        {
            if (_Count != 1 && !force) return;

            if (_DbTrans != null)
            {
                try
                {
                    if (commit)
                        _DbTrans.Commit();
                    else
                        _DbTrans.Rollback();
                }
                catch { }

                if (_DbTrans.Connection != null)
                {
                    _DbTrans.Connection.Close();
                    _DbTrans.Connection.Dispose();
                }

                _DbTrans.Dispose();
            }

            _DbTrans = null;
        }
    }
}
