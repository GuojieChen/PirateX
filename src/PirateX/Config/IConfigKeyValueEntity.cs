namespace PirateX.Config
{
    /// <summary>
    /// KEY-VALUE 存储模型的接口 
    /// 
    /// id 既Key
    /// </summary>
    public interface IConfigKeyValueEntity :IConfigEntity<string>
    {
        string V { get; set; }
    }
}
