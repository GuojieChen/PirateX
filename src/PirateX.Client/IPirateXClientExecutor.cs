using System.Collections.Specialized;
using PirateX.Protocol.Package;
using PirateX.Protocol.Package.ResponseConvert;

namespace PirateX.Client
{
    public interface IPirateXClientExecutor
    {
        NameValueCollection Header { get; set; }

        IResponseConvert ResponseConvert { get; set; }


        void Excute(PirateXClient client, byte[] data);
    }
}
