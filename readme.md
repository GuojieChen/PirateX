#PirateX
.NET FrameWrok : 4.5.2


PirateX

> TODO    框架目标     
更加快速的重连机制   
客户端数据同步机制，只返回变化的数据，大部分数据存放在客户端自己这边   
缓存管理，合理是用散列的方式来存储数据模型   
协议的修改，以此来支持重连、扩展等机制   
更加简便的国际化语言管理方式，最好使用CVS格式来管理字段对应的不同语种  
红点推送机制的优化   
Config索引机制   
消息广播   
定时重载Config   
日志的记录   
push   
protobuf，自动生成模型的描述文件

TODO SubscribeAsync 需要看看是否用对了


##配置数据模型(Config)
```csharp
/// <summary>
/// 普通模式，以Id为主键的
/// </summary>
[ConfigIndex("Index1", "Index2")]
public class PetConfig : IConfigIdEntity
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Index1 { get; set; }

    public int Index2 { get; set; }
}
```

```csharp
/// <summary>
/// KEY-VALUE模式
/// </summary>
public class DefaultConfig : IConfigKeyValueEntity
{
    public string Id { get; set; }

    public string V { get; set; }
}
```

```csharp
///配置模型索引键，后期查询用
Resolver.Resolve<IConfigReader>().SingleByIndexes<PetConfig>(new
{
    Index1 = 1,
    Index2 = 2
}); 
```

##表结构自维护   
注册需要进行维护的程序集，在此之后，每次重启的时候都会对表结构进行维护    
这里的 “EntityAssembly”是一个关键字

```csharp
public override void IocConfig(ContainerBuilder builder)
{
    builder.Register(c => typeof(Role).Assembly).Named<Assembly>("EntityAssembly").SingleInstance();
}
```

此外还有两个开关来控制每个容器是否启用表结构的维护   
```csharp
IServerSetting.AlterTable
```
该开关是全局控制的，如果为FALSE，则所有的容器都不执行表结构的维护

```csharp
IDistrictConfig.AlterTable
```
该开关只控制自己的容器


>特别提示   
开发过程中严谨对模型字段进行改名，这会带来不必要的麻烦。通常都是增加冗余字段。


##全局配置
指在AppSettings中可以配置的项

##服务器配置    
框架中的配置基本都是基于IOC来控制的，所以我们需要做的就是在<code>IocConfig()</code> 方法中指定相应的配置。   
###全局Redis序列化方式
默认采用的是<code>ProtobufRedisSerializer</code>方式来进行序列化和反序列化   
框架中另外提供了 [protobuf](https://github.com/mgravell/protobuf-net) 的方式来进行序列化和反序列化
```csharp
public override void IocConfig(ContainerBuilder builder)
{
    builder.Register(c => new JsonRedisSerializer()).As<IRedisSerializer>().SingleInstance();
}
```   
采用protobuf方式模式时 需要设定游戏模型的属性

```csharp
[Serializable]
[ProtoContract(Name = "RoleInfoResponse")]
public class RoleInfoResponse
{
    [ProtoMember(1)]
    public string Name { get; set; }

    [ProtoMember(2)]
    public int Lv { get; set; }

    [ProtoMember(3)]
    public DateTime CreateAt { get; set; }
}
```
>TODO 框架提供的基础模型需要支持好此属性

###工作单元
所有数据库查询的连接都是既用既开的


##数据缓存

###1、缓存类型
**1. 配置数据**   
<code>config</code>   
指游戏中的数值数据，这样的数据单独放在一个数据库中。服务器在启动过程中进行加载缓存。缓存直接缓在内存当中。
>当配置数据发生变化的时候，需要重新启动机器。
>TODO 后续需要修改成启动的时候加载。后期变动时可以将数据flush掉，如果内存中没有该数据，则重新加载数据。 不过这样会产生一个问题，线上玩家需要等待配置数据加载完成才能进行请求的处理，会影响到性能问题（需要再议）

**2. 私有数据**   
<code>private</code>   
私有型数据不直接操作数据库，所有的更改和查询都会在Redis中，当然也不支持关系型数据库的查询。
私有型数据会在玩家初次登陆时加载，并保存24小时。在操作过程中，记录保存到Redis，操作记录会添加到同步都列，并且是针对单个对象进行合并处理。
>变化类型有   增/删/改 其代码分别为 i/d/u  
>其中优先级为 删>增>改

私有数据是按照角色rid存放在独自的容器中。

>TODO 私有数据的管理需要考虑到Redis的特性。后续在做具体的支持

**4. 缓存数据**   
<code>internal</code>   
缓存型数据是指那些不重要，并且可以容忍数据丢失的
一般用作类似聊天这样的数据



##国际化
1、AppSettings中添加配置

```csharp
<add key="defaultCulture" value="zh-CN"/>
```


> TODO 国际化需要支持多国语言。并且各语言需要在同一个CSV文件中，方便后期维护。

> 1、Task做分布式协调，可以参考AkkA.NET    
> 2、玩家私有数据 可以做缓存，数据库层面可以进行分表
> 3、玩家历史数据的保留可以考虑分表分库，在库表的定位上需要进行封装

##Open Source Projects in Use

- [Autofac](https://github.com/autofac/Autofac)
- [SuperSocket](https://github.com/kerryjiang/SuperSocket)
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
- [ServiceStack-V3](https://github.com/ServiceStack/ServiceStack)
- [ServiceStack.Text-V3](https://github.com/ServiceStack/ServiceStack.Text)
- [ServiceStack.Redis-V3](https://github.com/ServiceStack/ServiceStack.Redis)