#PirateX
.NET FrameWrok : 4.5.2

##配置

###全局配置
指在AppSettings中可以配置的项

###Server配置


##数据缓存

###1、缓存类型
**1. 配置数据**    
指游戏中的数值数据，这样的数据单独放在一个数据库中。服务器在启动过程中进行加载缓存。缓存直接缓在内存当中。
>当配置数据发生变化的时候，需要重新启动机器。
>TODO 后续需要修改成启动的时候加载。后期变动时可以将数据flush掉，如果内存中没有该数据，则重新加载数据。 不过这样会产生一个问题，线上玩家需要等待配置数据加载完成才能进行请求的处理，会影响到性能问题（需要再议）

**2. 公有**    
是指正对一个服内所有玩家的共享数据。

**3. 私有**   
是指玩家私有的数据，类似角色信息、背包等
私有数据中的核心部分会在玩家登陆时候进行加载，并且进行24小时的保存，剩余部分在第一次使用时进行加载。   
玩家加载其他玩家的私有数据


### 2、数据操作
1. 增-删-改   
    数据的自增由Redis进行维护。数据的变动会先保存到Redis，同时会产生数据变更命令（增-删-改）保存到消息队列中
    由后台异步线程定时同步到数据库中。
    >这里需要考虑一个问题，在一段时间内，数据可能由新增、修改、删除等一系列操作。如果能对此进行合并提交，这样会减轻数据库的压力
    >最好可以将同个玩家的数据进行合并提交。是否有问题
2. 查询   
    
3. 缓存标记    
    class一旦打上了缓存标记，则后续的数据维护都是先在Redis中，由异步线程定时同步到数据中。
    默认情况下是直接对关系型数据库进行操作。同时Redis特有的数据操作将不被支持
4. 

在请求某一数据时先从Redis中查找，如果没有再从数据库中获取，同时缓存到Redis中，并且后期的操作都是对Redis。


数据加载分及时加载和延迟加载两种
1. 及时加载.在服务启动的时候会将数据从数据库中加载到Redis中
2. 延迟加载.在第一次使用时会将数据加载到Redis中



##国际化
1、AppSettings中添加配置

```csharp
<add key="defaultCulture" value="zh-CN"/>
```


> TODO 国际化需要支持多国语言。并且各语言需要在同一个CSV文件中，方便后期维护。



##Open Source Projects in Use

- [Autofac](https://github.com/autofac/Autofac)
- [SuperSocket](https://github.com/kerryjiang/SuperSocket)
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
- [ServiceStack-V3](https://github.com/ServiceStack/ServiceStack)
- [ServiceStack.Text-V3](https://github.com/ServiceStack/ServiceStack.Text)
- [ServiceStack.Redis-V3](https://github.com/ServiceStack/ServiceStack.Redis)