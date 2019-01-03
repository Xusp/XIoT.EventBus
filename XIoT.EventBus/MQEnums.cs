using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIoT.EventBus
{
    /// <summary>
    /// MQ类型
    /// </summary>
    public enum MQTypeEnum
    {
        Local    = 1, // 本地消息
        ActiveMQ = 2,
        RabbitMQ = 3,
        Redis    = 4,
        Kafka    = 5,
        RocketMQ = 6
    }

    /// <summary>
    /// 消息优先级
    /// </summary>
    public enum MQPriority
    {
        Lowest = 0,
        VeryLow = 1,
        Low = 2,
        AboveLow = 3,
        BelowNormal = 4,
        Normal = 5,
        AboveNormal = 6,
        High = 7,
        VeryHigh = 8,
        Highest = 9
    }

    /// <summary>
    /// 消息处理结果
    /// </summary>
    public enum ProcessingResult
    {
        Accept, // 成功
        Retry,  // 重试
        Reject  // 失败
    }
}
