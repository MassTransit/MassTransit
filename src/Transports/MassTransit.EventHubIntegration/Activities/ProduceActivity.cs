namespace MassTransit.EventHubIntegration.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;


    public class ProduceActivity<TInstance, TMessage> :
        Activity<TInstance>
        where TInstance : SagaStateMachineInstance
        where TMessage : class
    {
        readonly AsyncEventMessageFactory<TInstance, TMessage> _asyncMessageFactory;
        readonly EventHubNameProvider<TInstance> _nameProvider;
        readonly IPipe<EventHubSendContext<TMessage>> _pipe;

        public ProduceActivity(EventHubNameProvider<TInstance> nameProvider,
            AsyncEventMessageFactory<TInstance, TMessage> messageFactory, Action<EventHubSendContext<TMessage>> contextCallback)
        {
            _nameProvider = nameProvider;
            _asyncMessageFactory = messageFactory;

            _pipe = contextCallback != null ? Pipe.Execute(contextCallback) : Pipe.Empty<EventHubSendContext<TMessage>>();
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("publish");
            _pipe.Probe(scope);
        }

        async Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        async Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            return next.Faulted(context);
        }

        Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, Behavior<TInstance, T> next)
        {
            return next.Faulted(context);
        }

        async Task Execute(BehaviorContext<TInstance> context)
        {
            ConsumeEventContext<TInstance> consumeContext = context.CreateConsumeContext();

            var message = await _asyncMessageFactory(consumeContext).ConfigureAwait(false);

            var producer = await context.GetProducer(consumeContext, _nameProvider(consumeContext));

            await producer.Produce(message, _pipe).ConfigureAwait(false);
        }
    }


    public class ProduceActivity<TInstance, TData, TMessage> :
        Activity<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
        where TMessage : class
    {
        readonly AsyncEventMessageFactory<TInstance, TData, TMessage> _asyncMessageFactory;
        readonly EventHubNameProvider<TInstance, TData> _nameProvider;
        readonly IPipe<EventHubSendContext<TMessage>> _pipe;

        public ProduceActivity(EventHubNameProvider<TInstance, TData> nameProvider,
            AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<EventHubSendContext<TMessage>> contextCallback)
        {
            _nameProvider = nameProvider;
            _asyncMessageFactory = messageFactory;
            _pipe = contextCallback != null ? Pipe.Execute(contextCallback) : Pipe.Empty<EventHubSendContext<TMessage>>();
        }

        void Visitable.Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("publish");
            _pipe.Probe(scope);
        }

        async Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            ConsumeEventContext<TInstance, TData> consumeContext = context.CreateConsumeContext();

            var message = await _asyncMessageFactory(consumeContext).ConfigureAwait(false);

            var producer = await context.GetProducer(consumeContext, _nameProvider(consumeContext));

            await producer.Produce(message, _pipe).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        Task Activity<TInstance, TData>.Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context,
            Behavior<TInstance, TData> next)
        {
            return next.Faulted(context);
        }
    }
}
