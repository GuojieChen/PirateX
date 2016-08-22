# PirateX
.NET FrameWrok : 4.5.2


PirateX

> TODO    框架目标     
更加快速的重连机制   
客户端数据同步机制，只返回变化的数据，大部分数据存放在客户端自己这边   
缓存管理，合理用散列的方式来存储数据模型   
协议的修改，以此来支持重连、扩展等机制    
红点推送机制的优化   
消息广播
日志的记录   
protobuf，自动生成模型的描述文件   
DDD的实践   
多语系统,服务器拜托语言的束缚，都采用模板的方式，包括信件系统、任务系统等等   
支持实时对战模式的处理    
静态数据静默重载     

>TODO SubscribeAsync 需要看看是否用对了   


## 功能说明
1. 登陆控制  实行单设备登陆。每次登陆或者重连都将记录当前角色的SessionID ，并在每次请求中检查SessionID，如果发现不一致的，说明角色登陆有变化，需要重新连接


## 配置数据模型(Config)
>注册配置    

```csharp
public class DemoServer : GameServer<DemoSession,OnlineRole>
{
        public override Assembly ConfigAssembly()
        {
            return this.GetType().Assembly;
        }
}
```  

```csharp
/// <summary>
/// 普通模式，以Id为主键
/// 额外会有其他的作为查询索引
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

## 表结构自维护   
注册需要进行维护的程序集，在此之后，每次重启的时候都会对表结构进行维护    
这里的 “EntityAssembly”是一个关键字

```csharp
public override void IocConfig(ContainerBuilder builder)
{
    builder.Register(c => typeof(Role).Assembly).Named<Assembly>("EntityAssembly").SingleInstance();
}
```

此外还有两个开关来控制每个容器是否启用表结构的维护   
全局开关，如果为FALSE，则所有的容器无论是否启用表维护都不执行表结构的维护
```csharp
IServerSetting.AlterTable
```
私有关只控制自己的容器
```csharp
IDistrictConfig.AlterTable
```

>特别提示   
开发过程中严谨对模型字段进行改名，这会带来不必要的麻烦。通常都是增加冗余字段。    


## 全局配置
指在AppSettings中可以配置的项

## 服务器配置    
框架中的配置基本都是基于IOC来控制的，所以我们需要做的就是在<code>IocConfig()</code> 方法中指定相应的配置。   
### 全局Redis序列化方式
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

### 工作单元
所有数据库查询的连接都是既用既开的


## 数据缓存

### 1、缓存类型
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



## PUSH
设备层面的消息推送

```csharp
Resolver.Resolve<IPushService>().Notification();
```

## 广播
游戏层面的消息推送
```csharp
//广播给玩家
Resolver.Resolve<IMessageBroadcast>().Send(new News{ Name = "abc", Content = "Content" }, 1, 2);
//广播给一个区
Resolver.Resolve<IMessageBroadcast>().SendToDistrict(new News{ Name = "abc", Content = "Content" }, 1, 2);
```

>广播返回给客户端的模型是
```csharp
new
{
    B = typeof(T).Name,//类型的名称，上面是News
    D = message
}
```

## Protobuf模型获取
在程序启动阶段，会更新一下本地的protobuf清单，启动之后，客户端可以对其进行同步
（暂定），如果客户端不能动态更新 则需要另外的方式

## 国际化
1、AppSettings中添加配置

```csharp
<add key="defaultCulture" value="zh-CN"/>
```

## 玩家数据
玩家数据的操作没有做严格的区分，可以按照实际的项目进行设计    
>框架在开发的时候可以提供相应的组件
### 公共数据
数据存放在一起，在一些逻辑处理的时候涉及到全量数据的。   
用例说明：   
冠军联赛分组   
   

用户昵称查找    
	玩家数据作为公共数据，但在玩家登陆之后是存放在内存当中的，自己使用角色信息的时候是对内存的访问。当有修改的时候需要通过某种机制保存到数据库中    

排行数据	

>这种模式可能会遇到一个挑战，就是在大量部署游戏服导致数据库连接不够。    
>一种解决办法是：部署一套公共数据逻辑操作API，游戏服通过API进行处理。


### 私有数据
数据使用不涉数据库层面及多玩家的筛选和计算。别的玩家在访问数据的时候只能通过逻辑接口进行获取。    
此种数据可以按照相应的规则进行分库

#### 分库操作
账号服生成角色的时候可以有一个分库Id


### 归档数据
玩家数据删除的情况下迁入到归档数据中     
此数据库可以跟私有数据库类似，进行分库设计


> TODO 国际化需要支持多国语言。

> 1、Task做分布式协调，可以参考AkkA.NET    
> 2、玩家私有数据 可以做缓存，数据库层面可以进行分表
> 3、玩家历史数据的保留可以考虑分表分库，在库表的定位上需要进行封装

## Open Source Projects in Use

- [Autofac](https://github.com/autofac/Autofac)
- [SuperSocket](https://github.com/kerryjiang/SuperSocket)
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
- [ServiceStack-V3](https://github.com/ServiceStack/ServiceStack)
- [ServiceStack.Text-V3](https://github.com/ServiceStack/ServiceStack.Text)
- [ServiceStack.Redis-V3](https://github.com/ServiceStack/ServiceStack.Redis)