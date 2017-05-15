using System.Collections.Concurrent;
using System.Net;
using PirateX.Protocol.Package;
using PirateX.Protocol.Zip;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Net.SuperSocket
{
    public class ProxySession:AppSession<ProxySession,BinaryRequestInfo>, IProtocolPackage
    {
        private ProtocolPackage ProtocolPackage  = new ProtocolPackage();


        public new GameAppServer AppServer => (GameAppServer) base.AppServer;

        public string Id
        {
            get => base.SessionID;
            set { }
        }

        public int Rid
        {
            get => ProtocolPackage.Rid;
            set => ProtocolPackage.Rid = value;
        }
        public IZip Zip => ProtocolPackage.Zip;
        public bool ZipEnable
        {
            get => ProtocolPackage.ZipEnable;
            set => ProtocolPackage.ZipEnable = value;
        }
        public byte[] PackKeys
        {
            get => ProtocolPackage.PackKeys;
            set => ProtocolPackage.PackKeys = value;
        }
        public byte[] UnPackKeys
        {
            get => ProtocolPackage.UnPackKeys;
            set => ProtocolPackage.UnPackKeys = value;
        }
        public byte CryptoByte
        {
            get => ProtocolPackage.CryptoByte;
            set => ProtocolPackage.CryptoByte = value;
        }

        public int LastNo
        {
            get => ProtocolPackage.LastNo;
            set => ProtocolPackage.LastNo = value;
        }

        public EndPoint RemoteEndPoint
        {
            get => base.RemoteEndPoint;
            set { }
        }

        public byte[] PackPacketToBytes(IPirateXPackage requestPackage)
        {
            return ProtocolPackage.PackPacketToBytes(requestPackage);
        }

        public IPirateXPackage UnPackToPacket(byte[] datas)
        {
            return ProtocolPackage.UnPackToPacket(datas);
        }

        public void Send(byte[] body)
        {
            base.Send(body,0,body.Length);
        }
    }
}
