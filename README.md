# XIoT.EventBus
## 简介
[XIoT.EventBus](https://github.com/Xusp/EventBus)是一个基于[新生命X组件](https://github.com/NewLifeX/X)的分布式事件总线，可以跨应用触发事件。基于发布/订阅模式，消息的传递可以通过activemq, redis，rabbitmq，kafka, rocketmq等实现，支持控制台和web应用。

目前，已经实现了activemq、rabbitmqg。

本组件可能简化对消息发布和订阅的操作处理工作，让开发人员将精力集中在业务逻辑的处理上，发布消息时只需要通过IEventBus.Publish(topic, EventMessage)，订阅消息时使用IEventBus.Subscribe(topic, EventHandler)即可。其中，EventHandler为订阅端接收到消息后的处理器。

## 使用

### IEventBus接口

```
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="message">消息体，注意所有发布的消息均须为EventMessage的子类，方便事件总线底层进行通讯处理。</param>
        /// <param name="priority">消息优先级</param>
        void Publish(String topic, EventMessage message, MQPriority priority = MQPriority.Normal);
        /// <summary>
        /// 异步发布消息
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="message">消息体</param>
        /// <param name="priority">消息优先级</param>
        /// <returns></returns>
        Task PublishAsync(String topic, EventMessage message, MQPriority priority = MQPriority.Normal);
        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">消息事件处理器，用于在接收到指定主题的消息时对该消息进行相关业务处理工作，<seealso cref="EventHandler"/></param>
        void Subscribe(String topic, EventHandler handler);
        /// <summary>
        /// 异步订阅消息
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">消息事件处理器</param>
        /// <returns></returns>
        Task SubscribeAsync(String topic, EventHandler handler);
        /// <summary>
        /// 取消消息订阅
        /// </summary>
        /// <param name="topics">主题列表</param>
        void Unsubscribe(IEnumerable<String> topics);
        /// <summary>
        /// 异步取消消息订阅
        /// </summary>
        /// <param name="topics">消息列表</param>
        /// <returns></returns>
        Task UnsubscribeAsync(IEnumerable<String> topics);
        /// <summary>
        /// 取消该事件总线上所有的消息订阅
        /// </summary>
        void UnsubscribeAll();
        /// <summary>
        /// 异步取消该事件总线上所有的消息订阅
        /// </summary>
        /// <returns></returns>
        Task UnsubscribeAllAsync();
```

### Publish
发布消息时，系统只需要创建一个指定的消息服务事件处理总线，然后通过上面提供的消息发布接口即可实现消息发布，在本实现中无论采用哪种消息服务器，都默认实现了消息的持久化，使得在系统出现异常的时候不丢失消息。
```
    IEventBus evtbus = new ActiveMQEventBus();
    var topic = "XIoT/EventBus/Test";
    evtbus.Publish(topic, new EventMessage() {
        Action = "Test",
        Body = "学无先后达者为师"
    });
```
其中，发布和订阅的消息体负载Payload采用String属性，方便通过Json序列化方式对业务需要传输的实体进行封装，也是利用这个特性实现不同消息的发布和订阅机制。Action为业务定义的动作属性。

### Subscribe
订阅消息前首先要定义好用于处理消息的事件处理器（即业务端的处理代码），继承EventHandler类，实现Handle(EventMessage evtArgs)方法，在订阅的时候底层框架会自动处理消息监听和事件分发工作。
```
    public class MyEventBusHandler : EventHandler
    {
        public override void Handle(EventMessage evtArgs)
        {
            Console.WriteLine($"{evtArgs.EventTime} - {(String)evtArgs.Body} 接收到消息， 动作名称：{evtArgs.Action}");
        }
    }

    IEventBus evtbus = new ActiveMQEventBus();
    var topics = new List<String>() { "XIoT/EventBus/Test" };
    var handler = new MyEventBusHandler();
    evtbus.Subscribe(topics, handler);
```

## 资源

1.[X组件](https://github.com/NewLifeX/X)

2.[ActiveMQ](http://activemq.apache.org/)

3.[RabbitMQ](http://rabbitmq.com)

4.[EasyNETQ](http://github.com/EasyNETQ)