using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Util
{
    public static class OrderFormStatus
    {
        public static readonly string Saved = "saved";
        public static readonly string Waiting = "waiting";
        public static readonly string Dispatching = "dispatching";
        public static readonly string Deleted = "deleted";
        public static readonly string Complete = "complete";

        public static string GetName(string status)
        {
            if (string.Compare(status, Waiting, true) == 0)
            {
                return "待发放";
            }
            if (string.Compare(status, Dispatching, true) == 0)
            {
                return "发放中";
            }
            else if (string.Compare(status, Saved, true) == 0)
            {
                return "暂存";
            }
            else if (string.Compare(status, Deleted, true) == 0)
            {
                return "已删除";
            }
            if (string.Compare(status, Complete, true) == 0)
            {
                return "已完成";
            }
            else
            {
                return "";
            }
        }
    }

    public static class OrderFormItemStatus
    {
        public static readonly string Saved = "saved";
        public static readonly string Waiting = "waiting";
        public static readonly string Dispatching = "dispatching";
        public static readonly string Dispatched = "dispatched";
        public static readonly string Auditing = "auditing";
        public static readonly string Complete = "complete";

        public static string GetName(string status)
        {
            if (string.Compare(status, Waiting, true) == 0)
            {
                return "待发放";
            }
            if (string.Compare(status, Dispatching, true) == 0)
            {
                return "发放中";
            }
            else if (string.Compare(status, Saved, true) == 0)
            {
                return "暂存";
            }
            if (string.Compare(status, Auditing, true) == 0)
            {
                return "审核中";
            }
            if (string.Compare(status, Dispatched, true) == 0)
            {
                return "已发放";
            }
            if (string.Compare(status, Complete, true) == 0)
            {
                return "已完成";
            }
            else
            {
                return "";
            }
        }
    }

    public static class DispatchFormStatus
    {
        public static readonly string Waiting = "waiting";
        public static readonly string Dispatching = "dispatching";
        public static readonly string Confirmed = "confirmed";
        public static readonly string Cancelled = "cancelled";

        public static readonly string QueryDispatchable = "dispatchable";

        public static string GetName(string status)
        {
            if(string.Compare(status, Waiting, true) == 0)
            {
                return "待发放";
            }
            if(string.Compare(status, Dispatching, true) == 0)
            {
                return "发放中";
            }
            else if(string.Compare(status, Confirmed, true) == 0)
            {
                return "已确认";
            }
            else if(string.Compare(status, Cancelled, true) == 0)
            {
                return "已取消";
            }
            else
            {
                return "";
            }
        }
    }

    public static class ApplyFormStatus
    {
        public static readonly string Saved = "saved";
        public static readonly string Granting = "granting";
        public static readonly string Granted = "granted";
        public static readonly string Cancelled = "cancelled";

        public static string GetName(string status)
        {
            if (string.Compare(status, Saved, true) == 0)
            {
                return "保存";
            }
            if (string.Compare(status, Granting, true) == 0)
            {
                return "发放中";
            }
            else if (string.Compare(status, Granted, true) == 0)
            {
                return "已发放";
            }
            else if (string.Compare(status, Cancelled, true) == 0)
            {
                return "已取消";
            }
            else
            {
                return "";
            }
        }
    }

    public static class CheckFormStatus
    {
        public static readonly string Saved = "saved";
        public static readonly string Checking = "checking";
        public static readonly string Complete = "complete";

        public static string GetName(string status)
        {
            if(string.Compare(status, Saved, true) == 0)
            {
                return "保存";
            }
            else if(string.Compare(status, Checking, true) == 0)
            {
                return "盘查";
            }
            else if(string.Compare(status, Complete, true) == 0)
            {
                return "结束";
            }
            else
            {
                return string.Empty;
            }
        }
    }

    public class CheckProductStatus
    {
        public static readonly string NoCheck = "no_check";
        public static readonly string NoMatch = "no_match";
        public static readonly string Match = "match";
        public static readonly string Handle = "handle";

        public static string GetName(string status)
        {
            if(string.Compare(status, NoCheck, true) == 0)
            {
                return "未盘查";
            }
            else if (string.Compare(status, NoMatch, true) == 0)
            {
                return "异常";
            }
            else if (string.Compare(status, Handle, true) == 0)
            {
                return "处理";
            }
            else
            {
                return "正常";
            }
        }
    }

    public class CheckAdjustType
    {
        public static readonly string Ignore = "ignore";
        public static readonly string Override = "override";
    }

    public class CheckProductValidationStatus
    {
        public static readonly string Init = "init";
        public static readonly string Match = "match";
        public static readonly string Storeroom = "storeroom";
        public static readonly string Count = "count";
        public static readonly string NoExist = "no_exist";
        public static readonly string InSystem = "in_system";

        public static string GetInvalidDesc(string status)
        {
            if(string.Compare(status, Storeroom, true) == 0)
            {
                return "货品所在库房不是盘点的库房！";
            }
            else if(string.Compare(status, Count, true) == 0)
            {
                return "货品库存和盘查数量不一致！";
            }
            else if(string.Compare(status, NoExist, true) == 0)
            {
                return "货品库存不存在！";
            }
            else if (string.Compare(status, InSystem, true) == 0)
            {
                return "货品只存在系统中！";
            }
            else
            {
                return "正常";
            }
        }
    }

    public static class MoveFormStatus
    {
        public static readonly string Saved = "saved";
        public static readonly string Movedout = "movedout";
        public static readonly string Movedin = "movedin";
        public static readonly string Deleted = "deleted";

        public static string GetName(string status)
        {
            if (string.Compare(status, Saved, true) == 0)
            {
                return "暂存";
            }
            if (string.Compare(status, Movedout, true) == 0)
            {
                return "已出库";
            }
            else if (string.Compare(status, Movedin, true) == 0)
            {
                return "已入库";
            }
            else if (string.Compare(status, Deleted, true) == 0)
            {
                return "已删除";
            }
            else
            {
                return "";
            }
        }
    }
}
