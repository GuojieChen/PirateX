using System.Net;
using SuperSocket.ClientEngine;

namespace PirateX.SLB
{
    public class AsyncTcpSession2 : AsyncTcpSession
    {

        private string _servername; 

        public AsyncTcpSession2(EndPoint remoteEndPoint) : base()
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
