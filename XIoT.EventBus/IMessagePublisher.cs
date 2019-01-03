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
        void Publish(String topic, EventMessage message, MQPriority priority = MQPriority.Normal);
        Task PublishAsync(String topic, EventMessage message, MQPriority priority = MQPriority.Normal);
    }
}
