namespace MassTransit.Futures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using SagaStateMachine;


    public class FutureFault<TCommand, TFault, TInput> :
        ISpecification
        where TCommand : class
        where TFault : class
        where TInput : class
    {
        static readonly object _defaultValues = new Default();
        ContextMessageFactory<BehaviorContext<FutureState, TInput>, TFault> _factory;

        public FutureFault()
        {
            _factory = new ContextMessageFactory<BehaviorContext<FutureState, TInput>, TFault>(DefaultFactory);
        }

        public ContextMessageFactory<BehaviorContext<FutureState, TInput>, TFault> Factory
        {
            set => _factory = value;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public async Task SetFaulted(BehaviorContext<FutureState, TInput> context)
        {
            context.SetFaulted(context.Saga.CorrelationId);

            var fault = await context.SendMessageToSubscriptions(_factory,
                context.Saga.HasSubscriptions() ? context.Saga.Subscriptions.ToArray() : Array.Empty<FutureSubscription>());

            context.SetFault(context.Saga.CorrelationId, fault);
        }

        static Task<SendTuple<TFault>> DefaultFactory(BehaviorContext<FutureState, TInput> context)
        {
            if (context.Message is Fault fault)
            {
                var request = context.GetCommand<TCommand>();

                return context.Init<TFault>(new
                {
                    fault.FaultId,
                    fault.FaultedMessageId,
                    fault.Timestamp,
                    fault.Exceptions,
                    fault.Host,
                    fault.FaultMessageTypes,
                    Message = request
                });
            }

            return context.Init<TFault>(_defaultValues);
        }


        class Default
        {
        }
    }


    public class FutureFault<TFault> :
        ISpecification
        where TFault : class
    {
        static readonly object _defaultValues = new Default();
        ContextMessageFactory<BehaviorContext<FutureState>, TFault> _factory;

        public FutureFault()
        {
            _factory = MessageFactory<TFault>.Create((Func<BehaviorContext<FutureState>, Task<SendTuple<TFault>>>)DefaultFactory);
        }

        public ContextMessageFactory<BehaviorContext<FutureState>, TFault> Factory
        {
            set => _factory = value;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public async Task SetFaulted(BehaviorContext<FutureState> context)
        {
            context.SetFaulted(context.Saga.CorrelationId);

            var fault = await context.SendMessageToSubscriptions(_factory,
                context.Saga.HasSubscriptions() ? context.Saga.Subscriptions.ToArray() : Array.Empty<FutureSubscription>());

            context.SetFault(context.Saga.CorrelationId, fault);
        }

        static Task<SendTuple<TFault>> DefaultFactory(BehaviorContext<FutureState> context)
        {
            return context.Init<TFault>(_defaultValues);
        }


        class Default
        {
        }
    }
}
