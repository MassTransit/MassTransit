namespace MassTransit.Consumer
{
    using System;
    using System.Threading.Tasks;


    public class ObjectConsumerFactory<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly IConsumerFactory<TConsumer> _delegate;

        public ObjectConsumerFactory(Func<Type, object> objectFactory)
        {
            _delegate = new DelegateConsumerFactory<TConsumer>(() => (TConsumer)objectFactory(typeof(TConsumer)));
        }

        public Task Send<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            where TMessage : class
        {
            return _delegate.Send(context, next);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateConsumerFactoryScope<TConsumer>("objectFactory");
        }
    }
}
