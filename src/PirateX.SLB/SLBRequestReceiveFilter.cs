using System;
using SuperSocket.Common;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.SLB
{
    public class SLBRequestReceiveFilter : FixedHeaderReceiveFilter<BinaryRequestInfo>
    {
        private SLBSession _session; 

        public SLBRequestReceiveFilter(SLBSession session) : base(0)
        {
            this._session = session;
        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            return BitConverter.ToInt32(header, offset);
        }

        protected override BinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            var bytes = bodyBuffer.CloneRange(offset, length); 
            _session.PushRequestToRemoteServer(bytes, 0,bytes.Length);

            return null; 
        }
    }
}
