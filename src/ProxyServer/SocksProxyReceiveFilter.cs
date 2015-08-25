using System;
using SuperSocket.SocketBase.Protocol;

namespace PokemonX.ProxyServer
{
    class SocksProxyReceiveFilter : ReceiveFilterBase<BinaryRequestInfo>
    {
        public override BinaryRequestInfo Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int left)
        {
            throw new NotImplementedException();
        }
    }
}
