using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aliyun.MNS;
using Aliyun.MNS.Model;
using NewLife.Log;

namespace XIoT.EventBus.AliyunMNS
{
    public class AliyunMNSSubscriber : IMessageSubscriber
    {
        private readonly AliyunMNSEventBus _eventbus;
        private readonly IMNS _client;
        private readonly ConcurrentDictionary<String, ComsumerWrapper> _comsumers = new ConcurrentDictionary<String, ComsumerWrapper>();

        public AliyunMNSSubscriber(IRemoteEventBus eventBus)
        {
        }

        #region IMessageSubscriber 接口
        public void Dispose()
        {
        }

        public void Subscribe(string topic, EventHandler handler)
        {
            ThrowException();
        }

        public Task SubscribeAsync(string topic, EventHandler handler)
        {
            throw new EventBusException("AliyunMNS当前仅作为短信、邮件发送之用，不能订阅消息。");
        }

        public void Unsubscribe(IEnumerable<string> topics)
        {
            ThrowException();
        }

        public Task UnsubscribeAsync(IEnumerable<string> topics)
        {
            throw new EventBusException("AliyunMNS当前仅作为短信、邮件发送之用，不能订阅消息。");
        }

        public void UnsubscribeAll()
        {
            ThrowException();
        }

        public Task UnsubscribeAllAsync()
        {
            throw new EventBusException("AliyunMNS当前仅作为短信、邮件发送之用，不能订阅消息。");
        }
        #endregion

        #region 辅助方法

        private void ThrowException()
        {
            throw new EventBusException("AliyunMNS当前仅作为短信、邮件发送之用，不能订阅消息。");
        }

        private ComsumerWrapper GetComsumer(String topicName)
        {
            if (_comsumers.ContainsKey(topicName))
                return _comsumers[topicName];

            ComsumerWrapper comsumer = null;
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return comsumer;
        }

        #endregion
    }
}
