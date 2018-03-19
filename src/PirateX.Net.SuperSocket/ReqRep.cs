using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core;
using PirateX.Protocol;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Net.SuperSocket
{
    public class ReqRep : CommandBase<ProxySession, BinaryRequestInfo>
    {
        public override void ExecuteCommand(ProxySession session, BinaryRequestInfo requestInfo)
        {
            var sw = new Stopwatch();
            sw.Start();
            var dout = session.AppServer.NetService.ProcessRequest(session, requestInfo.Body);

            Console.WriteLine($"---------{sw.ElapsedMilliseconds}----------");

            if (dout == null)
                return;

            var response = new PirateXResponsePackage()
            {
                HeaderBytes = dout.HeaderBytes,
                ContentBytes = dout.BodyBytes
            };

            if (Equals(dout.Action, PirateXAction.Seed))
            {
                session.Rid = dout.Id;
                session.AppServer.Attach(session);
            }

            if (dout.LastNo >= 0)
                session.LastNo = dout.LastNo;

            var bytes = session.PackPacketToBytes(response);
            session.Send(bytes);

            if (Equals(dout.Action, PirateXAction.Seed))
            {
                var clientkey = dout.ClientKeys;
                var serverkey = dout.ServerKeys;
                var crypto = dout.Crypto;

                //种子交换  记录种子信息，后续收发数据用得到
                session.PackKeys = serverkey;
                session.UnPackKeys = clientkey;
                session.CryptoByte = crypto;
            }
        }
    }
}
