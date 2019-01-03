using Apache.NMS;
using NewLife.Log;
using NewLife.Serialization;
using System;
using System.Collections.Generic;

namespace XIoT.EventBus.ActiveMQ
{
    /// <summary>
    /// ActiveMQ消息消费者包装器
    /// </summary>
    public class ConsumerWrapper
    {
        public IConnection Connection { get; private set; }
        public ISession Session { get; private set; }
        public IMessageConsumer Consummer { get; private set; }
        public IDictionary<Int32, EventHandler> EventHandlers { get; } = new Dictionary<Int32, EventHandler>();

        public ConsumerWrapper(IConnection connection, String clientId, String topic)
        {
            Connection = connection;
            if (!Connection.IsStarted) Connection.Start();
            Session = Connection.CreateSession(AcknowledgementMode.AutoAcknowledge);

            var consumerName = (clientId + topic).GetHashCode().ToString();
            Consummer = Session.CreateDurableConsumer(Session.GetTopic(topic), consumerName, null, true);
            Consummer.Listener += MessageListenner;
        }

        public ConsumerWrapper(ISession session, String topic)
        {
            Session = session;
            var name = topic.GetHashCode().ToString();
            Consummer = session.CreateDurableConsumer(session.GetTopic(topic), name, null, true);
            Consummer.Listener += MessageListenner;
        }

        /// <summary>
        /// 注册事件处理程序
        /// </summary>
        /// <param name="handler">The handler.</param>
        public void RegisterHandler(EventHandler handler)
        {
            var key = handler.GetHashCode();
            if (!EventHandlers.ContainsKey(key)) {
                EventHandlers.Add(key, handler);
            }
        }

        public void Close()
        {
            EventHandlers.Clear();
            Consummer.Listener -= MessageListenner;
            Consummer.Close();
            if (Session != null) Session.Close();
            if (Connection != null) Connection.Close();
        }

        #region 辅助方法

        /// <summary>
        /// 消息监听处理程序
        /// </summary>
        /// <param name="message">The message.</param>
        private void MessageListenner(IMessage message)
        {
            ITextMessage msg = (ITextMessage)message;
            var args = msg.Text.ToJsonEntity<EventMessage>();
            if (args != null) {
                foreach (var kv in EventHandlers)
                {
                    try
                    {
                        var handler = kv.Value;
                        handler.Handle(args); // 触发消息订阅事件处理程序
                    }
                    catch (Exception ex)
                    {
                        XTrace.WriteLine($"处理订阅消息事件失败, 主题:{kv.Key}, 消息体:{args}。");
                        XTrace.WriteException(ex);
                        continue; // 记录下异常情况继续处理
                    }
                }

            }
        }

        #endregion
    }
}
