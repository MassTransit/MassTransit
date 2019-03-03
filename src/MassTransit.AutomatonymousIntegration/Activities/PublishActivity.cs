namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;


    public class PublishActivity<TInstance, TMessage> :
        Activity<TInstance>
        where TInstance : SagaStateMachineInstance
        where TMessage : class
    {
        readonly AsyncEventMessageFactory<TInstance, TMessage> _asyncMessageFactory;
        readonly EventMessageFactory<TInstance, TMessage> _messageFactory;
        readonly IPipe<PublishContext<TMessage>> _publishPipe;

        public PublishActivity(EventMessageFactory<TInstance, TMessage> messageFactory, Action<PublishContext<TMessage>> contextCallback)
            : this(contextCallback)
        {
            _messageFactory = messageFactory;
        }

        public PublishActivity(AsyncEventMessageFactory<TInstance, TMessage> messageFactory, Action<PublishContext<TMessage>> contextCallback)
            : this(contextCallback)
        {
            _asyncMessageFactory = messageFactory;
        }

        PublishActivity(Action<PublishContext<TMessage>> contextCallback)
        {
            _publishPipe = contextCallback != null ? Pipe.Execute(contextCallback) : Pipe.Empty<PublishContext<TMessage>>();
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("publish");
            _publishPipe.Probe(scope);
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

            var message = _messageFactory?.Invoke(consumeContext) ?? await _asyncMessageFactory(consumeContext).ConfigureAwait(false);

            await consumeContext.Publish(message, _publishPipe).ConfigureAwait(false);
        }
    }


    public class PublishActivity<TInstance, TData, TMessage> :
        Activity<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
        where TMessage : class
    {
        readonly AsyncEventMessageFactory<TInstance, TData, TMessage> _asyncMessageFactory;
        readonly EventMessageFactory<TInstance, TData, TMessage> _messageFactory;
        readonly IPipe<PublishContext<TMessage>> _publishPipe;

        public PublishActivity(EventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<PublishContext<TMessage>> contextCallback)
            : this(contextCallback)
        {
            _messageFactory = messageFactory;
        }

        public PublishActivity(AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<PublishContext<TMessage>> contextCallback)
            : this(contextCallback)
        {
            _asyncMessageFactory = messageFactory;
        }

        PublishActivity(Action<PublishContext<TMessage>> contextCallback)
        {
            _publishPipe = contextCallback != null ? Pipe.Execute(contextCallback) : Pipe.Empty<PublishContext<TMessage>>();
        }

        void Visitable.Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("publish");
            _publishPipe.Probe(scope);
        }

        async Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            ConsumeEventContext<TInstance, TData> consumeContext = context.CreateConsumeContext();

            var message = _messageFactory?.Invoke(consumeContext) ?? await _asyncMessageFactory(consumeContext).ConfigureAwait(false);

            await consumeContext.Publish(message, _publishPipe).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        Task Activity<TInstance, TData>.Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context,
            Behavior<TInstance, TData> next)
        {
            return next.Faulted(context);
        }
    }
}
