using NewLife.Log;
using NewLife.Serialization;
using NewLife.Threading;
using System;
using XCode.Membership;
using XIoT.EventBus.ActiveMQ;
using XIoT.EventBus.RabbitMQ;

namespace XIoT.EventBus.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            XTrace.UseConsole();

            TestEventBus(MQSetting.Current.MQType);

            Console.ReadKey();
        }


        /// <summary>
        /// 测试ActvieMQ 消息发布与订阅
        /// </summary>
        /// <param name="isServer"></param>
        private static void TestEventBus(MQTypeEnum mqType, Boolean isServer = true)
        {
            Console.WriteLine("消息测试：1，消息发布；2，消息订阅");

            isServer = Console.ReadKey().KeyChar == '1';

            var topic1 = "Topic.Test";
            var topic2 = "Topic.Admin.User";
            IEventBus eventbus;
            switch (mqType) {
                case MQTypeEnum.ActiveMQ:
                    eventbus = new ActiveMQEventBus();
                    break;

                case MQTypeEnum.RabbitMQ:
                    eventbus = new RabbitMQEventBus();
                    break;

                default:
                    eventbus = new RabbitMQEventBus();
                    break;
            }
            Int64 userid = UserX.FindCount() + 1;

            TimerX timer;
            if (isServer)
            {
                timer = new TimerX((Object state) =>
                {
                    try
                    {
                        var time = DateTime.Now;
                        eventbus.Publish(topic1, new EventMessage()
                        {
                            Action = "Test",
                            Payload = "学无先后达者为师"
                        });
                        Console.WriteLine($"{time} 发送了一条消息，主题为：{topic1}");

                        // 实体消息测试
                        long tick = DateTime.Now.Ticks;
                        var rd = new Random((Int32)(tick & 0xFFFFFFFFL) | (Int32)(tick >> 32));
                        var user = new UserX()
                        {
                            Name = "User" + rd.Next(),
                            DisplayName = $"{Enum.GetName(typeof(MQTypeEnum), eventbus.MQType)}_{userid++}",
                            Code = userid.ToString().PadLeft(7, '0')
                        };
                        //user.SaveAsync(); // 向数据库中插入数据
                        eventbus.PublishAsync(topic2, new EventMessage()
                        {
                            Action = "Insert",
                            Payload = user.ToJson()
                        });
                    }
                    catch (Exception ex)
                    {
                        XTrace.WriteException(ex);
                        throw ex;
                    }
                }, null, 10, 10);
            }
            else
            {
                var handler = new EventBusHandler();
                eventbus.Subscribe(topic1, handler);

                eventbus.SubscribeAsync(topic2, new MyHandler());
            }
        }
    }
}
