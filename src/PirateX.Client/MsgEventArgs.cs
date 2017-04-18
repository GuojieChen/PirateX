using System;

namespace PirateX.Client
{
    public class MsgEventArgs : EventArgs
    {
        public string Msg { get; private set; }

        public byte[] OriginalBytes { get; private set; }
        /// <summary> 是否是应答方式获得的
        /// 不是的话就是广播了
        /// </summary>
        public bool IsAck { get; set; }
        public MsgEventArgs(string msg, byte[] originalBytes,bool isAck)
        {
            this.Msg = msg;
            this.OriginalBytes = originalBytes;
            IsAck = isAck; 
        }

        public MsgEventArgs(string msg,byte[] originalBytes) :this(msg,originalBytes,true)
        {

        }
    }
}
