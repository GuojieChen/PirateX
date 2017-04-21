namespace PirateX.Core.Container
{
    /// <summary> 游戏配置模型
    /// </summary>
    public interface IDistrictConfig
    {
        int Id { get; set; }

        string SecretKey { get; set; }
    }

}
