namespace Automatonymous.CorrelationConfigurators
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;


    public class EventMissingInstanceConfigurator<TInstance, TData> :
        IMissingInstanceConfigurator<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
    {
        public IPipe<ConsumeContext<TData>> Discard()
        {
            return Pipe.Empty<ConsumeContext<TData>>();
        }

        public IPipe<ConsumeContext<TData>> Fault()
        {
            return Pipe.Execute<ConsumeContext<TData>>(context =>
            {
                throw new SagaException("An existing saga instance was not found", typeof(TInstance), typeof(TData), context.CorrelationId ?? Guid.Empty);
            });
        }

        public IPipe<ConsumeContext<TData>> ExecuteAsync(Func<ConsumeContext<TData>, Task> action)
        {
            return Pipe.ExecuteAsync(action);
        }

        public IPipe<ConsumeContext<TData>> Execute(Action<ConsumeContext<TData>> action)
        {
            return Pipe.Execute(action);
        }
    }
}
