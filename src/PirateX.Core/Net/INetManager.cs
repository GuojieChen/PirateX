using PirateX.Protocol.Package;

namespace PirateX.Core.Net
{
    public interface INetManager
    {
        ProtocolPackage GetProtocolPackage(string sessionid);

        void Send(string sessionid,byte[] datas);

    }
}
