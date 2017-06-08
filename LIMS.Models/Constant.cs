using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Modes
{

    public enum UnitType { None = 0, Hospital = 10, Vendor = 20, HospitalUnit = 30, VendorUnit = 40 }

    public static class Constant
    {
        public const string ADMIN_ID = "00000000-0000-0000-0000-000000000000";
        public const string SYSTEM_NAME = "LIMS";
        public const string DEFAULT_UNIT_ROOT_ID = "0";
        public const string CURRENT_HOSPITAL_COOKIE = "CurrentHospital";
    }

    public sealed class ErrorCodes
    {
        public static int RequireField = 1000001;
    }

    public static class IdentityKey
    {
        public const string ORDER_FORM = "ORDER_FORM";
        public const string GOOD_BARCODE = "GOOD_BARCODE";
    }

    public static class OrderStatus
    {
        public const int Editable = 0;
        public const int View = 1;
    }

    public enum GoodBarcodeValidType
    {
        Dispatch
    }

    public static class OrderDetailStatus
    {
        public const int WAITING_SEND = 1;
        public const int WAITING_AUDIT = 2;
        public const int SENDING = 3;
        public const int SENDED = 4;
        public const int CANCEL = 5;

        public static Dictionary<int, string> ms_StatusName = new Dictionary<int, string>();

        static OrderDetailStatus()
        {
            ms_StatusName[WAITING_SEND] = "待发货";
            ms_StatusName[WAITING_AUDIT] = "待审核";
            ms_StatusName[SENDING] = "发货中";
            ms_StatusName[SENDED] = "已发货";
            ms_StatusName[CANCEL] = "取消";
        }

        public static string GetName(int status)
        {
            string name;
            if(ms_StatusName.TryGetValue(status, out name))
            {
                return name;
            }
            throw new Exception("The order status is invalid.");
        }
    }
}
