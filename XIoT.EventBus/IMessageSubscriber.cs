using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCode;

namespace XIoT.EventBus
{
    /// <summary>
    /// 消息订阅者
    /// </summary>
    public interface IMessageSubscriber : IDisposable
    {
        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">消息事件处理器，用于在接收到指定主题的消息时对该消息进行相关业务处理工作，<seealso cref="EventHandler"/></param>
        void Subscribe(String topic, EventHandler handler);
        /// <summary>
        /// 异步订阅消息
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">消息事件处理器</param>
        /// <returns></returns>
        Task SubscribeAsync(String topic, EventHandler handler);
        /// <summary>
        /// 取消消息订阅
        /// </summary>
        /// <param name="topics">主题列表</param>
        void Unsubscribe(IEnumerable<String> topics);
        /// <summary>
        /// 异步取消消息订阅
        /// </summary>
        /// <param name="topics">消息列表</param>
        /// <returns></returns>
        Task UnsubscribeAsync(IEnumerable<String> topics);
        /// <summary>
        /// 取消该事件总线上所有的消息订阅
        /// </summary>
        void UnsubscribeAll();
        /// <summary>
        /// 异步取消该事件总线上所有的消息订阅
        /// </summary>
        /// <returns></returns>
        Task UnsubscribeAllAsync();

    }
}
