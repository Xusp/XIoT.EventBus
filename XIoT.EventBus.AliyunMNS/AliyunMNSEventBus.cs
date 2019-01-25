using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewLife.Log;

namespace XIoT.EventBus.AliyunMNS
{
    /// <summary>
    /// 阿里云消息服务事件总线
    /// </summary>
    public class AliyunMNSEventBus : EventBusBase, IRemoteEventBus
    {
        public string ServerUri { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public String AccessKeyId { get; private set; }
        public String AccessKeySecret { get; private set; }
        public String Endpoint { get; private set; }

        public AliyunMNSEventBus()
        {
            MQType = MQTypeEnum.AliyunMNS;
            XTrace.WriteLine($"初始化消息服务 {Enum.GetName(typeof(MQTypeEnum), MQType)} ……");
            var setting = MQSetting.Current;
            ServerUri = setting.ServerUri.Trim();
            UserName = setting.UserName;
            Password = setting.Password;

            // 阿里云AK
            AccessKeyId = setting.AccessKey.Trim();
            AccessKeySecret = setting.AccessKeySecret.Trim();
            Endpoint = setting.Endpoint.Trim();

            if (setting.MQType != MQTypeEnum.AliyunMNS)
            {
                setting.MQType = MQTypeEnum.AliyunMNS;
                setting.SaveAsync();
            }

            Publisher = new AliyunMNSPublisher(this);
            Subscriber = new AliyunMNSSubscriber(this);
            Container.Register<IEventBus>(this); // 注册事件总线

            XTrace.WriteLine($"初始化消息服务 {Enum.GetName(typeof(MQTypeEnum), MQType)} 完成。");
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
