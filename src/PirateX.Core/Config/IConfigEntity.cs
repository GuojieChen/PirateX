namespace PirateX.Core.Config
{
    public interface IConfigEntity
    {
    }

    public interface IConfigEntity<TPrimaryKey> : IConfigEntity
    {
        TPrimaryKey Id { get; set; }
    }

    /// <summary>
    /// KEY-VALUE 存储模型的接口 
    /// 
    /// id 既Key
    /// </summary>
    public interface IConfigKeyValueEntity : IConfigEntity<string>
    {
        string Value { get; set; }
    }

    public interface IConfigIdEntity : IConfigEntity<int>
    {
        
    }
}
