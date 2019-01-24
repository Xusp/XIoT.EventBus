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
        String Body { get; set; }
        /// <summary>
        /// 消息标签
        /// </summary>
        String Tag { get; set; }
        /// <summary>
        /// 自定义参数项
        /// </summary>
        IDictionary<String, String> Data { get; }
        /// <summary>
        /// 消息时间
        /// </summary>
        /// <value>The event time.</value>
        DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 消息数据
    /// </summary>
    /// <seealso cref="MQ.IMQMessageBase" />
    [Serializable]
    public class EventMessage : IEventMessage
    {
        private readonly IDictionary<String, String> data = new Dictionary<String, String>();
        public String Action { get; set; }
        public String Body { get; set ; }
        public String Tag { get; set; }
        public IDictionary<String, String> Data => data;
        public DateTime CreateTime { get; set; }

        public EventMessage()
        {
            CreateTime = DateTime.Now;
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
