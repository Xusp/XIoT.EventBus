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

        /// <summary>
        /// 获取消息处理总线
        /// </summary>
        /// <returns></returns>
        public IBus GetRabbitBus()
        {
            return RabbitHutch.CreateBus($"host={ServerUri};username={UserName};password={Password}");
        }
    }
}
