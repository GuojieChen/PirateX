using System;
using System.IO;
using System.Net;
using System.Text;
using SuperSocket.Common;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Protocol.Http
{
    class HttpProxyReceiveFilter : ReceiveFilterBase<IGameRequestInfo>
    {
        private static byte[] m_HeadSeparator = Encoding.ASCII.GetBytes("\r\n\r\n");

        private const string CONNECT = "CONNECT";

        private const string PROTOCOL = "HTTP/1.1";

        private const string HOST = "Host:";

        private SuperSocket.Common.SearchMarkState<byte> m_SearchState = new SuperSocket.Common.SearchMarkState<byte>(m_HeadSeparator);

        public override IGameRequestInfo Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int left)
        {
            left = 0;

            int prevMatched = m_SearchState.Matched;

            int result = readBuffer.SearchMark(offset, length, m_SearchState);

            if (result < 0)
            {
                this.AddArraySegment(readBuffer, offset, length, toBeCopied);
                return null;
            }

            int requestLength = prevMatched > 0 ? (BufferSegments.Count - prevMatched) : (BufferSegments.Count + result - offset);

            if (offset + length > result + m_HeadSeparator.Length)
            {
                return null;
            }

            string header;

            if (this.BufferSegments.Count <= 0)
                header = Encoding.ASCII.GetString(readBuffer, offset, requestLength);
            else
            {
                this.BufferSegments.AddSegment(readBuffer, offset, requestLength, toBeCopied);
                header = this.BufferSegments.Decode(Encoding.ASCII);
                this.BufferSegments.ClearSegements();
            }

            var lineReader = new StringReader(header);

            string line = lineReader.ReadLine();

            var headItems = line.Split(' ');

            var method = headItems[0];

            var fullHost = headItems[1].Trim();
            if (method.Equals(CONNECT))//http request, fullHost misses https.
            {
                if (!fullHost.StartsWith("https://"))
                {
                    fullHost = "https://" + fullHost;
                }
            }

            var uri = new Uri(fullHost);

            if (string.IsNullOrEmpty(uri.Host))
            {
                return null;
            }

            int port = uri.Port > 0 ? uri.Port : 80;

            EndPoint targetEndPoint;

            IPAddress ipAddress;

            if (IPAddress.TryParse(uri.Host, out ipAddress))
                targetEndPoint = new IPEndPoint(ipAddress, port);
            else
                targetEndPoint = new DnsEndPoint(uri.Host, port);

            return null;
        }

        private static byte[] m_OkResponse = Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\n\r\n");
        private static byte[] m_FailedResponse = Encoding.ASCII.GetBytes("HTTP/1.1 400 FAILED\r\n\r\n");

        private string m_SendingHeader;
        
    }
}
