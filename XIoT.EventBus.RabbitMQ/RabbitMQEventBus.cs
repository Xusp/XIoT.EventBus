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
        private IBus bus;
        public readonly String QueuePrefix = $"XIoT.EBQ.";

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMQEventBus"/> class.
        /// </summary>
        public RabbitMQEventBus()
        {
            MQType = MQTypeEnum.RabbitMQ;
            XTrace.WriteLine($"初始化消息服务 {Enum.GetName(typeof(MQTypeEnum), MQType)} ……");
            var setting = MQSetting.Current;
            ServerUri = setting.ServerUri.Trim();
            UserName = setting.User;
            Password = setting.Password;
            if (setting.MQType != MQTypeEnum.RabbitMQ) {
                setting.MQType = MQTypeEnum.RabbitMQ;
                setting.SaveAsync();
            }

            Publisher = new RabbitMQPublisher(this);
            Subscriber = new RabbitMQSubscriber(this);
            Container.Register<IEventBus>(this); // 注册事件总线
         
            XTrace.WriteLine($"初始化消息服务 {Enum.GetName(typeof(MQTypeEnum), MQType)} 完成。");
        }

        #region IRemoteEventBus 接口
        /// <summary>
        /// 消息服务器地址，可以包括协议名称和端口号
        /// </summary>
        public string ServerUri { get ; set; }
        /// <summary>
        /// 登录用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        public void Start() {
        }

        public void Stop() {
        }
        #endregion

        /// <summary>
        /// 消息处理总线
        /// </summary>
        /// <returns></returns>
        public IBus Bus 
        {
            get
            {
                if (bus == null) {
                    bus = RabbitHutch.CreateBus($"host={ServerUri};username={UserName};password={Password}");
                }
                return bus;
            }
        }
    }
}
