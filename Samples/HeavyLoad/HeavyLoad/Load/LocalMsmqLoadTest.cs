namespace HeavyLoad.Load
{
    using System;
    using MassTransit.ServiceBus.MSMQ;

    public class LocalMsmqLoadTest : LocalLoadTest
    {
        private static readonly string _queueUri = "msmq://localhost/test_servicebus";

        public LocalMsmqLoadTest()
            : base(new Uri(_queueUri))
        {
            MsmqHelper.ValidateAndPurgeQueue(((MsmqEndpoint) LocalEndpoint).QueuePath);
        }
    }
}