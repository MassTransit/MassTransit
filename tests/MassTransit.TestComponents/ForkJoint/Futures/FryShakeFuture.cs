namespace MassTransit.TestComponents.ForkJoint.Futures
{
    using Contracts;
    using MassTransit.Futures;


    public class FryShakeFuture :
        Future<OrderFryShake, FryShakeCompleted>
    {
        public FryShakeFuture()
        {
            ConfigureCommand(x => x.CorrelateById(context => context.Message.OrderLineId));

            SendRequest<OrderFry>(x =>
                {
                    x.UsingRequestInitializer(context => new
                    {
                        OrderId = context.Instance.CorrelationId,
                        OrderLineId = InVar.Id,
                        context.Message.Size,
                    });

                    x.TrackPendingRequest(message => message.OrderLineId);
                })
                .OnResponseReceived<FryCompleted>(x =>
                {
                    x.CompletePendingRequest(message => message.OrderLineId);
                });

            SendRequest<OrderShake>(x =>
                {
                    x.UsingRequestInitializer(context => new
                    {
                        OrderId = context.Instance.CorrelationId,
                        OrderLineId = InVar.Id,
                        context.Message.Flavor,
                        context.Message.Size,
                    });

                    x.TrackPendingRequest(message => message.OrderLineId);
                })
                .OnResponseReceived<ShakeCompleted>(x =>
                {
                    x.CompletePendingRequest(message => message.OrderLineId);
                });


            WhenAllCompleted(x => x.SetCompletedUsingInitializer(context =>
            {
                var message = context.Instance.GetCommand<OrderFryShake>();

                return new {Description = $"{message.Size} {message.Flavor} FryShake({context.Instance.Results.Count})"};
            }));
        }
    }
}
