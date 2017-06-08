using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

using LIMS.Util;
using LIMS.Entities;

namespace LIMS.Repositories
{
    public static class ReceiveIncomingFormRepository
    {
        #region Get
        public static ReceiveFormEntity Get(string id)
        {
            return ReceiveFormRepository.Get(id, null, null);
        }
        #endregion


        public static ReceiveFormEntity GetBySerialId(string serialId)
        {
            return ReceiveFormRepository.GetBySerialId(serialId);
        }

        #region Pass
        public static void Pass(ReceiveFormEntity form)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {


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



        #region PartPass 
        public static string PartPass(ReceiveFormEntity form, IList<string> barcodes, string userId)
        {
            var newSerialId = form.SerialId;
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        newSerialId=ReceiveFormRepository.PartPass(form, barcodes, userId, db, trans);
                        var incomingService = new IncomingFormEntity
                        IncomingFormEntity formIncoming = incomingService.GetBySerialId(newSerialId);
                        formIncoming.ConfirmedTime = DateTime.Now;
                        incomingService.Pass(formIncoming);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            return serialId;
        }
        #endregion
    }
}
