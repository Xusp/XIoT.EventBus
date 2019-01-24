using NewLife.Serialization;
using NewLife.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCode;
using XCode.Membership;
using XIoT.EventBus;
using XIoT.EventBus.ActiveMQ;

namespace XIoT.EventBus.Test
{
    public class EventBusHandler : EventHandler
    {
        public override void Handle(EventMessage evtArgs)
        {
            Console.WriteLine($"{evtArgs.CreateTime} - {(String)evtArgs.Body} 接收到消息， 动作名称：{evtArgs.Action}");
        }
    }

    [EventHandler("XIoT/Admin/User/Register")]
    public class MyHandler : EventHandler
    {
        public override void Handle(EventMessage evtArgs)
        {
            var user = evtArgs.Body.ToJsonEntity<UserX>();
            if (user != null) {
                Console.WriteLine($"接收到消息：{evtArgs.Action}, 发送时间：{evtArgs.CreateTime}");
                Console.WriteLine($"Code:{user.Code}, Name:{user.Name}, DisplayName:{user.DisplayName}");
                Console.WriteLine();
            }
        }
    }
}
