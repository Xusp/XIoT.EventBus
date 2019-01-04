using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIoT.EventBus
{
    /// <summary>
    /// 消息发布者
    /// </summary>
    public interface IMessagePublisher : IDisposable
    {
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="message">消息体，注意所有发布的消息均须为EventMessage的子类，方便事件总线底层进行通讯处理。</param>
        /// <param name="priority">消息优先级</param>
        void Publish(String topic, EventMessage message, MQPriority priority = MQPriority.Normal);
        /// <summary>
        /// 异步发布消息
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="message">消息体</param>
        /// <param name="priority">消息优先级</param>
        /// <returns></returns>
        Task PublishAsync(String topic, EventMessage message, MQPriority priority = MQPriority.Normal);
    }
}
