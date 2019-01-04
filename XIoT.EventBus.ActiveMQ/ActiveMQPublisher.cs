using Apache.NMS;
using Apache.NMS.ActiveMQ;
using NewLife.Log;
using NewLife.Serialization;
using NewLife.Threading;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace XIoT.EventBus.ActiveMQ
{
    /// <summary>
    /// ActiveMQ 消息发布者
    /// </summary>
    /// <seealso cref="XIoT.EventBus.IMessagePublisher" />
    public class ActiveMQPublisher : IMessagePublisher
    {
        private readonly ActiveMQEventBus eventBus;

        private ISession session;
        private Boolean _disposed = false;
        private readonly ConcurrentDictionary<String, IMessageProducer> producers = new ConcurrentDictionary<string, IMessageProducer>();
        private ConcurrentDictionary<String, ConcurrentQueue<EventMessage>> republishQueues = new ConcurrentDictionary<string, ConcurrentQueue<EventMessage>>();
        private readonly TimerX timer;

        public ActiveMQPublisher(IRemoteEventBus eventbus)
        {
            eventBus = eventbus as ActiveMQEventBus;
            timer = new TimerX(RePublish, null, 1000 * 60, 1000 * 10);
        }

        public void Dispose()
        {
            if (!_disposed) {
                foreach (var producer in producers.Values) {
                    producer.Close();
                }
                session.Close();
                _disposed = true;
            }
        }

        private Int32 retryTimes = 0;
        private Int32 MaxRetryTimes { get => 5; }
        private readonly Object syncObj = new object();
        /// <summary>
        /// 发送指定主题的消息
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="message">The MSG data.</param>
        /// <param name="priority">The priority.</param>
        /// <exception cref="System.Exception"></exception>
        public void Publish(string topic, EventMessage message, MQPriority priority = MQPriority.Normal)
        {
            var producer = GetProducer(topic);
            if (producer == null)
                throw new Exception($"不存在主题为 {topic} 的消息发布者。");

            try
            {
                ITextMessage msg = producer.CreateTextMessage();
                msg.Text = message.ToJson();
                foreach (var kv in message.Items)
                {
                    msg.Properties.SetString(kv.Key, kv.Value);
                }
                producer.Send(msg, MsgDeliveryMode.Persistent, (MsgPriority)priority, TimeSpan.MinValue); // 发送消息
                retryTimes = 0; // 重置失败重试次数
            }
            catch (IOException ex) {
                XTrace.WriteLine($"发布消息 {topic} 失败，消息内容：{message.ToString()}。");
                XTrace.WriteException(ex);
                Thread.Sleep(1000 * 60); // 延迟1分钟后再试
                RetryPublish(topic, message, ex);
            }
            catch (ConnectionClosedException ex) {
                XTrace.WriteException(ex);
                Thread.Sleep(1000 * 10); // 延迟10秒钟再试
                RetryPublish(topic, message, ex);
            }
            catch (Exception ex) {
                XTrace.WriteException(ex);
                PushRepublishMessage(topic, message);
                producers.Remove(topic);
                throw ex;
            }
        }

        /// <summary>
        /// Publishes the asynchronous.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="message">The MSG data.</param>
        /// <returns>Task.</returns>
        public Task PublishAsync(string topic, EventMessage message, MQPriority priority = MQPriority.Normal)
        {
            return Task.Factory.StartNew(() => {
                Publish(topic, message, priority);
            });
        }

        #region 辅助方法
        /// <summary>
        /// 失败消息重发
        /// </summary>
        /// <param name="state"></param>
        private void RePublish(object state)
        {
            foreach (var kv in republishQueues) {
                var topic = kv.Key;
                foreach (var msg in kv.Value) {
                    Publish(topic, msg);
                }
                republishQueues.Remove(topic); // 清除重发队列
            }
        }

        /// <summary>
        /// 重发该消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="msgData"></param>
        private void RetryPublish(String topic, EventMessage msgData, Exception ex)
        {
            if (retryTimes < MaxRetryTimes)
            {
                lock (syncObj)
                {
                    retryTimes++;
                    XTrace.WriteLine($"发送消息：{msgData.ToString()} 失败，进行第 {retryTimes} 重试。");
                    Publish(topic, msgData);
                }
            }
            else {
                PushRepublishMessage(topic, msgData);
                producers.Remove(topic); // 移除该消息生产者，下次重新生成一个
                throw ex;
            }
        }

        /// <summary>
        /// 将发送失败的数据放入重发队列
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="data"></param>
        private void PushRepublishMessage(String topic, EventMessage data)
        {
            if (!republishQueues.ContainsKey(topic))
            {
                var queue = new ConcurrentQueue<EventMessage>();
                queue.Enqueue(data);
                republishQueues.TryAdd(topic, queue);
            }
            else {
                republishQueues[topic].Enqueue(data);
            }
        }

        /// <summary>
        /// 根据消息主题返回消息生产者
        /// </summary>
        /// <param name="topic">主题</param>
        private IMessageProducer GetProducer(String topic)
        {

            if (!producers.ContainsKey(topic))
            {
                try {
                    var conn = eventBus.Connection;
                    if (!conn.IsStarted) conn.Start();
                    if (session == null)
                        session = conn.CreateSession(AcknowledgementMode.AutoAcknowledge);

                    var producer = session.CreateProducer(session.GetTopic(topic));
                    producers.TryAdd(topic, producer); // 添加生产者
                }
                catch (BrokerException ex) {
                    XTrace.WriteLine("ActiveMQ 消息服务器异常关闭。");
                    XTrace.WriteException(ex);
                    throw ex;
                }
            }

            return producers[topic];
        }

        #endregion
    }
}
