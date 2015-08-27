using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.ClientEngine;

namespace GameServer.SLB
{
    public class AsyncTcpSession2 : AsyncTcpSession
    {

        private string _servername; 

        public AsyncTcpSession2(EndPoint remoteEndPoint) : base(remoteEndPoint)
        {
            _servername = remoteEndPoint.ToString();
        }

        public AsyncTcpSession2(EndPoint remoteEndPoint, int receiveBufferSize) : base(remoteEndPoint, receiveBufferSize)
        {
            _servername = remoteEndPoint.ToString();
        }


        public override string ToString()
        {
            return _servername;
        }
    }
}
