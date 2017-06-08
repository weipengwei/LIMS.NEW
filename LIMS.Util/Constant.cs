using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Util
{

    public enum DisplayModeType { Visibility = 0, Hidden = 1 }

    public enum UnitType { None = 0, Hospital = 10, Vendor = 20, HospitalUnit = 30, VendorUnit = 40 }

    public enum UnitBusinessType { None = 0, Business = 1, Storeroom = 2 }

    public enum FormType
    {
        Failed = -1,
        None = 0,
        Dispatch = 1,
        Receive = 2,
        Inspection = 3,
        Incoming = 4,
        Apply = 5,
        Splitting = 6,
        Return = 7,
        InspectionAudit = 8,
        ReturnReceive = 9,
        OrderAudit = 10,
        MoveAudit = 11,
        MoveOut = 12,
        MoveIn = 13,
        Order = 14,
        SalerDispatch = 15,
        Out = 16
    }

    public enum ApproverType
    {
        None = 0,
        Applyer = 1,
        ApplyUnitManager = 2,
        ChoosePerson = 3,
        ChooseUnitManager = 4
    }

    public enum UserTitle
    {
        None = 0,
        Normal = 1,
        Manager = 2
    }

    public static class Constant
    {
        public const string ADMIN_ID = "00000000-0000-0000-0000-000000000000";
        public const string ADMIN_MENU_ID = "BD6E5F1C-DBC9-4C20-896B-E171300BD98E";
        public const string SYSTEM_NAME = "LIMS";
        public const string DEFAULT_UNIT_ROOT_ID = "0";
        public const string CURRENT_HOSPITAL_COOKIE = "CurrentHospital";

        public const string NO_EXIST_PRODUCT = "产品不存在";
        public const string QUERY_NO_PRODUCT = "产品名为空";
    }

    public static class IdentityKey
    {
        public const string ORDER_FORM = "ORDER_FORM";
        public const string GOODS_BARCODE = "GOODS_BARCODE";
        public const string APPLY_FORM = "APPLY_FORM";
        public const string MOVEOUT_FORM = "MOVEOUT_FORM";
        public const string PRODUCT_BARCODE = "PRODUCT_BARCODE";
    }

    public static class OrderStatus
    {
        public const int Editable = 0;
        public const int View = 1;
        public const int Delete = 2;
    }

    public static class FormAuditStatus
    {
        public const int Waiting = 0;
        public const int Approving = 1;
        public const int Approved = 2;
        public const int Reject = 3;

        private static IDictionary<int, string> ms_StatusName = new Dictionary<int, string>();

        static FormAuditStatus()
        {
            ms_StatusName[(int)Waiting] = "待签核";
            ms_StatusName[(int)Approving] = "签核中";
            ms_StatusName[(int)Approved] = "已签核";
            ms_StatusName[(int)Reject] = "已拒绝";
        }

        public static string GetStatusName(int status)
        {
            string name;
            if (ms_StatusName.TryGetValue(status, out name))
            {
                return name;
            }

            throw new Exception("The audit status does not exist.");
        }
    }

    public enum GoodsStatus
    {
        None = 0,
        Usable = 1,
        Closed = 5
    }

    public static class BarcodeStatusName
    {
        private static IDictionary<int, string> ms_StatusName = new Dictionary<int, string>();
        static BarcodeStatusName()
        {
            ms_StatusName[(int)FormType.Failed] = "作废";
            ms_StatusName[(int)FormType.Apply] = "领用";
            ms_StatusName[(int)FormType.Out] = "发放";
            ms_StatusName[(int)FormType.Dispatch] = "供应商发货";
            ms_StatusName[(int)FormType.Incoming] = "入库";
            ms_StatusName[(int)FormType.Inspection] = "检验";
            ms_StatusName[(int)FormType.InspectionAudit] = "检验审核";
            ms_StatusName[(int)FormType.MoveAudit] = "移库审核";
            ms_StatusName[(int)FormType.MoveIn] = "移入";
            ms_StatusName[(int)FormType.MoveOut] = "移出";
            ms_StatusName[(int)FormType.OrderAudit] = "订单审核";
            ms_StatusName[(int)FormType.Receive] = "收货";
            ms_StatusName[(int)FormType.Return] = "退货";
            ms_StatusName[(int)FormType.ReturnReceive] = "退货收货";
            ms_StatusName[(int)FormType.Splitting] = "分装";
            ms_StatusName[(int)FormType.Order] = "订货";
            ms_StatusName[(int)FormType.SalerDispatch] = "销售发货";
        }
        public static string GetName(FormType status)
        {
            string name;
            if (ms_StatusName.TryGetValue((int)status, out name))
            {
                return name;
            }
            else
            {
                return string.Empty;
            }
        }
    }

    public static class GoodsStatusName
    {
        private static IDictionary<int, string> ms_StatusName = new Dictionary<int, string>();

        static GoodsStatusName()
        {
            ms_StatusName[(int)GoodsStatus.Usable] = "使用中";
            ms_StatusName[(int)GoodsStatus.Closed] = "完结";
        }

        public static string GetName(GoodsStatus status)
        {
            string name;
            if (ms_StatusName.TryGetValue((int)status, out name))
            {
                return name;
            }
            else
            {
                return string.Empty;
            }
        }
    }

    //public static class ApplyFormStatus
    //{
    //    private static IDictionary<int, string> ms_StatusName = new Dictionary<int, string>();

    //    public const int None = 0;
    //    public const int Applied = 1;
    //    public const int Granting = 2;
    //    public const int Granted = 3;
    //    public const int Cancel = 4;

    //    static ApplyFormStatus()
    //    {
    //        ms_StatusName[Applied] = "申请中";
    //        ms_StatusName[Granting] = "发放中";
    //        ms_StatusName[Granted] = "已发放";
    //        ms_StatusName[Cancel] = "取消";
    //    }

    //    public static string GetName(int status)
    //    {
    //        string name;
    //        if (ms_StatusName.TryGetValue(status, out name))
    //        {
    //            return name;
    //        }
    //        else
    //        {
    //            return string.Empty;
    //        }
    //    }
    //}

    public static class ReturnFormStatus
    {
        private static IDictionary<int, string> ms_StatusName = new Dictionary<int, string>();

        public const int None = 0;
        public const int Applying = 1;
        public const int Handling = 2;
        public const int Confirmed = 3;
        //public const int Cancel = 3;

        static ReturnFormStatus()
        {
            ms_StatusName[Applying] = "申请中";
            ms_StatusName[Handling] = "处理中";
            ms_StatusName[Confirmed] = "已确认";
            //ms_StatusName[Cancel] = "取消";
        }

        public static string GetName(int status)
        {
            string name;
            if (ms_StatusName.TryGetValue(status, out name))
            {
                return name;
            }
            else
            {
                return string.Empty;
            }
        }
    }

    public static class OrderDetailStatus
    {
        public const int WAITING_SEND = 1;
        public const int WAITING_AUDIT = 2;
        public const int SENDING = 3;
        public const int SENDED = 4;
        public const int REJECT = 5;
        public const int RECEIVED = 6;
        public const int INSPECTION = 7;
        public const int INSPECTION_AUDIT = 8;
        public const int INCOMING = 9;
        public const int COMPLETED = 10;

        private static Dictionary<int, string> ms_StatusName = new Dictionary<int, string>();

        static OrderDetailStatus()
        {
            ms_StatusName[WAITING_SEND] = "待发货";
            ms_StatusName[WAITING_AUDIT] = "待审核";
            ms_StatusName[SENDING] = "发货中";
            ms_StatusName[SENDED] = "已发货";
            ms_StatusName[REJECT] = "拒签";
            ms_StatusName[RECEIVED] = "已接收";
            ms_StatusName[INSPECTION] = "等检验";
            ms_StatusName[INSPECTION_AUDIT] = "检验审核中";
            ms_StatusName[INCOMING] = "待入库";
            ms_StatusName[COMPLETED] = "完成";
        }

        public static string GetName(int status)
        {
            string name;
            if (ms_StatusName.TryGetValue(status, out name))
            {
                return name;
            }
            else
            {
                return string.Empty;
            }
        }
    }

    public static class UnitBusinessTypeName
    {
        private static Dictionary<UnitBusinessType, string> ms_StatusName = new Dictionary<UnitBusinessType, string>();

        static UnitBusinessTypeName()
        {
            ms_StatusName[UnitBusinessType.None] = "未知";
            ms_StatusName[UnitBusinessType.Business] = "使用单位";
            ms_StatusName[UnitBusinessType.Storeroom] = "库房";
        }

        public static string GetName(UnitBusinessType businessType)
        {
            string name;
            if(ms_StatusName.TryGetValue(businessType, out name))
            {
                return name;
            }
            else
            {
                return ms_StatusName[UnitBusinessType.None];
            }
        }
    }

    public static class GoodsCheckStatus
    {
        public const string ACTIVE = "active";
        public const string INACTIVE = "inactive";
    }

    public static class GoodsCheckDataStatus
    {
        public const string SCAN = "scan";
        public const string CHECKED = "checked";
        public const string UNKNOWN = "unknown";
    }


    public class FormKind
    {
        public static readonly string OrderDetail = "order_detail";
        public static readonly string DispatchItem = "dispatch_item";
        public static readonly string Receive = "receive";
        public static readonly string ReceiveItem = "receive_item";
        public static readonly string Inspection = "inspection";
        public static readonly string Incoming = "incoming";
        public static readonly string MovedOut = "moved_out";
        public static readonly string MovedIn = "moved_in";
    }
}
