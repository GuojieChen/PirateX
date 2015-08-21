using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Online
{
    /// <summary> 在线管理抽象类
    /// </summary>
    public interface IOnlineManager
    {
        /// <summary> 机器下线
        /// </summary>
        void ServerOnline();
        /// <summary> 机器上线
        /// </summary>
        void ServerOffline();
        /// <summary> 登录
        /// </summary>
        void Login(IOnlineRole onlineRole);
        /// <summary> 登出
        /// </summary>
        void Logout(long rid,string sessionid);
        /// <summary> 是否在线
        /// </summary>
        /// <param name="rid">角色ID</param>
        /// <returns></returns>
        bool IsOnline(long rid);
        /// <summary> 获取在线角色信息
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        IOnlineRole GetOnlineRole(long rid);
    }

    /// <summary> 抽象的在线角色信息
    /// </summary>
    public interface IOnlineRole
    {
        long Id { get; set; }

        int ServerId { get; set; }

        string ServerName { get; set; }

        string SessionID { get; set; }
    }
}
