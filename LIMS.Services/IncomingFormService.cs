using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.SqlCe;

using LIMS.Entities;
using LIMS.Repositories;

namespace LIMS.Services
{
    public class IncomingFormService
    {
        public IncomingFormEntity Get(string id)
        {
            return IncomingFormRepository.Get(id);
        }

        public IncomingFormEntity GetBySerialId(string serialId, Database db, DbTransaction trans)
        {
            return IncomingFormRepository.GetBySerialId(serialId, db, trans);
        }

        public IncomingFormEntity GetBySerialId(string serialId)
        {
            return IncomingFormRepository.GetBySerialId(serialId);
        }

        public void Pass(IncomingFormEntity form, Database db, DbTransaction trans)
        {
            IncomingFormRepository.Pass(form, db, trans);
        }

        public void Pass(IncomingFormEntity form)
        {
            IncomingFormRepository.Pass(form);
        }

        //public IList<IncomingFormEntity> QueryConfirm(string hospitalId)
        //{
        //    return IncomingFormRepository.QueryConfirm(hospitalId);
        //}

        //public void Confirm(string id, string storeroomId, string userId)
        //{
        //    IncomingFormRepository.Confirm(id, storeroomId, userId);
        //}
    }
}
