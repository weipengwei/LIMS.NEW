using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Util;

namespace LIMS.Models
{
    public class ResponseResult
    {
        public ResponseResult()
        {
            this.IsSuccess = true;
        }

        public ResponseResult(bool isSuccess, object data)
        {
            this.IsSuccess = isSuccess;
            if (!this.IsSuccess)
            {
                if (data != null)
                {
                    if (data is InvalidException)
                    {
                        this.ErrorCode = ErrorCodes.DefinedMessage;
                        this.Message = ((InvalidException)data).Message;
                    }
                    else if (data is Exception)
                    {
                        var e = data as Exception;
                        this.ErrorCode = ErrorCodes.Unknow;
                        this.Message = string.Format("Message:{0}|TraceStack:{1}", e.Message, e.StackTrace);
                    }
                    else
                    {
                        this.Data = data;
                    }
                }
            }
            else
            {
                this.Data = data;
            }
        }

        public ResponseResult(bool isSuccess, string message): this(isSuccess, message, ErrorCodes.DefinedMessage)
        {
        }

        public ResponseResult(bool isSuccess, object data, PagerInfo pageInfo) : this(isSuccess, data)
        {
            this.PageInfo = pageInfo;
        }

        public ResponseResult(bool isSuccess, string message, int errorCode = 0)
        {
            this.IsSuccess = isSuccess;
            this.Message = message;
            this.ErrorCode = errorCode;
        }

        public ResponseResult(Exception e): this(false, e)
        {
        }

        public bool IsSuccess
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }

        public int ErrorCode
        {
            get; set;
        }

        public object Data
        {
            get; set;
        }

        public PagerInfo PageInfo
        {
            get; set;
        }
    }
}
