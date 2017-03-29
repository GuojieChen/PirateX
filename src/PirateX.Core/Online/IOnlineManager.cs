using System;

namespace PirateX.Core.Online
{

    public interface IOnlineManager
    {
        /// <summary> 角色登录
        /// </summary>
        void Login(IOnlineRole onlineRole);
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
        IOnlineRole GetOnlineRole(long rid);

        IOnlineRole GetOnlineRole(string sessionid);
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
        /// Session ID
        /// </summary>
        string SessionId { get; set; }
        /// <summary>
        /// 第三方账号标识
        /// </summary>
        string Uid { get; set; }
        /// <summary>
        /// 建立时间
        /// </summary>
        DateTime StartUtcAt { get; set; }
        /// <summary>
        /// 账号服用以授权的token信息
        /// </summary>
        string Token { get; set; }
        /// <summary>
        /// 设备标识
        /// </summary>
        string DeviceId { get; set; }
    }
}
