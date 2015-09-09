namespace PirateX.Cointainer
{
    /// <summary> 游戏配置模型
    /// </summary>
    public interface IDistrictConfig
    {
        int Id { get; set; }
        /// <summary> 游戏数据库地址
        /// </summary>
        string ConnectionString { get; set; }
        /// <summary> 配置表数据库连接地址
        /// </summary>
        string ConfigConnectionString { get; set; }

        string Redis { get; set; }
        int RedisDb { get; set; }
    }


}
