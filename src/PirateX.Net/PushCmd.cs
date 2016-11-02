using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using PirateX.Protocol.Package;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Net
{
    /// <summary>
    /// 将任务进行包装，推送到工作机上进行处理
    /// </summary>
    public class PushCmd : CommandBase<ProxySession, BinaryRequestInfo>
    {
        public override void ExecuteCommand(ProxySession session, BinaryRequestInfo requestInfo)
        {
            var request = session.ProtocolPackage.UnPackToRequestPackage(requestInfo.Body);


            var msg = new NetMQMessage();
            msg.Append(new byte[] { 1 });//版本号
            msg.Append("action");//动作
            msg.Append(session.SessionID);//sessionid
            msg.Append(session.ProtocolPackage.ClientKeys);//客户端密钥
            msg.Append(session.ProtocolPackage.ServerKeys);//服务端密钥
            msg.Append(request.HeaderBytes);//信息头
            msg.Append(request.ContentBytes);//信息体
            //加入队列
            ProxyServer.PushQueue.Enqueue(msg);
            //pub log
        }
    }
}
