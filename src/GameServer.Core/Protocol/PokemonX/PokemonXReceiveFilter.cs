using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using SuperSocket.Common;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Logging;

namespace GameServer.Core.Protocol.PokemonX
{
    /*
     4字节表示整体数据的长度
     1字节表示是否启用压缩
     1字节表示加密方式 128表示XXTea
    */
    public class PokemonXReceiveFilter : FixedHeaderReceiveFilter<IPokemonXRequestInfo>
    {
        private ILog Logger { get; set; }

        private IGameSession _session;

        public PokemonXReceiveFilter(IGameSession session)
            : base(0)
        {
            this._session = session;
            this.Logger = session.Logger;
        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            var dataLen = BitConverter.ToInt32(header, offset);
#if DEBUG
            if (Logger.IsDebugEnabled)
            {
                var builer = new StringBuilder();
                builer.AppendLine();
                builer.AppendLine("---------------Get request from client---------------"); 
                builer.AppendLine(_session.SessionID);
                builer.AppendLine($"Header.Length : {header.Length}");
                builer.AppendLine($"Offset : {offset}");
                builer.AppendLine($"DataBody.Length : {dataLen}");
                Logger.Debug(builer);
            }
#endif
            return dataLen;
        }

        private void PrintKeys(string[] names, params byte[][] keys)
        {
            if (!Logger.IsDebugEnabled)
                return;
            if (keys == null)
                return;

            for (int i = 0; i < keys.Count(); i++)
            {
                Logger.Debug(names[i] + ":" + string.Join(",", keys[i]));
            }
        }

        protected override IPokemonXRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
#if DEBUG
            if (_session.PackageProcessor.ClientKeys.Any() && _session.PackageProcessor.ServerKeys.Any())
                PrintKeys(new[] { "ClientKey", "ServerKey" }, _session.PackageProcessor.ClientKeys[0], _session.PackageProcessor.ServerKeys[0]);

            LogHead(bodyBuffer, offset, length);
#endif

            var body = Encoding.UTF8.GetString(_session.PackageProcessor.Unpack(bodyBuffer.CloneRange(offset, length)));

            if (Logger.IsDebugEnabled)
                Logger.Debug($"Request\t#{_session.Rid}#\t{_session.RemoteEndPoint}\t{_session.SessionID}\t{body}");

            var jObject = JObject.Parse(body);
            var c = jObject["C"];
            if (c == null)
                //throw new AbstactGameException(ServerCode.NotFound, "C");
                return null; 

            var r = false;
            var rjson = jObject["R"];
            if (rjson != null)
                r = Convert.ToBoolean(rjson);

            var o = 0;
            var ojson = jObject["O"];
            if (ojson != null)
                o = Convert.ToInt32(ojson);

            if (_session.MyLastO > o)
            {
                Logger.Error($"RequestError\t#{_session.Rid}#\t{_session.RemoteEndPoint}\t{_session.SessionID}\t{body}");
                _session.Close(CloseReason.SocketError);

                return null;
                //throw new AbstactGameException(ServerCode.RequestError);
            }
            if (!r) //retry的情况下O会被改大
                _session.MyLastO = o; 

            _session.CurrentO = o;
            
            if (Logger.IsDebugEnabled)
                Logger.Debug($"Client[{_session.SessionID}]'s O is {_session.CurrentO}");

            return new PokemonXRequestInfo(c.ToString(), jObject["D"], jObject["Ex"], r);
        }

        private void LogHead(byte[] bodyBuffer, int offset, int length)
        {
            if (Logger.IsDebugEnabled)
            {
                var headBytes = bodyBuffer.CloneRange(offset, 6);

                Logger.Debug($"Head info:{string.Join(",", headBytes)}");

                var bodyCopy = bodyBuffer.CloneRange(offset, length);

                byte[] dataBytes = null;
                var zipBit = new byte[1];
                var cryptoBit = new byte[1];

                using (var stream = new MemoryStream(bodyCopy))
                {
                    var lenBytes = new byte[4];

                    stream.Read(lenBytes, 0, 4);
                    stream.Read(zipBit, 0, 1);
                    stream.Read(cryptoBit, 0, 1);

                    var len = BitConverter.ToInt32(lenBytes, 0) - 6;
                    dataBytes = new byte[len];
                    stream.Read(dataBytes, 0, len);
                }

                Logger.Debug(string.Join(",", dataBytes));
            }
        }
    }
}
