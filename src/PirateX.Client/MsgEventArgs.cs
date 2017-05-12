using System;
using PirateX.Protocol.Package;

namespace PirateX.Client
{
    public class MsgEventArgs : EventArgs
    {
        public string Msg { get; private set; }


        public IPirateXResponseInfo Package { get; private set; }

        public MsgEventArgs(string c,IPirateXResponseInfo package)
        {
           Msg = c;
            Package = package;
        }
    }

    public class OutMsgEventArgs : EventArgs
    {
        public string Msg { get; private set; }


        public IPirateXRequestInfoBase Package { get; private set; }

        public OutMsgEventArgs(string c, IPirateXRequestInfoBase package)
        {
            Msg = c;
            Package = package;
        }
    }
}
