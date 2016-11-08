namespace PirateX
{
    /// <summary>
    /// 服务器系统状态码
    /// <b>区间是[100,599]</b>
    /// </summary>
    public enum StatusCode : short
    {
        /// <summary>
        /// 成功接收数据，并且正确执行业务逻辑
        /// </summary>
        Ok = 200,
        /// <summary>
        /// 错误请求，可能是Json数据解析错误 或者参数错误
        /// </summary>
        BadRequest = 400,
        /// <summary>
        /// 未授权，请求要求进行身份验证
        /// </summary>
        Unauthorized = 401,
        /// <summary>
        /// 无效的请求，可能是没有找到对应的方法
        /// </summary>
        NotFound = 404,
        /// <summary>
        /// 种子重复生成
        /// </summary>
        SeedReCreate = 406,
        /// <summary>
        /// 请求头缺少
        /// </summary>
        LengthRequired = 411,
        /// <summary>
        /// Seed没有创建,Seed创建是一切的开始
        /// </summary>
        PreconditionFailed = 412,
        /// <summary> 账号已在其他设备上登录 </summary>
        ReLogin = 413,
        /// <summary>
        /// 不能重复登陆
        /// </summary>
        NotLoginAgain = 414,

        /// <summary>
        /// 角色被封停
        /// </summary>
        RoleStoped = 415,
        /// <summary>
        /// 维护关闭中，不可操作
        /// </summary>
        ServerClosing = 416,

        /// <summary>
        /// 内部服务器错误
        /// </summary>
        InternalServerError = 500,
        /// <summary> 游戏服初始化失败，没有正确进行配置
        /// </summary>
        InitFail = 510,
        /// <summary>
        /// 发生多个异常
        /// </summary>
        MultiExp = 504,
        /// <summary>
        /// 操作失败，请重试
        /// </summary>
        RemoteError = 506,
        /// <summary>
        /// 网络异常，请重新登录
        /// </summary>
        ReLoginRequired = 507,
        /// <summary>
        /// 版本过低，重新打开客户端试试
        /// </summary>
        VersionLow = 508,
        /// <summary>
        /// 配置数据缺少
        /// </summary>
        ConfigDataNull = 599,

    }
}
