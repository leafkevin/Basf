using RabbitMQ.Client;

namespace Basf.Rabbitmq
{
    public class ChannelPool
    {
        private static ConnectionFactory objConnFactory = null;
        private static string ConnectionUri = Utility.GetAppSettingValue("RabbitUri","");
        //private BlockingCollection<Channel> objWorkerList = null;
        //private BlockingCollection<Channel> objRetryWorkerList = null;

        public ChannelPool()
        {
            if (objConnFactory == null)
            {
                objConnFactory = new ConnectionFactory { Uri = ConnectionUri, AutomaticRecoveryEnabled = true };
            }
            //objConnFactory.RequestedConnectionTimeout = 300;
            //objConnFactory.RequestedChannelMax = 2;
            //objConnFactory.RequestedHeartbeat = 10;
            //this.objWorkerList = new BlockingCollection<Channel>(MaxConnCount);
            //this.objRetryWorkerList = new BlockingCollection<Channel>(2);
            //this.objQueue = new BlockingCollection<Attendance>(BatchCount * 2);
            //this.Configure(iDataType);
            //this.objTimer = new Timer(this.ConsumerExecute, null, 0, BatchInterval);
        }
    }
}
