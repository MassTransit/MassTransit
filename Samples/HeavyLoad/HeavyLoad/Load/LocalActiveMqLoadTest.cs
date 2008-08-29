namespace HeavyLoad.Load
{
    public class LocalActiveMqLoadTest : LocalLoadTest
    {
        private static readonly string _queueUri = "activemq://localhost:61616/load_test_queue";

        public LocalActiveMqLoadTest()
            : base()
        {
        }
    }
}