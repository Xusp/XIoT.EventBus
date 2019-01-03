using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIoT.EventBus
{
    /// <summary>
    /// 消息事件总线接口
    /// </summary>
    public interface IEventBus : IMessagePublisher, IMessageSubscriber
    {
        /// <summary>
        /// MQ类型
        /// </summary>
        MQTypeEnum MQType { get; }

    }
}
