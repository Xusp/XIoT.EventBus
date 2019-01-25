using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIoT.EventBus.AliyunMNS
{
    public class AliyunMNSSubscriber : IMessageSubscriber
    {
        private readonly AliyunMNSEventBus eventbus;

        public AliyunMNSSubscriber(IRemoteEventBus eventBus)
        {
            eventbus = (AliyunMNSEventBus) eventBus;
        }

        #region IMessageSubscriber 接口
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Subscribe(string topic, EventHandler handler)
        {
            throw new NotImplementedException();
        }

        public Task SubscribeAsync(string topic, EventHandler handler)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(IEnumerable<string> topics)
        {
            throw new NotImplementedException();
        }

        public Task UnsubscribeAsync(IEnumerable<string> topics)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeAll()
        {
            throw new NotImplementedException();
        }

        public Task UnsubscribeAllAsync()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
