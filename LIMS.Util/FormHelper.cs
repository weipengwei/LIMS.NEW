using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Util
{
    public static class FormHelper
    {
        private static IDictionary<FormType, string> ms_FormList = new Dictionary<FormType, string>();
        private static IDictionary<FormType, string> ms_AuditForms = new Dictionary<FormType, string>();

        static FormHelper()
        {
            ms_FormList[FormType.Apply] = "申请";
            ms_FormList[FormType.Dispatch] = "发货";
            ms_FormList[FormType.Incoming] = "入库";
            ms_FormList[FormType.Inspection] = "验货";
            ms_FormList[FormType.InspectionAudit] = "验货审核";
            ms_FormList[FormType.OrderAudit] = "订货审核";
            ms_FormList[FormType.Receive] = "收货";
            ms_FormList[FormType.Return] = "退货";
            ms_FormList[FormType.ReturnReceive] = "退货接收";
            ms_FormList[FormType.Splitting] = "分装";
            ms_FormList[FormType.MoveAudit] = "移库审核";
        }

        public static string FormName(FormType formType)
        {
            string name;
            if(ms_FormList.TryGetValue(formType, out name))
            {
                return name;
            }

            return string.Empty;
        }

        public static IDictionary<FormType, string> AuditForms()
        {
            ms_AuditForms[FormType.OrderAudit] = FormName(FormType.OrderAudit);
            ms_AuditForms[FormType.InspectionAudit] = FormName(FormType.InspectionAudit);
            ms_AuditForms[FormType.MoveAudit] = FormName(FormType.MoveAudit);

            return ms_AuditForms;
        }
    }
}
