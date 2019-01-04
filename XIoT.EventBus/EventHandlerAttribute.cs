using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIoT.EventBus
{
    /// <summary>
    /// 消息订阅器事件特性，用于在ActvieMQ及RabbitMQ中标识特定的订阅客户端，以便于消息服务器能够将消息持久化。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EventHandlerAttribute : Attribute
    {
        /// <summary>
        /// 订阅者ID
        /// ActvieMQ必须要有该ID服务器才会在订阅端退出后仍然保留该消息。
        /// RabbitMQ中相同的ID会平均分流从服务器来的消息，如果不同的ID则每个订阅者能够接收到同样的消息，再通过Topic匹配能够获取更多的灵活性。
        /// </summary>
        public String ClientId { get; set; }

        public EventHandlerAttribute(String clientId)
        {
            ClientId = clientId;
        }
    }
}
