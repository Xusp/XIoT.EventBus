using NewLife.Log;
using NewLife.Serialization;
using System;
using System.Threading.Tasks;

namespace XIoT.EventBus.RabbitMQ
{

    /// <summary>
    /// RabbitMQ 消息发布器
    /// </summary>
    public class RabbitMQPublisher : IMessagePublisher
    {
        private RabbitMQEventBus eventBus;
        private bool _disposed = false;

        public RabbitMQPublisher(IRemoteEventBus eventbus)
        {
            eventBus = eventbus as RabbitMQEventBus;
        }

        public void Dispose()
        {
            if (!_disposed) {
                eventBus = null;
                _disposed = true;
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        /// <param name="priority"></param>
        public void Publish(string topic, EventMessage message, MQPriority priority = MQPriority.Normal)
        {
            var bus = eventBus.Pool.Get();
            try
            {
                bus.Publish(message, topic);
            }
            catch(Exception ex) {
                XTrace.WriteLine($"发送消息 {topic} - {message.ToJson()} 失败。");
                XTrace.WriteException(ex);
                throw ex;
            }
            finally {
                eventBus.Pool.Put(bus);
            }
        }

        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public async Task PublishAsync(string topic, EventMessage message, MQPriority priority = MQPriority.Normal)
        {
            var bus = eventBus.Pool.Get();
            try
            {
                await bus.PublishAsync(message, topic);
            }
            catch (Exception ex)
            {
                XTrace.WriteLine($"发送消息 {topic} - {message.ToJson()} 失败。");
                XTrace.WriteException(ex);
                throw ex;
            }
            finally
            {
                eventBus.Pool.Put(bus);
            }
        }
    }
}
