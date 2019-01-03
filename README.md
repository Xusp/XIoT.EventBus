# XIoT.EventBus
## 简介
[XIoT.EventBus](https://github.com/Xusp/EventBus)是一个基于[新生命X组件](https://github.com/NewLifeX/X)的分布式事件总线，可以跨应用触发事件。基于发布/订阅模式，消息的传递可以通过activemq, redis，rabbitmq，kafka, rocketmq等实现，支持控制台和web应用。

## 使用

### Publish

```
    IEventBus evtbus = new ActiveMQEventBus();
    var topic = "XIoT/EventBus/Test";
    evtbus.Publish(topic, new MQMessageData() {
        Action = "Test",
        Message = "学无先后达者为师"
    });
```

### Subscribe

```
    public class MyEventBusHandler : MQEventHandler
    {
        public override void Handle(MQMessageData evtArgs)
        {
            Console.WriteLine($"{evtArgs.EventTime} - {(String)evtArgs.Message} 接收到消息， 动作名称：{evtArgs.Action}");
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