namespace MassTransit.Consumer
{
    using System.Threading.Tasks;
    using Context;


    /// <summary>
    /// Retains a reference to an existing message consumer, and uses it to send consumable messages for
    /// processing.
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type</typeparam>
    public class InstanceConsumerFactory<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly TConsumer _consumer;

        public InstanceConsumerFactory(TConsumer consumer)
        {
            _consumer = consumer;
        }

        public Task Send<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            where TMessage : class
        {
            return next.Send(new ConsumerConsumeContextScope<TConsumer, TMessage>(context, _consumer));
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateConsumerFactoryScope<TConsumer>("instance");
        }
    }
}
