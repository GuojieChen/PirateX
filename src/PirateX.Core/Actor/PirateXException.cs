using System;

namespace PirateX.Core
{
    public class PirateXException:Exception
    {
        public string ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        public short Code { get; set; }

        public PirateXException(string errorCode, string errorMsg)
        {
            Code = StatusCode.Exception;
            this.ErrorCode = errorCode;
            this.ErrorMsg = errorMsg;
        }
    }
}
