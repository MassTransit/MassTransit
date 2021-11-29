namespace MassTransit.Tests
{
    namespace PingDefinitions
    {
        using System;
        using System.Threading.Tasks;
        using TestFramework.Messages;


        public interface SubmitOrder
        {
            Guid OrderId { get; }

            string CustomerId { get; }
        }


        public interface OrderAccepted
        {
            Guid OrderId { get; }

            string CustomerId { get; }
        }


        public interface OrderRejected
        {
            Guid OrderId { get; }

            string CustomerId { get; }
        }


        public interface OrderReceived
        {
            Guid OrderId { get; }

            string CustomerId { get; }
        }


        public interface ProcessOrder
        {
            Guid OrderId { get; }

            string CustomerId { get; }
        }


        public class PingConsumer :
            IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
            }
        }


        public class SubmitOrderConsumer :
            IConsumer<SubmitOrder>
        {
            public async Task Consume(ConsumeContext<SubmitOrder> context)
            {
            }
        }


        public class SubmitOrderConsumerDefinition :
            ConsumerDefinition<SubmitOrderConsumer>
        {
            public SubmitOrderConsumerDefinition()
            {
            }
        }
    }
}
