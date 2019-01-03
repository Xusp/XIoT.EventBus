using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using NewLife.Collections;
using NewLife.Log;

namespace XIoT.EventBus.RabbitMQ
{
    /// <summary>
    /// RabbitMQ 事件总线
    /// </summary>
    public class RabbitMQEventBus : EventBusBase, IRemoteEventBus, IDisposable
    {
        public readonly String Exchange = $"xiot.eventbus.exchange";
        public readonly String QueuePrefix = $"XIoT.EBQ.";

        public RabbitMQEventBus()
        {
            MQType = MQTypeEnum.RabbitMQ;
            XTrace.WriteLine($"初始化消息服务 {Enum.GetName(typeof(MQTypeEnum), MQType)} ……");
            var setting = MQSetting.Current;
            ServerUri = setting.ServerUri.Trim();
            UserName = setting.User;
            Password = setting.Password;

            Publisher = new RabbitMQPublisher(this);
            Subscriber = new RabbitMQSubscriber(this);
         
            XTrace.WriteLine($"初始化消息服务 {Enum.GetName(typeof(MQTypeEnum), MQType)} 完成。");
        }

        #region 连接池
        /// <summary>
        /// 消息服务器连接池
        /// </summary>
        class BusPool : ObjectPool<IBus>
        {
            public RabbitMQEventBus Instance { get; set; }

            protected override IBus OnCreate()
            {
                var evtbus = Instance;

                return RabbitHutch.CreateBus($"host={evtbus.ServerUri};username={evtbus.UserName};password={evtbus.Password}");
            }
        }

        private BusPool _Pool;

        /// <summary>
        /// 连接池
        /// </summary>
        public IPool<IBus> Pool
        {
            get
            {
                if (_Pool != null) return _Pool;

                lock (this) {
                    if (_Pool != null) return _Pool;

                    var pool = new BusPool() {
                        Name = "RabbitMQ_EventBus_Pool",
                        Instance = this,
                        Min = 2,
                        Max = 1000,
                        IdleTime = 20,
                        AllIdleTime = 120
                    };

                    return _Pool = pool;
                }
            }
        }

        #endregion 

        #region IRemoteEventBus 接口
        public string ServerUri { get ; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        #endregion
    }
}
