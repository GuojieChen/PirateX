using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Actor;

namespace PirateX.GameServerTest
{
    public class ExceptionAction:ActionBase
    {

        public static readonly string ErrorCode = "testerrorcode";
        public static readonly string ErrorMsg = "testerrormsg"; 


        public override void Execute()
        {
            throw new PirateXException(ErrorCode, ErrorMsg);
        }
    }
}
