using NewLife.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIoT.EventBus
{
    [XmlConfigFile("config/MQ.config", 15000)]
    public class MQSetting : XmlConfig<MQSetting>
    {
        [Description("消息服务类型，默认为本地消息（可以不用设置以下几个参数）")]
        public MQTypeEnum MQType { get; set; } = MQTypeEnum.ActiveMQ;

        [Description("服务器地址，含端口号。")]
        public String ServerUri { get; set; } = "activemq:tcp://127.0.0.1:61616";
        
        [Description("用户名")]
        public String UserName { get; set; } = "admin";

        [Description("登录密码")]
        public String Password { get; set; } = "admin";

        [Description("AccessKey ID，访问阿里云API的密钥")]
        public String AccessKey { get; set; } = "";
        [Description("AccessKeySecret")]
        public String AccessKeySecret { get; set; } = "";
        [Description("Endpoint")]
        public String Endpoint { get; set; } = "";
    }
}
