namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class When_broker_is_not_available :
        AzureServiceBusTestFixture
    {
        [Test]
        [Explicit]
        public async Task Should_not_cache_faulted_publish_endpoint()
        {
            Console.WriteLine("Disable network connection to broker");

            await Task.Delay(10000);

            Console.WriteLine("Publishing for fault");

            try
            {
                await Bus.Publish<FirstMessage>(new {Value = "Attempt during network unavailable"});
            }
            catch (Exception ex)
            {
                Console.WriteLine("Publish faulted: " + ex.Message);
            }

            Console.WriteLine("Reconnect network");

            await Task.Delay(10000);

            try
            {
                await Bus.Publish<SecondMessage>(new {Value = "Attempt after network reconnection"});
            }
            catch (Exception ex)
            {
                Console.WriteLine("Publish faulted: " + ex.Message);
            }

            try
            {
                await Bus.Publish<FirstMessage>(new {Value = "Attempt after network reconnection"});
            }
            catch (Exception ex)
            {
                Console.WriteLine("Publish still faulted: " + ex.Message);
            }
        }


        public interface FirstMessage
        {
            string Value { get; }
        }


        public interface SecondMessage
        {
            string Value { get; }
        }
    }
}
