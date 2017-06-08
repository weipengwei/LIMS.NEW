using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Util
{
    public sealed class ErrorCodes
    {
        public static int DefinedMessage = 0;
        public static int Unauthorization = 1000002;
        public static int RequireField = 1000001;
        public static int Unknow = 9999999;

        public const string NotMatchFormKindInGoodsFlow = "1000000001";
        public const string StatusErrorInGoodsFlow = "1000000002";
    }

    public static class GoodsStateValidateCodes
    {
        public const string BarcodeNotExist = "0000000001";
        public const string NotValidState = "0000000002";
        public const string FutureFormNotExist = "0000000003";
        public const string HospitalNoFutureForm = "0000000004";
        public const string VendorNoFutureForm = "0000000005";

        public static string GetMessage(string errorCode)
        {
            string message;
            switch(errorCode)
            {
                case BarcodeNotExist:
                    message = "找不到扫描码对应的物品！";
                    break;
                case NotValidState:
                    message = "扫描码对应的物品无效！";
                    break;
                case FutureFormNotExist:
                    message = "物品不能进行当前操作！";
                    break;
                case HospitalNoFutureForm:
                    message = "物品不属于当前医院！";
                    break;
                case VendorNoFutureForm:
                    message = "物品不属于当前供应商！";
                    break;
                default:
                    message = "扫描出错啦！";
                    break;
            }

            return message;
        }
    }
}
