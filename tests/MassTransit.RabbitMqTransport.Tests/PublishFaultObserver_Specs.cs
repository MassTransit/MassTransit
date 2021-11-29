namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    [Explicit]
    public class PublishFaultObserver_Specs :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_handle_the_observer_fault()
        {
            Bus.ConnectPublishObserver(new PublishObserver());

            for (var i = 0; i < 30; i++)
            {
                try
                {
                    await Task.Delay(1000);

                    await Bus.Publish(new TestMessage { Index = i });
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync($"Publish {i} faulted: {ex.Message}");
                }
            }
        }


        class PublishObserver :
            IPublishObserver
        {
            public Task PostPublish<T>(PublishContext<T> context)
                where T : class
            {
                return Console.Out.WriteLineAsync($"PostPublish: {((Test)context.Message).Index}");
            }

            public Task PrePublish<T>(PublishContext<T> context)
                where T : class
            {
                return Console.Out.WriteLineAsync($"PrePublish: {((Test)context.Message).Index}");
            }

            public Task PublishFault<T>(PublishContext<T> context, Exception exception)
                where T : class
            {
                return Console.Out.WriteLineAsync($"PublishFault: {((Test)context.Message).Index}");
            }
        }


        public interface Test
        {
            int Index { get; }
        }


        public class TestMessage :
            Test
        {
            public int Index { get; set; }
        }
    }
}
