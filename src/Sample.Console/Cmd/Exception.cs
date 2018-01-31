using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX;
using PirateX.Core;
using PirateX.Protocol;

namespace GameServer.Console.Cmd
{
    public class Exception : RepAction
    {
        public override void Execute()
        {
            throw new PirateXException("CustomeException", "CustomeException");
        }
    }
}
