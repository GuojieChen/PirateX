using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using log4net.Repository.Hierarchy;
using SuperSocket.Common;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Protocol.Package
{
    public class PirateXReceiveFilter: FixedHeaderReceiveFilter<IPirateXRequestInfo>
    {
        private IGameSessionBase _session;

        public PirateXReceiveFilter(IGameSessionBase session) : base(0)
        {
            this._session = session;
        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            var dataLen = BitConverter.ToInt32(header, offset);
            return dataLen;
        }

        protected override IPirateXRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            var body = _session.ProtocolPackage.Unpack(bodyBuffer.CloneRange(offset, length));

            byte[] headers = null;
            byte[] content = null;
            using (var stream = new MemoryStream(body))
            {
                var headerBytes = new byte[4];
                var bodayByts = new byte[4];
                stream.Read(bodayByts, 0, 4);
                stream.Read(headerBytes, 0, 4);

                var bodyLen = BitConverter.ToInt32(bodayByts, 0);
                var headerlen = BitConverter.ToInt32(headerBytes, 0);

                //header
                headers = new byte[headerlen];
                stream.Read(headers, 0, headerlen);

                //content
                content = new byte[bodyLen - headerlen - 4 - 4];
                stream.Read(content, 0, bodyLen - headerlen - 4 - 4);
            }

            var headstr = Encoding.UTF8.GetString(headers);
            var index = headstr.IndexOf("?", StringComparison.Ordinal);
            var key = headstr.Substring(0, index);
            
            var requestinfo = new PirateXRequestInfo(key, HttpUtility.ParseQueryString(headstr), HttpUtility.ParseQueryString(Encoding.UTF8.GetString(content)));

            if (_session.Logger.IsInfoEnabled)
                _session.Logger.Info($"Request\t#{_session.Rid}#\t{_session.RemoteEndPoint}\t{_session.SessionID}\t{requestinfo?.QueryString}");

            if (string.IsNullOrEmpty(requestinfo?.Key) || _session.MyLastO > requestinfo.O)
            {
                _session.Close(CloseReason.SocketError);
                return null;
            }

            if (!requestinfo.R) //retry的情况下O会被改大
                _session.MyLastO = requestinfo.O;

            _session.CurrentO = requestinfo.O;

            return requestinfo;
        }
        
    }
}
