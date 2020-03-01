using System;
using System.Threading.Tasks;

namespace Sample.Analyzers
{
    using MassTransit;


    public interface OrderRequest
    {
        Guid CorrelationId { get; }
        string OrderId { get; }
    }

    public interface OrderResponse
    {
        string OrderId { get; }
        string OrderStatus { get; }
    }

    public class TestConsumeContext0
    {
        public async Task Consume(ConsumeContext<OrderRequest> context)
        {
            await context.RespondAsync<OrderResponse>(new
            {
            });

            await context.Publish<OrderResponse>(new
            {
            });

            var endpoint = await context.GetSendEndpoint(new Uri("queue:some-queue"));

            await endpoint.Send<OrderResponse>(new
            {
            });

            var requestClient = context.CreateRequestClient<OrderRequest>(null);

            var request = requestClient.Create(new
            {
            });
        }
    }
}