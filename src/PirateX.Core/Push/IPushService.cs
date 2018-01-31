using System;

namespace PirateX.Core
{
    /// <summary> 设备信息推送接口
    /// </summary>
    public interface IPushService
    {
        void Notification(IPushMessage message);
    }

    public interface IPushMessage
    {
        long Rid { get; set; }

        string Token { get; set; }

        string Sound { get; set; }

        string Msg { get; set; }

        string Channel { get; set; }

        DateTime LogTime { get; set; }
    }
}
