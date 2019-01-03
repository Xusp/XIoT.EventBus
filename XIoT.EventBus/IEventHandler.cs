using System;
using System.Collections.Generic;

namespace XIoT.EventBus
{
    /// <summary>
    /// MQ消息接口
    /// </summary>
    public interface IEventMessage
    {
        /// <summary>
        /// 动作
        /// </summary>
        String Action { get; set; }
        /// <summary>
        /// 消息体
        /// </summary>
        String Payload { get; set; }

        DateTime EventTime { get; set; }
        /// <summary>
        /// 自定义项
        /// </summary>
        IDictionary<String, String> Items { get; }
    }

    /// <summary>
    /// 消息数据
    /// </summary>
    /// <seealso cref="MQ.IMQMessageBase" />
    [Serializable]
    public class EventMessage : IEventMessage
    {
        private readonly IDictionary<String, String> items = new Dictionary<String, String>();
        public String Action { get; set; }
        public String Payload { get; set ; }
        public DateTime EventTime { get; set; }
        public IDictionary<String, String> Items => items;

        public EventMessage()
        {
            EventTime = DateTime.Now;
        }
    }

    /// <summary>
    /// 事件处理器接口
    /// </summary>
    /// <typeparam name="TMQMessage"></typeparam>
    public interface IEventHandler<TMQMessage> where TMQMessage : IEventMessage
    {
        void Handle(TMQMessage evtArgs);
    }

    /// <summary>
    /// 消息事件处理器
    /// </summary>
    public abstract class EventHandler : IEventHandler<EventMessage>
    {
        /// <summary>
        /// 处理消息事件
        /// </summary>
        /// <param name="eventArgs"></param>
        public abstract void Handle(EventMessage evtArgs);
    }
}
