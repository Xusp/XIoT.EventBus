using Apache.NMS;
using Apache.NMS.ActiveMQ;
using NewLife.Log;
using System;

namespace XIoT.EventBus.ActiveMQ
{
    /// <summary>
    /// ActiveMQ消息总线
    /// </summary>
    /// <seealso cref="XIoT.EventBus.EventBusBase" />
    /// <seealso cref="XIoT.EventBus.IRemoteEventBus" />
    public class ActiveMQEventBus : EventBusBase, IRemoteEventBus, IDisposable
    {
        private IConnectionFactory connectionFactory;
        private Boolean _disposed;
        private int exceptionCount;
        private readonly int maxExceptionCount = 5;

        public IConnection Connection { get; private set; }
        public String ServerUri { get; set; }
        public String ClientId { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }

        public ActiveMQEventBus()
        {
            MQType = MQTypeEnum.ActiveMQ;
            var setting = MQSetting.Current;
            InitActiveMQ(setting);
            Publisher = new ActiveMQPublisher(this);
            Subscriber = new ActiveMQSubscriber(this);
            Container.Register<IRemoteEventBus>(this); // 注册消息总线
        }

        /// <summary>
        /// 启动服务器连接
        /// </summary>
        public void Start()
        {
            InitActiveMQ(MQSetting.Current);
        }

        /// <summary>
        /// 停止服务器连接
        /// </summary>
        public void Stop()
        {
            if (Connection.IsStarted) {
                UnsubscribeAll();
                Connection.Stop();
            }
        }

        public override void Dispose()
        {
            if (!_disposed) {
                Stop();
                _disposed = true;
            }
        }

        #region 辅助方法

        /// <summary>
        /// 初始化ActiveMQ消息服务器
        /// </summary>
        /// <param name="setting"></param>
        private void InitActiveMQ(MQSetting setting)
        {
            XTrace.WriteLine($"初始化消息服务 {Enum.GetName(typeof(MQTypeEnum), MQType)} ……");
            ServerUri = setting.ServerUri.Trim();
            UserName = setting.User;
            Password = setting.Password;
            if (connectionFactory == null)
                connectionFactory = new ConnectionFactory(ServerUri);

            // 初始化到消息服务器的连接
            try
            {
                if (Connection == null)
                    Connection = connectionFactory.CreateConnection(UserName, Password);
                if (!Connection.IsStarted)
                    Connection.Start();
            }
            catch (Exception ex)
            {
                exceptionCount++;
                XTrace.WriteException(ex);
                throw ex;
            }
            XTrace.WriteLine($"初始化消息服务 {Enum.GetName(typeof(MQTypeEnum), MQType)} 完成。");
        }

        #endregion
    }
}
