using System;
using System.Diagnostics;
using System.Net;
using SuperSocket.Common;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Net.SuperSocket
{
    public class ProxyReceiveFilterFactory : IReceiveFilterFactory<BinaryRequestInfo>
    {
        public IReceiveFilter<BinaryRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new ProxyReceiveFilter((ProxySession)appSession);
        }
    }

    public class ProxyReceiveFilter : FixedHeaderReceiveFilter<BinaryRequestInfo>
    {

        private ProxySession _session;
        public ProxyReceiveFilter(ProxySession session) : base(0)
        {
            _session = session;
        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            var dataLen = BitConverter.ToInt32(header, offset);
            return dataLen;
        }

        protected override BinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            //原始数据

            var datas = bodyBuffer.CloneRange(offset, length);
            return new BinaryRequestInfo("PushCmd", datas);
            _session.AppServer.NetService.ProcessRequest(_session, datas);

            return null;
        }
    }
}
