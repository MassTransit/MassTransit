namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;


    public class FaultedSendActivity<TInstance, TException, TMessage> :
        Activity<TInstance>
        where TInstance : SagaStateMachineInstance
        where TMessage : class
        where TException : Exception
    {
        readonly AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> _asyncMessageFactory;
        readonly DestinationAddressProvider<TInstance> _destinationAddressProvider;
        readonly EventExceptionMessageFactory<TInstance, TException, TMessage> _messageFactory;
        readonly IPipe<SendContext<TMessage>> _sendPipe;

        public FaultedSendActivity(DestinationAddressProvider<TInstance> destinationAddressProvider,
            EventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback)
            : this(messageFactory, contextCallback)
        {
            _destinationAddressProvider = destinationAddressProvider;
        }

        public FaultedSendActivity(DestinationAddressProvider<TInstance> destinationAddressProvider,
            AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback)
            : this(messageFactory, contextCallback)
        {
            _destinationAddressProvider = destinationAddressProvider;
        }

        public FaultedSendActivity(EventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback)
            : this(contextCallback)
        {
            _messageFactory = messageFactory;
        }

        public FaultedSendActivity(AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback)
            : this(contextCallback)
        {
            _asyncMessageFactory = messageFactory;
        }

        FaultedSendActivity(Action<SendContext<TMessage>> contextCallback)
        {
            _sendPipe = contextCallback != null ? Pipe.Execute(contextCallback) : Pipe.Empty<SendContext<TMessage>>();
        }

        void Visitable.Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("send-faulted");
            _sendPipe.Probe(scope);
        }

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            return next.Execute(context);
        }

        Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            return next.Execute(context);
        }

        async Task Activity<TInstance>.Faulted<T>(BehaviorExceptionContext<TInstance, T> context,
            Behavior<TInstance> next)
        {
            if (context.TryGetExceptionContext(out ConsumeExceptionEventContext<TInstance, TException> exceptionContext))
            {
                var message = _messageFactory?.Invoke(exceptionContext) ?? await _asyncMessageFactory(exceptionContext).ConfigureAwait(false);

                if (_destinationAddressProvider != null)
                {
                    var destinationAddress = _destinationAddressProvider(exceptionContext);

                    var endpoint = await exceptionContext.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

                    await endpoint.Send(message, _sendPipe).ConfigureAwait(false);
                }
                else
                    await exceptionContext.Send(message, _sendPipe).ConfigureAwait(false);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }

        async Task Activity<TInstance>.Faulted<T, TOtherException>(BehaviorExceptionContext<TInstance, T, TOtherException> context, Behavior<TInstance, T> next)
        {
            if (context.TryGetExceptionContext(out ConsumeExceptionEventContext<TInstance, TException> exceptionContext))
            {
                var message = _messageFactory?.Invoke(exceptionContext) ?? await _asyncMessageFactory(exceptionContext).ConfigureAwait(false);

                if (_destinationAddressProvider != null)
                {
                    var destinationAddress = _destinationAddressProvider(exceptionContext);

                    var endpoint = await exceptionContext.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

                    await endpoint.Send(message, _sendPipe).ConfigureAwait(false);
                }
                else
                    await exceptionContext.Send(message, _sendPipe).ConfigureAwait(false);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }
    }


    public class FaultedSendActivity<TInstance, TData, TException, TMessage> :
        Activity<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
        where TMessage : class
        where TException : Exception
    {
        readonly AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> _asyncMessageFactory;
        readonly DestinationAddressProvider<TInstance, TData> _destinationAddressProvider;
        readonly EventExceptionMessageFactory<TInstance, TData, TException, TMessage> _messageFactory;
        readonly IPipe<SendContext<TMessage>> _sendPipe;

        public FaultedSendActivity(DestinationAddressProvider<TInstance, TData> destinationAddressProvider,
            EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback)
            : this(messageFactory, contextCallback)
        {
            _destinationAddressProvider = destinationAddressProvider;
        }

        public FaultedSendActivity(DestinationAddressProvider<TInstance, TData> destinationAddressProvider,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback)
            : this(messageFactory, contextCallback)
        {
            _destinationAddressProvider = destinationAddressProvider;
        }

        public FaultedSendActivity(EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback)
            : this(contextCallback)
        {
            _messageFactory = messageFactory;
        }

        public FaultedSendActivity(AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback)
            : this(contextCallback)
        {
            _asyncMessageFactory = messageFactory;
        }

        FaultedSendActivity(Action<SendContext<TMessage>> contextCallback)
        {
            _sendPipe = contextCallback != null ? Pipe.Execute(contextCallback) : Pipe.Empty<SendContext<TMessage>>();
        }

        void Visitable.Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("send-faulted");
            _sendPipe.Probe(scope);
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
                var message = _messageFactory?.Invoke(exceptionContext) ?? await _asyncMessageFactory(exceptionContext).ConfigureAwait(false);

                if (_destinationAddressProvider != null)
                {
                    var destinationAddress = _destinationAddressProvider(exceptionContext);

                    var endpoint = await exceptionContext.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

                    await endpoint.Send(message, _sendPipe).ConfigureAwait(false);
                }
                else
                    await exceptionContext.Send(message, _sendPipe).ConfigureAwait(false);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }
    }
}
