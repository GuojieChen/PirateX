﻿# PirateX
.NET FrameWrok : 4.5.2


PirateX   

轻量级游戏服务器引擎


> TODO    框架目标     
客户端数据同步机制，只返回变化的数据，大部分数据存放在客户端自己这边   
缓存管理，合理用散列的方式来存储数据模型     
红点推送机制的优化     
protobuf，自动生成模型的描述文件   
DDD的实践   
多语系统,服务器摆脱语言的束缚，都采用模板的方式，包括信件系统、任务系统等等   
支持实时对战模式的处理    
静态数据静默重载     
需要考虑下红点     
以前的任务系统是否需要做成中间件    

>TODO SubscribeAsync 需要看看是否用对了   

## 协议说明
### 请求
|-------------------------------------------------    
|   A  |  B  |  C |  D |  E | F |    
|------------------------------------------------    

| 占位  | 字节长度 |        说明           |
| -------- | -------------- | ----------------------- |
| A       | 4字节      |  数据整体长度  |
| B       | 1字节      | 是否启用压缩   |
| C       | 1字节      | 加密描述符       |
| D       | 4字节      | 信息头长度       |
| E       |  D描述大小     | 消息头数据       |
| F       |  A大小 - 4 -1-1 -4-D大小     | 请求数据       |

消息头包含方法名称和参数

c=login&r=1&o=2&t=147364928384

后续是通信信息   
c : 请求方法
r  : 是否为重试   
o : 请求序号，自增    
t  : 时间戳    
token:授权信息    
lang:客户端请求语言 参考[区域设置](https://zh.wikipedia.org/wiki/%E5%8C%BA%E5%9F%9F%E8%AE%BE%E7%BD%AE)    
format :期望消息返回格式     


请求数据格式   
类似httpGET方式,做好encode编码    

username=xxx&password=xxx   

>Token格式

```chsarp
message Token {
   optional int32 Did = 1 [default = 0];  //服ID
   optional int64 Rid = 2 [default = 0];  //角色ID
   optional int64 Ts = 3 [default = 0];   //时间戳
   optional string Sign = 4;              //签名
   optional string Uid = 6;               //UID
}
```

### 返回
|-------------------------------------------------    
|   A  |  B  |  C |  D |  E |  F    
|------------------------------------------------                

| 占位  | 字节长度 |        说明           |
| -------- | -------------- | ----------------------- |
| A       | 4字节      |  数据整体长度  |
| B       | 1字节      | 是否启用压缩   |
| C       | 1字节      | 加密描述符       |
| D       | 4字节      | 消息头长度       |
| E       |  D描述大小     | 消息头数据       |
| F       |  A大小 - 4 -1-1 -4-D大小     | 请求数据       |

信息头   
c=login&r=1&o=2   
c : 方法名     
i : 消息类型 (1:返回  2广播)   
o : 请求序号，自增    
t  : 时间戳  
format:消息格式，默认protobuf, 其他可选json 和 text     
code:状态码 200以外都是错误异常        
errorCode:（ * ） 错误消息码    
errorMsg : （ * ） 错误消息
responsetype (*) protobuf的情况下告知客户端对应的描述   


返回数据   
可以按照需要定义好数据格式   
例如JSON 或者 protobuf

## 系统接口
## _ProtoSync_
同步模型protobuf描述   
客户端将上次同步的hash值高速服务器，在发现不一致的情况下，服务器返回最新的描述内容和hash值。   

参数 ：  hash  , 客户端上次记录的hash值    
返回 ：
```csharp 
message ProtoSyncResponse {
    optional string Hash = 1;
    optional string Proto = 2;
}
```


## 功能说明
1. 登陆控制  实行单设备登陆。每次登陆或者重连都将记录当前角色的SessionID ，并在每次请求中检查SessionID，如果发现不一致的，说明角色登陆有变化，需要重新连接

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


## 配置数据
### 注册配置 

```csharp
public class DemoServer : DistrictContainer<DemoServer>
{
        protected override List<Assembly> GetConfigAssemblyList()
        {
            var list = base.GetConfigAssemblyList();
            list.Add(typeof(TestConfig).Assembly);
            return list;
        }
}
```  
### 定义配置
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

### 使用配置



```csharp
///配置模型索引键，后期查询用
Resolver.Resolve<IConfigReader>().SingleByIndexes<PetConfig>(new
{
    Index1 = 1,
    Index2 = 2
}); 
```



### 导入配置

使用PirateX.ConfigImport.exe 将excel文件中的配置数据导入到指定的数据库中   

```csharp
PirateX.ConfigImport.exe --input "C:\\Users\\Guojie\\Desktop\\1\\config" --maxworkers 5  --connectionstring "server=192.168.1.54;user id=root;password=123456;persist security info=True;database=piratex_config;CharSet='utf8'" --dlldir "C:\\Users\\Guojie\\Desktop\1\\dll"
```   
参数说明    

| 参数  | 可选 |        说明           | 示例 |
| -------- | -------------- | ----------------------- | ---- |
| input       | N     |  配置文件目录  | |
| maxworkers       |Y       | 工作队列，默认5   | |
| ignore       | Y      | 忽略的字段       | --ignore "charactorConfig.xlsx=MegaEvId,EvId,ConsumeIds&material.xlsx=Effects,UpPrice" |
| connectionstring       | N      | 数据库连接字符串       | |
| dlldir       |  N     | 模块目录/文件       | |


## 表结构自维护   
本框架表维护引用了EF模块，最终通过模型映射到数据库表，在模型设计的时候准寻EF的标准。   

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

