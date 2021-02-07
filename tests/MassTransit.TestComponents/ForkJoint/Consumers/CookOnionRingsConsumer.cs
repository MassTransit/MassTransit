namespace MassTransit.TestComponents.ForkJoint.Consumers
{
    using System.Threading.Tasks;
    using Contracts;
    using MassTransit;
    using Services;


    public class CookOnionRingsConsumer :
        IConsumer<CookOnionRings>
    {
        readonly IFryer _fryer;

        public CookOnionRingsConsumer(IFryer fryer)
        {
            _fryer = fryer;
        }

        public async Task Consume(ConsumeContext<CookOnionRings> context)
        {
            await _fryer.CookOnionRings(context.Message.Quantity);

            await context.RespondAsync<OnionRingsReady>(new
            {
                context.Message.OrderId,
                context.Message.OrderLineId,
                context.Message.Quantity
            });
        }
    }
}
