using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Net.Actor
{
    public class PirateXException:Exception
    {
        public string ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        public PirateXException(string errorCode, string errorMsg)
        {
            this.ErrorCode = errorCode;
            this.ErrorMsg = errorMsg;
        }
    }
}
