using System;
using System.Threading.Tasks;
using Automatonymous;
using GreenPipes;

namespace MassTransit.EventStoreDbIntegration.Activities
{
    public class FaultedProduceActivity<TInstance, TException, TMessage> :
        Activity<TInstance>
        where TInstance : SagaStateMachineInstance
        where TMessage : class
        where TException : Exception
    {
        readonly AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> _asyncMessageFactory;
        readonly ExceptionEventStoreDbStreamNameProvider<TInstance, TException> _nameProvider;
        readonly IPipe<EventStoreDbSendContext<TMessage>> _pipe;

        public FaultedProduceActivity(ExceptionEventStoreDbStreamNameProvider<TInstance, TException> nameProvider,
            AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            Action<EventStoreDbSendContext<TMessage>> contextCallback)
        {
            _nameProvider = nameProvider;
            _asyncMessageFactory = messageFactory;
            _pipe = contextCallback != null ? Pipe.Execute(contextCallback) : Pipe.Empty<EventStoreDbSendContext<TMessage>>();
        }

        void Visitable.Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            return next.Execute(context);
        }

        Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            return next.Execute(context);
        }

        async Task Activity<TInstance>.Faulted<T>(BehaviorExceptionContext<TInstance, T> context, Behavior<TInstance> next)
        {
            await Faulted(context).ConfigureAwait(false);

            await next.Faulted(context).ConfigureAwait(false);
        }

        async Task Activity<TInstance>.Faulted<T, TOtherException>(BehaviorExceptionContext<TInstance, T, TOtherException> context, Behavior<TInstance, T> next)
        {
            await Faulted(context).ConfigureAwait(false);

            await next.Faulted(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("publish-faulted");

            _pipe.Probe(scope);
        }

        async Task Faulted(BehaviorContext<TInstance> context)
        {
            if (context.TryGetExceptionContext(out ConsumeExceptionEventContext<TInstance, TException> exceptionContext))
            {
                var message = await _asyncMessageFactory(exceptionContext).ConfigureAwait(false);

                var producer = await context.GetProducer(exceptionContext, _nameProvider(exceptionContext));

                await producer.Produce(message, _pipe).ConfigureAwait(false);
            }
        }
    }


    public class FaultedProduceActivity<TInstance, TData, TException, TMessage> :
        Activity<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
        where TMessage : class
        where TException : Exception
    {
        readonly AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> _asyncMessageFactory;
        readonly ExceptionEventStoreDbStreamNameProvider<TInstance, TData, TException> _nameProvider;
        readonly IPipe<EventStoreDbSendContext<TMessage>> _pipe;

        public FaultedProduceActivity(ExceptionEventStoreDbStreamNameProvider<TInstance, TData, TException> nameProvider,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, Action<EventStoreDbSendContext<TMessage>> contextCallback)
        {
            _asyncMessageFactory = messageFactory;
            _nameProvider = nameProvider;
            _pipe = contextCallback != null ? Pipe.Execute(contextCallback) : Pipe.Empty<EventStoreDbSendContext<TMessage>>();
        }

        void Visitable.Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            return next.Execute(context);
        }

        async Task Activity<TInstance, TData>.Faulted<T>(BehaviorExceptionContext<TInstance, TData, T> context,
            Behavior<TInstance, TData> next)
        {
            if (context.TryGetExceptionContext(out ConsumeExceptionEventContext<TInstance, TData, TException> exceptionContext))
            {
                var message = await _asyncMessageFactory(exceptionContext).ConfigureAwait(false);

                var producer = await context.GetProducer(exceptionContext, _nameProvider(exceptionContext));

                await producer.Produce(message, _pipe).ConfigureAwait(false);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("publish-faulted");
            _pipe.Probe(scope);
        }
    }
}
