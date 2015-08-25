using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using PokemonX.Common.Exception;
using PokemonX.StatusCode;
using PokemonX.SuperSocket;
using PokemonX.SuperSocket.Protocol;
using SuperSocket.Common;
using SuperSocket.Facility.Protocol;

namespace PokemonX.ProxyServer
{
    public class PokemonSocketHeaderReceiveFilter : FixedHeaderReceiveFilter<ISocketRequestInfo>
    {
        private PokemonSession _session;

        public PokemonSocketHeaderReceiveFilter(PokemonSession session)
            : base(0)
        {
            this._session = session;
        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            var dataLen = BitConverter.ToInt32(header, offset);
            return dataLen;
        }

        protected override ISocketRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            var bodyBytes = _session.PackageProcessor.Unpack(bodyBuffer.CloneRange(offset, length));

            if (!_session.PackageProcessor.CryptoEnable)
            {
                var body = Encoding.UTF8.GetString(bodyBytes);

                var jObject = JObject.Parse(body);
                var c = jObject["C"];
                if (c == null)
                    throw new PException(ServerCode.NotFound, "C");

                if (Equals(c.ToString(), "NewSeed"))
                {//种子交换
                    //其他的都做代理转发
                    return new PRequestInfo(c.ToString(), jObject["D"], true);
                }

                throw new PException(ServerCode.PreconditionFailed);
            }

            //其他的都做代理转发
            _session.RequestDataReceived(bodyBytes,offset);

            return null;
        }

    }
}
