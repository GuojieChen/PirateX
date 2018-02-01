using PirateX.Protocol;

namespace PirateX.Core
{
    public interface INetManager
    {
        IProtocolPackage GetProtocolPackage(string sessionid);

        IProtocolPackage GetProtocolPackage(int rid);


        void Attach(IProtocolPackage package);

        void Send(string sessionid,byte[] datas);

    }
}
