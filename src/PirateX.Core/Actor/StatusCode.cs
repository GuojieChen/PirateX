namespace PirateX.Core.Actor
{
    /// <summary>
    /// 服务器系统状态码
    /// <b>区间是[100;599]</b>
    /// 
    /// 客户端接收异常之后，需要正对不同的code做出后续的逻辑
    /// </summary>
    public static class StatusCode
    {
        /// <summary>
        /// 成功接收数据，并且正确执行业务逻辑
        /// </summary>
        public const short Ok = 200;
        /// <summary>
        /// 错误请求，可能是数据解析错误 或者参数错误
        /// </summary>
        public const short BadRequest = 400;
        /// <summary>
        /// 未授权，请求要求进行身份验证
        /// </summary>
        public const short Unauthorized = 401;
        /// <summary>
        /// 容器未定义
        /// </summary>
        public const short ContainerNull = 402;

        /// <summary>
        /// 返回格式是ProtoBuf，需要设置属性
        /// </summary>
        public const short ProtobufData = 403;

        /// <summary>
        /// 无效的请求，可能是没有找到对应的方法
        /// </summary>
        public const short NotFound = 404;
        /// <summary>
        /// 种子重复生成
        /// </summary>
        public const short SeedReCreate = 406;
        /// <summary>
        /// 请求头缺少
        /// </summary>
        public const short LengthRequired = 411;
        /// <summary>
        /// Seed没有创建;Seed创建是一切的开始
        /// </summary>
        public const short PreconditionFailed = 412;
        /// <summary> 账号已在其他设备上登录 </summary>
        public const short ReLogin = 413;
        /// <summary>
        /// 不能重复登陆
        /// </summary>
        public const short NotLoginAgain = 414;

        /// <summary>
        /// 角色被封停
        /// </summary>
        public const short RoleStoped = 415;
        /// <summary>
        /// 维护关闭中，不可操作
        /// </summary>
        public const short ServerClosing = 416;

        /// <summary>
        /// 内部服务器错误
        /// </summary>
        public const short shorternalServerError = 500;
        /// <summary> 游戏服初始化失败，没有正确进行配置
        /// </summary>
        public const short InitFail = 510;

        public const short ServerError = 503;

        /// <summary>
        /// 发生多个异常
        /// </summary>
        public const short MultiExp = 504;
        /// <summary>
        /// 操作失败，请重试
        /// </summary>
        public const short RemoteError = 506;
        /// <summary>
        /// 网络异常，请重新登录
        /// </summary>
        public const short ReLoginRequired = 507;
        /// <summary>
        /// 版本过低，重新打开客户端试试
        /// </summary>
        public const short VersionLow = 508;
        /// <summary>
        /// 超时
        /// </summary>
        public const short Timeout = 509;

        /// <summary>
        /// 配置数据缺少
        /// </summary>
        public const short ConfigDataNull = 599;

        /// <summary>
        /// 逻辑处理的时候出现异常
        /// </summary>
        public const short Exception = 1000;

    }
}
