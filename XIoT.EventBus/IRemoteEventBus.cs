using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIoT.EventBus
{
    /// <summary>
    /// 分布式消息总线
    /// </summary>
    /// <seealso cref="MQ.IEventBus" />
    public interface IRemoteEventBus : IEventBus
    {
        /// <summary>
        /// 服务器地址及端口号
        /// </summary>
        String ServerUri { get; set; }
        /// <summary>
        /// 用户名，用于连接服务器的凭证
        /// </summary>
        String UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        String Password { get; set; }
        /// <summary>
        /// 启动服务器连接
        /// </summary>
        void Start();
        /// <summary>
        /// 停止服务器连接
        /// </summary>
        void Stop();
    }
}
