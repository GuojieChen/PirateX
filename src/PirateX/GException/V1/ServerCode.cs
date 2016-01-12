using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GException.V1
{
    /// <summary>
    /// 服务器状态码
    /// <b>区间是[100,599]</b>
    /// </summary>
    public enum ServerCode : short
    {
        /// <summary>
        /// 成功接收数据，并且正确执行业务逻辑
        /// </summary>
        Successful = 200,
        /// <summary>
        /// 错误请求，可能是Json数据解析错误 或者参数错误
        /// </summary>
        [PExceptionMessage("错误请求 [{0}]")]
        BadRequest = 400,
        /// <summary>
        /// 未授权，请求要求进行身份验证
        /// </summary>
        [PExceptionMessage("未授权，请求要求进行身份验证")]
        Unauthorized = 401,
        /// <summary>
        /// 无效的请求，可能是没有找到对应的方法
        /// </summary>
        [PExceptionMessage("{0}")]
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
        [PExceptionMessage("您的账号已在其他设备登陆，请重新登陆")]
        ReLogin = 413,

        /// <summary>
        /// 角色被封停
        /// </summary>
        [PExceptionMessage("该角色已被封停")]
        RoleStoped = 415,

        [PExceptionMessage("维护关闭中，不可操作")]
        ServerClosing = 416,

        [PExceptionMessage("{0}")]
        ServerClosed = 417,

        /// <summary>
        /// 内部服务器错误
        /// </summary>
        [PExceptionMessage("出错 .[{0}]")]
        InternalServerError = 500,
        /// <summary> 初始化失败
        /// </summary>
        [PExceptionMessage("游戏服初始化失败，没有正确进行配置")]
        InitFail = 510,

        [PExceptionMessage("发生多个异常")]
        MultiExp = 504,

        [PExceptionMessage("操作失败，请重试")]
        RemoteError = 506,

        [PExceptionMessage("网络异常，请重新登录")]
        ReLoginRequired = 507,

        [PExceptionMessage("版本过低，重新打开客户端试试")]
        VersionLow = 508,

        /// <summary>
        /// 配置数据缺少
        /// </summary>
        [PExceptionMessage("{0}")]
        ConfigDataNull = 599,

    }
}
