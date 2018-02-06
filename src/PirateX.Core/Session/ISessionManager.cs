using System.Collections.Generic;

namespace PirateX.Core
{

    public interface ISessionManager
    {
        /// <summary> 角色登录
        /// </summary>
        void Login(PirateSession pirateSession);
        /// <summary> 角色登出
        /// </summary>
        void Logout(long rid);
        /// <summary> 是否在线
        /// </summary>
        /// <param name="rid">角色ID</param>
        /// <returns></returns>
        bool IsOnline(long rid);
        /// <summary>
        /// 保存Session
        /// </summary>
        /// <param name="session"></param>
        void Save(PirateSession session);

        /// <summary> 获取在线角色信息
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        PirateSession GetSession(long rid);

        PirateSession GetSession(string sessionid);
        /// <summary>
        /// 获取某个服的session列表
        /// </summary>
        /// <param name="did"></param>
        /// <returns></returns>
        IEnumerable<string> GetFrontendIDListByDid(int did);
    }
}
