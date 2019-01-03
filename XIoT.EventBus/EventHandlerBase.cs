using XCode;

namespace XIoT.EventBus
{
    /// <summary>
    /// 消息处理基类，用于上层业务逻辑处理中对业务的封装
    /// </summary>
    public class EventHandlerBase : IEventHandler<EventMessage>
    {
        public virtual void Handle(EventMessage evt)
        {
        }
    }
}
