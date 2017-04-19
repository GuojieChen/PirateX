using System;

namespace PirateX.Core.Actor
{
    public class PirateXException:Exception
    {
        public string ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        public int Code { get; set; }

        public PirateXException(string errorCode, string errorMsg)
        {
            this.ErrorCode = errorCode;
            this.ErrorMsg = errorMsg;
        }
    }
}
