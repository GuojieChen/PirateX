using PirateX.Protocol.Package;

namespace PirateX.Core.Net
{
    public interface INetManager
    {
        ProtocolPackage GetProtocolPackage(string sessionid);

        ProtocolPackage GetProtocolPackage(int rid);


        void Attach(ProtocolPackage package);

        void Send(string sessionid,byte[] datas);

    }
}
