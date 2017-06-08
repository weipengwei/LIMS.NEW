using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Util;
using LIMS.Entities;
using LIMS.Repositories;

namespace LIMS.Services
{
    public class GoodsSerialFormService
    {
        public string GetFormMessage(GoodsSerialFormEntity form)
        {
            if(string.Compare(form.FormKind, FormKind.DispatchItem, true) == 0)
            {
                return "货品正在发货阶段！";
            }
            else if(string.Compare(form.FormKind, FormKind.Receive, true) == 0)
            {
                return "货品正在收货阶段！";
            }
            else if (string.Compare(form.FormKind, FormKind.ReceiveItem, true) == 0)
            {
                return "货品正在收货阶段！";
            }
            else if (string.Compare(form.FormKind, FormKind.Inspection, true) == 0)
            {
                return "货品正在收货审核阶段！";
            }
            else if(string.Compare(form.FormKind, FormKind.Incoming, true) == 0)
            {
                return "货品正在入库阶段！";
            }
            else
            {
                return "货品处在未知阶段！";
            }
        }

        public GoodsSerialFormEntity GetBySerialId(string serialId)
        {
            return GoodsSerialFormRepository.GetBySerialId(serialId);
        }
    }
}
