namespace MassTransit.Tests.ContainerTests.Scenarios
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class AnotherMessageConsumerImpl :
        AnotherMessageConsumer
    {
        readonly ManualResetEvent _received;
        AnotherMessageInterface _last;

        public AnotherMessageConsumerImpl()
        {
            Console.WriteLine("AnotherMessageConsumer()");

            _received = new ManualResetEvent(false);
        }

        public AnotherMessageInterface Last
        {
            get
            {
                if (_received.WaitOne(TimeSpan.FromSeconds(8)))
                    return _last;

                throw new TimeoutException("Timeout waiting for message to be consumed");
            }
        }

        public async Task Consume(ConsumeContext<AnotherMessageInterface> context)
        {
            _last = context.Message;
            _received.Set();
        }
    }
}
