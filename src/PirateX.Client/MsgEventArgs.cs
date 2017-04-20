using System;
using PirateX.Protocol.Package;

namespace PirateX.Client
{
    public class MsgEventArgs : EventArgs
    {
        public string Msg { get; private set; }


        public IPirateXPackage Package { get; private set; }

        public MsgEventArgs(string c,IPirateXPackage package)
        {
           Msg = c;
            Package = package;
        }
    }
}
