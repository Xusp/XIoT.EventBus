using EasyNetQ;
using EasyNetQ.NonGeneric;
using NewLife.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XIoT.EventBus.RabbitMQ
{
    /// <summary>
    /// RabbitMQ 消息订阅器
    /// </summary>
    public class RabbitMQSubscriber : IMessageSubscriber
    {
        private readonly ConcurrentDictionary<String, ISubscriptionResult> consumers = new ConcurrentDictionary<string, ISubscriptionResult>();
        private RabbitMQEventBus eventBus;
        private IBus bus;
        private Boolean _disposed = false;

        public RabbitMQSubscriber(IRemoteEventBus eventbus)
        {
            eventBus = eventbus as RabbitMQEventBus;
            bus = eventBus.GetRabbitBus();
        }
        public void Dispose()
        {
            if (!_disposed) {
                UnsubscribeAll();
                bus.Dispose();
                eventBus.Dispose();
                _disposed = true;
            }
        }

        public void Subscribe(string topic, EventHandler handler)
        {
            try
            {
                if (!consumers.ContainsKey(topic)) {
                    var clientid = handler.GetType().GetCustomAttributes(typeof(EventHandlerAttribute), true).FirstOrDefault() is EventHandlerAttribute attr ? attr.ClientId : eventBus.QueuePrefix + handler.GetType().Name;
                    var consumer = bus.Subscribe<EventMessage>(clientid, handler.Handle, conf => conf.WithTopic(topic));
                    consumers.TryAdd(topic, consumer);
                }
            }
            catch (Exception ex)
            {
                XTrace.WriteLine($"订阅消息:{topic} 失败。");
                XTrace.WriteException(ex);
                throw ex;
            }
        }

        public Task SubscribeAsync(string topic, EventHandler handler)
        {
            return Task.Factory.StartNew(() => Subscribe(topic, handler));
        }

        public void Unsubscribe(IEnumerable<string> topics)
        {
            foreach (var topic in topics) {
                if (consumers.ContainsKey(topic)) {
                    consumers[topic].Dispose();
                }
            }
        }

        public void UnsubscribeAll()
        {
            Unsubscribe(consumers.Select(p => p.Key));
        }

        public Task UnsubscribeAllAsync()
        {
            return Task.Factory.StartNew(UnsubscribeAll);
        }

        public Task UnsubscribeAsync(IEnumerable<string> topics)
        {
            return Task.Factory.StartNew(() => Unsubscribe(topics));
        }
    }
}
