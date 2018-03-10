# PirateX
.NET FrameWrok : 4.5.2


PirateX   

游戏服务器引擎


> TODO    框架目标     
客户端数据同步机制，只返回变化的数据，大部分数据存放在客户端自己这边   
缓存管理，合理用散列的方式来存储数据模型     
红点推送机制的优化     
protobuf，自动生成模型的描述文件   
多语系统,服务器摆脱语言的束缚，都采用模板的方式，包括信件系统、任务系统等等   
支持实时对战模式的处理    
静态数据静默重载     
需要考虑下红点     
以前的任务系统是否需要做成中间件    

>TODO SubscribeAsync 需要看看是否用对了   

## 项目层级说明
这里介绍开发一般包含的项目

>xxx.Common   
1. 异常代码
2. 常量字段
3. 多语言信息

>xxx.Config   
静态配置模型，程序会在启动的时候从配置数据库读取并加载到程序内存中。
使用配置数据模块需要做如下配置
1. IServerSetting 实现 IConfigConnectionDistrictConfig 接口
2. 注入配置读取

>xxx.Domain   
业务模型

>xxx.Service   
数据层
业务层

>xxx.Api     
接口层

>xxx.Server   
程序入口

>xxx.Sechedul   
计划任务


## 框架功能介绍
框架提供便捷的开发方式。提供了以下功能  
1. 多区容器管理 
2. 依赖注入的使用
3. 自动生成protobuf模型描述
4. 配置数据自动加载
5. 数据库表自动维护
6. 程序升级逻辑处理
7. 中间层，封装了一些常规逻辑
8. 可拆分通信框架
9. 轻松合服

以下对每个功能进行介绍

### 多区容器管理
框架内使用Autofac作为容器，可以将不同区服隔离开，例如不同区服对应不同的数据库连接以及相应的组建。
每个区有自己的数据层和业务层，通过玩家请求头Token模型中的Did来识别所在的区服以及对应的容器。数据的操作都在该区范围内。

当然也可以通过一种特殊的方式来跨区操作。通过容器对象的GetDistrictContainer(int id=0)方法来获取对应的容器

除了分服容器外，框架还提供了全局的容器ServerIoc





### 依赖注入
框架采用了Autofac作为依赖注入框架，其内部已经包含了一些常用的内置组建。

外部可以通过注入的方式进行覆盖。

## 单设备登陆
登陆控制  实行单设备登陆。每次登陆或者重连都将记录当前角色的SessionID ，并在每次请求中检查SessionID，如果发现不一致的，说明角色登陆有变化，需要重新连接

## 表结构自维护   
本框架表维护引用了EF/ServiceStack.Ormlite模块，最终通过模型映射到数据库表，在模型设计的时候准寻EF/ServiceStack.Ormlite的标准。   

注册游戏服数据库维护对象
```csharp
protected override void BuildDistrictContainer(ContainerBuilder builder)
{
    builder.Register<IDatabaseInitializer>(c => new GameDatabaseInitializer());
}
```

注册其他数据库维护对象
```csharp
///注册其他库连接
public override IDictionary<string, string> GetNamedConnectionStrings()
{
    var list = base.GetNamedConnectionStrings();
    list.Add(ConnectionNames.Public, "server=192.168.1.54;user id=root;password=123456;persist security info=True;database=pokemoniii_public;CharSet='utf8'");
    return list;
}
///注册其他库连接对象
public override IDictionary<string,IDatabaseInitializer> GetNamedDatabaseInitializers()
{
    var dic = base.GetNamedDatabaseInitializers();
    dic.Add(ConnectionNames.Public, new PubclicDatabaseInitializer());
    return dic;
}

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
protected override void BuildServerContainer(ContainerBuilder builder)
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
客户端通信过程中通过lang参数告诉服务器需要请求的语言   

服务器除异常需要用到多语言之外，其他系统在设计过程中都需要避免直接产生文本。具体语言可以交由客户端。


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



### 归档数据
玩家数据删除的情况下迁入到归档数据中     
此数据库可以跟私有数据库类似，进行分库设计



# 数据同步 （初稿）
>需要考虑性价比
1、对玩家的每条数据添加版本号(时间戳)，通过数据库时间戳字段，自动在插入和修改数据时候变更字段值    
2、每个玩家有一个记录数据变化的版本号(时间戳)，任何一个模型有变动的时候，修改时间戳    
3、数据同步（重要）   

同步发生在以下几个地方：    
     
3.1、登陆请求
每个系统都会对应一个数据获取接口，客户端请求的时候需要告知服务器本地数据的最新版本号，客户端版本号老的情况下会返回数据列表，客户端刷新到最新的。

3.2、广播通知
广播的数据是对象模型，并且需要告知客户端该模型系统最新的版本号。 

3.3、接口通知   
接口通知发生在客户端主动请求接口，服务器执行逻辑处理引起数据变化后。
服务器告知客户端新增或者修改的对象模型数据以及该模型系统的最新版本号。
删除模型的逻辑需要客户端自身维护好。和以往一样。


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

