using System;

namespace PirateX.Core.Online
{
    /// <summary> 在线管理抽象类
    /// </summary>
    public interface IOnlineManager<TOnlineRole>
         where TOnlineRole : IOnlineRole
    {
        /// <summary> 服务器上线
        /// </summary>
        void ServerOnline();
        /// <summary> 服务器下线
        /// </summary>
        void ServerOffline();
        /// <summary> 角色登录
        /// </summary>
        void Login(TOnlineRole onlineRole);
        /// <summary> 角色登出
        /// </summary>
        void Logout(long rid, string sessionid);
        /// <summary> 是否在线
        /// </summary>
        /// <param name="rid">角色ID</param>
        /// <returns></returns>
        bool IsOnline(long rid);
        /// <summary> 获取在线角色信息
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        TOnlineRole GetOnlineRole(long rid);
    }

    /// <summary> 抽象的在线角色信息
    /// </summary>
    public interface IOnlineRole
    {
        long Id { get; set; }
        /// <summary> 区服ID
        /// </summary>
        int Did { get; set; }
        /// <summary>
        /// 区服NAME
        /// </summary>
        string HotName { get; set; }
        /// <summary>
        /// Session ID
        /// </summary>
        string SessionID { get; set; }
    }
}
