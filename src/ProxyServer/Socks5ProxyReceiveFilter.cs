﻿using System;
using SuperSocket.SocketBase.Protocol;

namespace PokemonX.ProxyServer
{
    class Socks5ProxyReceiveFilter : IReceiveFilter<BinaryRequestInfo>
    {
        public BinaryRequestInfo Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int left)
        {
            throw new NotImplementedException();
        }

        public int LeftBufferSize
        {
            get { throw new NotImplementedException(); }
        }

        public IReceiveFilter<BinaryRequestInfo> NextReceiveFilter
        {
            get { throw new NotImplementedException(); }
        }


        public void Reset()
        {
            throw new NotImplementedException();
        }

        public FilterState State
        {
            get { throw new NotImplementedException(); }
        }
    }
}
