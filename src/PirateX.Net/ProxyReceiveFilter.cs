using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.Common;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Net
{
    public class ProxyReceiveFilterFactory : IReceiveFilterFactory<BinaryRequestInfo>
    {
        public IReceiveFilter<BinaryRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new ProxyReceiveFilter(appSession);
        }
    }

    public class ProxyReceiveFilter:FixedHeaderReceiveFilter<BinaryRequestInfo>
    {

        private IAppSession _session;
        public ProxyReceiveFilter(IAppSession session) : base(0)
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
        }
    }
}
