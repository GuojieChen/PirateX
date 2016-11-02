using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using PirateX.Protocol.Package;

namespace PirateX.Net.Actor.Actions
{
    public abstract class RepAction<TResponse>:ActionBase
    {
        public override void Execute()
        {
            var response = Play();


            if (Context.Request.R)
            {
                //r == trye //拿之前的数据
                //获取和保存需要保持一致
            }
            else
            {
                if (!Equals(response, default(TResponse)))
                {
                    //有值，返回

                    SendMessage(response);
                }
                else
                {
                    //返回默认值

                }

                //缓存返回值
            }
        }

        protected void SendMessage<T>(IDictionary<string,string> header,T rep)
        {
            var repMsg = new NetMQMessage(); 
            repMsg.Append(new byte[] { Context.Version });//版本号
            repMsg.Append("action");//动作
            repMsg.Append(Context.SessionId);//sessionid
            repMsg.Append(Context.ClientKeys);//客户端密钥
            repMsg.Append(Context.ServerKeys);//服务端密钥
            repMsg.Append(GetHeaderBytes(header));//信息头
            repMsg.Append(ProtocolPackage.ResponseConvert.SerializeObject(rep));//信息体

            base.MessageQeue.Enqueue(repMsg);
        }

        protected void SendMessage(TResponse rep)
        {
            var headers = new Dictionary<string,string>();
            headers.Add("c", Context.Request.C);
            headers.Add("i", "1");//返回类型 
            headers.Add("o", Convert.ToString(Context.Request.O));
            headers.Add("code", Convert.ToString((int)200));

            SendMessage(headers,rep);
        }


        protected void SendMessage<T>(string name, T data)
        {
            var headers = new Dictionary<string, string>();
            headers.Add("c", Context.Request.C);
            headers.Add("i", "2");//返回类型 
            headers.Add("o", Convert.ToString(Context.Request.O));
            headers.Add("code", Convert.ToString((int)200));

            SendMessage(headers, data);
        }

        private byte[] GetHeaderBytes(IDictionary<string,string> headers)
        {
            return Encoding.UTF8.GetBytes($"{String.Join("&", headers.Keys.Select(a => a + "=" + headers[a]))}");
        }

        public abstract TResponse Play();
    }
}
