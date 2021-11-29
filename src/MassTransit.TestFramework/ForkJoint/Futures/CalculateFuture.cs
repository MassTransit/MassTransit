namespace MassTransit.TestFramework.ForkJoint.Futures
{
    using System.Linq;
    using Contracts;


    public class CalculateFuture :
        Future<OrderCalculate, CalculateCompleted>
    {
        public CalculateFuture()
        {
            ConfigureCommand(x => x.CorrelateById(context => context.Message.OrderLineId));

            FutureResponseHandle<OrderCalculate, CalculateCompleted, Fault<OrderCalculate>, OrderFry, FryCompleted> fry = SendRequest<OrderFry>(x =>
                {
                    x.UsingRequestInitializer(context => new
                    {
                        OrderId = context.Saga.CorrelationId,
                        OrderLineId = InVar.Id,
                        Size = Size.Medium
                    });

                    x.TrackPendingRequest(message => message.OrderLineId);
                })
                .OnResponseReceived<FryCompleted>(x =>
                {
                    x.CompletePendingRequest(message => message.OrderLineId);

                    x.WhenReceived(binder => binder.SetVariable("Taste", _ => "Delicious"));
                });


            WhenAllCompleted(x =>
            {
                x.SetCompletedUsingInitializer(context =>
                {
                    var fryCompleted = context.SelectResults<FryCompleted>().FirstOrDefault();

                    context.TryGetVariable("Taste", out string taste);

                    return new { Description = $"Calculated ({fryCompleted.Description}, {taste})" };
                });
            });
        }
    }
}
