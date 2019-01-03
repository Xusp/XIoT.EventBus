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
        void Subscribe(String topic, EventHandler handler);
        Task SubscribeAsync(String topic, EventHandler handler);
        void Unsubscribe(IEnumerable<String> topics);
        Task UnsubscribeAsync(IEnumerable<String> topics);
        void UnsubscribeAll();
        Task UnsubscribeAllAsync();

    }
}
