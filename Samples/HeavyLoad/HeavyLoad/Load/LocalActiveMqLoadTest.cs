namespace HeavyLoad.Load
{
    using System;

    public class LocalActiveMqLoadTest : LocalLoadTest
    {
        private static readonly string _queueUri = "activemq://localhost:61616/load_test_queue";

        public LocalActiveMqLoadTest()
            : base(new Uri(_queueUri))
        {
        }
    }
}