namespace PirateX.Core
{
    /// <summary> 游戏配置模型
    /// </summary>
    public interface IDistrictConfig
    {
        int Id { get; set; }

        string SecretKey { get; set; }
        /// <summary>
        /// 合服后的ID
        /// </summary>
        int TargetId { get; set; }
    }
}
