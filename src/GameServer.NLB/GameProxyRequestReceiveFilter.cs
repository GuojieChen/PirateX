using System;
using SuperSocket.Common;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.NLB
{
    public class GameProxyRequestReceiveFilter : FixedHeaderReceiveFilter<BinaryRequestInfo>
    {
        private GameProxySession _session; 

        public GameProxyRequestReceiveFilter(GameProxySession session) : base(0)
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
