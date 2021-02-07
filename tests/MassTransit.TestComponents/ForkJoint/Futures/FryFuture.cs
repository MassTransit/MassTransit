namespace MassTransit.TestComponents.ForkJoint.Futures
{
    using Contracts;
    using MassTransit.Futures;


    public class FryFuture :
        Future<OrderFry, FryCompleted>
    {
        public FryFuture()
        {
            ConfigureCommand(x => x.CorrelateById(context => context.Message.OrderLineId));

            SendRequest<CookFry>(x =>
                {
                    x.UsingRequestFactory(context => new CookFryRequest(context.Message.OrderId, context.Message.OrderLineId, context.Message.Size));
                })
                .OnResponseReceived<FryReady>(x =>
                {
                    x.SetCompletedUsingFactory(context => new FryCompletedResult(context.Instance.Created,
                        context.Instance.Completed ?? default,
                        context.Message.OrderId,
                        context.Message.OrderLineId,
                        context.Message.Size,
                        $"{context.Message.Size} Fries"));
                });
        }
    }
}
