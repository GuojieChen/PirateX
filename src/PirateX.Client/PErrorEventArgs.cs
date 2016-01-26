using System;

namespace PirateX.Client
{
    public class PErrorEventArgs : EventArgs
    {
        public int Code { get; set; }
        public string Msg { get; set; }

        public PErrorEventArgs(int code, string msg)
        {
            Code = code;
            Msg = msg;
        }
    }
}
