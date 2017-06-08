using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Models;
using LIMS.Repositories;
using LIMS.Entities;

namespace LIMS.Services
{
    public class FormApproversService
    {
        public IList<FormApproverEntity> GetDetailApprovers(string formId, int formType)
        {
            return FormApproversRepository.GetApprovers(formId, formType);
        }

        public IList<FormApproverEntity> Query(AuditQueryCondition condition, PagerInfo pager)
        {
            return FormApproversRepository.Query(condition, pager);
        }

        public FormApproverEntity Get(string id)
        {
            return FormApproversRepository.Get(id);
        }

        public void Approve(string id, bool yesOrNo, string remark)
        {
            FormApproversRepository.Approve(id, yesOrNo, remark);
        }
    }
}
