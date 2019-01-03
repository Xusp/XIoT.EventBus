using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIoT.EventBus
{
    /// <summary>
    /// 消息订阅器事件特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EventHandlerAttribute : Attribute
    {
        public String ClientId { get; set; }

        public EventHandlerAttribute(String clientId)
        {
            ClientId = clientId;
        }
    }
}
