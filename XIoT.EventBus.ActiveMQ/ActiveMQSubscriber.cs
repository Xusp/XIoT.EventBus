using Apache.NMS;
using Apache.NMS.ActiveMQ;
using NewLife.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XIoT.EventBus.ActiveMQ
{
    /// <summary>
    /// ActiveMQ消息订阅者
    /// </summary>
    public class ActiveMQSubscriber : IMessageSubscriber
    {
        private readonly ActiveMQEventBus eventBus;
        private ConcurrentDictionary<String, ConsumerWrapper> wrappers = new ConcurrentDictionary<string, ConsumerWrapper>();
        private Boolean _disposed = false;

        public ActiveMQSubscriber(IRemoteEventBus eventbus)
        {
            eventBus = eventbus as ActiveMQEventBus;
        }

        public void Dispose()
        {
            if (!_disposed) {
                foreach (var wrapper in wrappers.Values) {
                    wrapper.Close();
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Subscribes the specified topics.
        /// </summary>
        /// <param name="topics">The topics.</param>
        /// <param name="handler">The handler.</param>
        public void Subscribe(String topic, EventHandler handler)
        {
            ProcessSubscribe(topic, handler);
        }

        /// <summary>
        /// Subscribes the asynchronous.
        /// </summary>
        /// <param name="topic">The topics.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>Task.</returns>
        public Task SubscribeAsync(String topic, EventHandler handler)
        {
            return Task.Factory.StartNew(() =>
            {
                ProcessSubscribe(topic, handler);
            });
        }

        /// <summary>
        /// Unsubscribes the specified topics.
        /// </summary>
        /// <param name="topics">The topics.</param>
        public void Unsubscribe(IEnumerable<string> topics)
        {
            foreach (var topic in topics) {
                if (wrappers.ContainsKey(topic)) {
                    var wrapper = wrappers[topic];
                    wrapper.Close();
                    wrappers.Remove(topic);
                }
            }
        }

        public void UnsubscribeAll()
        {
            foreach (var w in wrappers.Values) {
                w.Close();
            }
            wrappers.Clear();
        }

        public Task UnsubscribeAllAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                UnsubscribeAll();
            });
        }

        public Task UnsubscribeAsync(IEnumerable<string> topics)
        {
            return Task.Factory.StartNew(() =>
            {
                Unsubscribe(topics);
            });
        }

        #region 辅助方法

        /// <summary>
        /// 订阅消息主题，并将事件处理程序注册到该主题上
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">事件处理程序</param>
        private void ProcessSubscribe(String topic, EventHandler handler)
        {
            try
            {
                ConsumerWrapper wrapper;
                // 首先检查该订阅是否需要对消息进行持久化
                if (handler.GetType().GetCustomAttributes(typeof(EventHandlerAttribute), true).FirstOrDefault() is EventHandlerAttribute attr)
                {
                    var ws = wrappers.Values.Where(w => w.Connection != null);
                    if (!ws.Any(w => w.Connection.ClientId != attr.ClientId))
                    {
                        // 创建带ClientId的连接
                        var factory = new ConnectionFactory(eventBus.ServerUri);
                        var conn = factory.CreateConnection(eventBus.UserName, eventBus.Password);
                        conn.ClientId = attr.ClientId;
                        conn.Start();

                        wrapper = new ConsumerWrapper(conn, attr.ClientId, topic);
                        wrapper.RegisterHandler(handler);
                        wrappers.TryAdd(topic, wrapper);
                    }
                }

                if (!wrappers.ContainsKey(topic))
                {
                    var conn = eventBus.Connection;
                    if (!conn.IsStarted) conn.Start();

                    var session = conn.CreateSession(AcknowledgementMode.AutoAcknowledge);
                    wrapper = new ConsumerWrapper(session, topic);

                    wrapper.RegisterHandler(handler);
                    wrappers.TryAdd(topic, wrapper);
                }
                else {
                    wrapper = wrappers[topic];
                    if (wrapper != null)
                        wrapper.RegisterHandler(handler);
                }
            }
            catch (Exception ex)
            {
                XTrace.WriteLine($"订阅消息:{topic} 失败。");
                XTrace.WriteException(ex);
                throw ex;
            }
        }
        #endregion
    }
}
