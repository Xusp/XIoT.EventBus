using NewLife.Log;
using NewLife.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIoT.EventBus
{
    /// <summary>
    /// 消息总线抽象基类，封装消息管理的公共方法
    /// </summary>
    /// <seealso cref="MQ.IEventBus" />
    public abstract class EventBusBase : IEventBus
    {
        #region 属性
        public IMessagePublisher Publisher { get; protected set; }
        public IMessageSubscriber Subscriber { get; protected set; }
        public IObjectContainer Container { get { return ObjectContainer.Current; } }

        /// <summary>
        /// MQ类型
        /// </summary>
        /// <value>The type of the mq.</value>
        public MQTypeEnum MQType { get; protected set; }

        #endregion

        #region 发布消息
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="message">消息体</param>
        public void Publish(string topic, EventMessage message, MQPriority priority = MQPriority.Normal)
        {
            Publisher.Publish(topic, message, priority);
        }

        /// <summary>
        /// 异步发布消息
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="message">消息体</param>
        /// <returns>Task.</returns>
        public async Task PublishAsync(string topic, EventMessage message, MQPriority priority = MQPriority.Normal)
        {
            await Publisher.PublishAsync(topic, message, priority);
        }

        #endregion

        #region 订阅消息
        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="topics">消息主题.</param>
        /// <param name="handler">消息事件处理器</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Subscribe(String topic, EventHandler handler)
        {
            Subscriber.Subscribe(topic, handler);
        }

        /// <summary>
        /// 异步订阅消息
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">消息事件处理器</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task SubscribeAsync(String topic, EventHandler handler)
        {
            await Subscriber.SubscribeAsync(topic, handler);
        }

        /// <summary>
        /// 取消订阅消息
        /// </summary>
        /// <param name="topics">消息主题</param>
        public void Unsubscribe(IEnumerable<string> topics)
        {
            Subscriber.Unsubscribe(topics);
        }

        /// <summary>
        /// 取消所有订阅消息
        /// </summary>
        public void UnsubscribeAll()
        {
            Subscriber.UnsubscribeAll();
        }

        /// <summary>
        /// 异步取消所有订阅消息
        /// </summary>
        /// <returns>Task.</returns>
        public async Task UnsubscribeAllAsync()
        {
            await Subscriber.UnsubscribeAllAsync();
        }

        /// <summary>
        /// 异步取消订阅消息
        /// </summary>
        /// <param name="topics">The topics.</param>
        /// <returns>Task.</returns>
        public async Task UnsubscribeAsync(IEnumerable<string> topics)
        {
            await Subscriber.UnsubscribeAsync(topics);
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Publisher.Dispose();
                    Subscriber.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~EventBusBase() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public virtual void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion

        #region 方法
        #endregion
    }
}
