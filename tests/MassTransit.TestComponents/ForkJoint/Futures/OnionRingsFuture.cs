namespace MassTransit.TestComponents.ForkJoint.Futures
{
    using Contracts;
    using MassTransit.Futures;


    public class OnionRingsFuture :
        Future<OrderOnionRings, OnionRingsCompleted>
    {
        public OnionRingsFuture()
        {
            ConfigureCommand(x => x.CorrelateById(context => context.Message.OrderLineId));

            SendRequest<CookOnionRings>()
                .OnResponseReceived<OnionRingsReady>(x =>
                {
                    x.SetCompletedUsingInitializer(context => new {Description = $"{context.Message.Quantity} Onion Rings"});
                });
        }
    }
}
