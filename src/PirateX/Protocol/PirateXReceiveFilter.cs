using System;
using System.Collections.Specialized;
using PirateX.Protocol.Package;
using SuperSocket.Common;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase;

namespace PirateX.Protocol
{
    public class PirateXReceiveFilter: FixedHeaderReceiveFilter<IPirateXRequestInfo>
    {
        private IPirateXSession _session;

        public PirateXReceiveFilter(IPirateXSession session) : base(0)
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
            var requestPack = _session.ProtocolPackage.UnPackToRequestPackage(bodyBuffer.CloneRange(offset, length));

            var requestinfo = new PirateRequestInfo2(requestPack);
            requestinfo.Key = requestinfo.C;

            if (_session.Logger.IsInfoEnabled)
                _session.Logger.Info($"Request\t#{_session.Rid}#\t{_session.RemoteEndPoint}\t{_session.SessionID}\t{requestinfo.Key}\t{requestinfo.Headers}\t{requestinfo.QueryString}");

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
