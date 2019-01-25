using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aliyun.MNS;
using Aliyun.MNS.Model;
using NewLife;
using NewLife.Log;
using NewLife.Serialization;

namespace XIoT.EventBus.AliyunMNS
{
    public class AliyunMNSPublisher : IMessagePublisher
    {
        private const String SubscriptionName = "XIoT.EventBus.AliyunMNS";
        private const String QueueName = "XIoT.EventBus.AliyunMNS.Queue";
        private AliyunMNSEventBus _eventbus;
        private readonly IMNS _client;
        private readonly ConcurrentDictionary<String, Topic> _topics = new ConcurrentDictionary<string, Topic>();
        private Boolean _disposed = false;

        public AliyunMNSPublisher(IRemoteEventBus eventBus)
        {
            _eventbus = (AliyunMNSEventBus) eventBus;
            try
            {
                _client = new MNSClient(_eventbus.AccessKeyId, _eventbus.AccessKeySecret, _eventbus.Endpoint);
            }
            catch (Exception ex)
            {
                XTrace.WriteLine($"创建阿里云消息发送器失败，请检查配置文件或阿里云配置是否正确。");
                XTrace.WriteException(ex);
                throw ex;
            }
        }

        #region IMessagePublisher 接口
        public void Dispose()
        {
            if (!_disposed)
            {
                foreach (var topic in _topics.Keys)
                {
                    _client.DeleteTopic(topic);
                }

                _eventbus = null;
                _disposed = true;
            }
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="message">消息体，注意所有发布的消息均须为EventMessage的子类，方便事件总线底层进行通讯处理。</param>
        /// <param name="priority">消息优先级</param>
        public void Publish(string topic, EventMessage message, MQPriority priority = MQPriority.Normal)
        {
            var publisher = GetTopic(topic);
            if (publisher == null) return;

            // 发送消息
            switch (message.Action)
            {
                // 发送短信
                case "SMS":
                    SendSms(message, publisher);
                    break;

                case "MAIL":
                    SendMail(message, publisher);
                    break;

                default:
                    publisher.PublishMessage(message.ToJson()); // 默认情况下直接将消息发送出去
                    break;
            }
        }

        /// <summary>
        /// 异步发布消息
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="message">消息体</param>
        /// <param name="priority">消息优先级</param>
        /// <returns>Task.</returns>
        public Task PublishAsync(string topic, EventMessage message, MQPriority priority = MQPriority.Normal)
        {
            return Task.Run(() => Publish(topic, message, priority));
        }

        #endregion

        #region 辅助函数
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="message">消息体</param>
        /// <param name="publisher">主题</param>
        private void SendMail(IEventMessage message, Topic publisher)
        {
            try
            {
                var request = new PublishMessageRequest();
                var mailAttrs = new MailAttributes
                {
                    AccountName = message.Data["AccountName"], // 接件邮箱
                    Subject = message.Data["Subject"],         // 邮件主题
                    IsHtml = false,
                    ReplyToAddress = false,
                    AddressType = 0
                };
                var messageAttrs = new MessageAttributes();
                messageAttrs.MailAttributes = mailAttrs;
                request.MessageAttributes = messageAttrs;
                request.MessageTag = message.Tag;
                request.MessageBody = message.Body;
                publisher.PublishMessage(request);

                // 检查发送结果
                var queue = _client.CreateQueue(QueueName);
                var response = queue.ReceiveMessage(30);
                XTrace.WriteLine($"发送邮件：{response.Message.Body} ");
            }
            catch (Exception ex)
            {
                XTrace.WriteLine("发送邮件失败。");
                XTrace.WriteException(ex);
            }
        }

        /// <summary>
        /// 发送短消息
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="publisher">The publisher.</param>
        private static void SendSms(IEventMessage message, Topic publisher)
        {
            try
            {
                var response = publisher.Subscribe(SubscriptionName + "batchsms", publisher.GenerateBatchSmsEndpoint());
                var request = new PublishMessageRequest();
                var batchAttrs = new BatchSmsAttributes
                {
                    FreeSignName = message.Data["FreeSignName"], // 短信签名
                    TemplateCode = message.Data["TemplateCode"]  // 短信模板
                };

                // 分解短信发送参数
                Dictionary<String, String> param = new Dictionary<String, String>();
                foreach (var kv in message.Data)
                {
                    if (!kv.Key.EqualIgnoreCase("FreeSignName", "TemplateCode", "PhoneNumbers"))
                    {
                        param.Add(kv.Key, kv.Value);
                    }
                }

                // 添加接收短信的号码
                var phoneNumbers = message.Data["PhoneNumbers"].Split(",", ";", "|");
                foreach (var phone in phoneNumbers) {
                    batchAttrs.AddReceiver(phone, param);
                }

                var messageAttrs = new MessageAttributes { BatchSmsAttributes = batchAttrs };
                request.MessageAttributes = messageAttrs;
                request.MessageBody = message.Body;
                request.MessageTag = message.Tag;
                publisher.PublishMessage(request); // 发送消息
            }
            catch (Exception ex)
            {
                XTrace.WriteLine($"发送短信失败：{message.ToJson()}");
                XTrace.WriteException(ex);
            }
        }

        /// <summary>
        /// 获取指定名称的Topic
        /// </summary>
        /// <param name="topicName">Name of the topic.</param>
        /// <returns>Topic.</returns>
        private Topic GetTopic(String topicName)
        {
            if (_topics.ContainsKey(topicName))
                return _topics[topicName];

            // 创建消息主题
            Topic topic = null;
            try
            {
                var topicRequest = new CreateTopicRequest {TopicName = topicName};
                _client.DeleteTopic(topicName);
                topic = _client.CreateTopic(topicRequest);
                _topics.TryAdd(topicName, topic);
            }
            catch (Exception ex)
            {
                XTrace.WriteLine($"创建消息发送主题 {topicName} 失败。");
                XTrace.WriteException(ex);
            }

            return topic;
        }
        #endregion

    }
}
