namespace MassTransit.TestFramework.ForkJoint.Futures
{
    using Contracts;


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
                    x.SetCompletedUsingFactory(context => new FryCompletedResult(context.Saga.Created,
                        context.Saga.Completed ?? default,
                        context.Message.OrderId,
                        context.Message.OrderLineId,
                        context.Message.Size,
                        $"{context.Message.Size} Fries"));
                });
        }
    }
}
