namespace MassTransit.Configuration
{
    using System;
    using System.Threading.Tasks;


    public class EventMissingInstanceConfigurator<TSaga, TMessage> :
        IMissingInstanceConfigurator<TSaga, TMessage>
        where TSaga : SagaStateMachineInstance
        where TMessage : class
    {
        public IPipe<ConsumeContext<TMessage>> Discard()
        {
            return Pipe.Empty<ConsumeContext<TMessage>>();
        }

        public IPipe<ConsumeContext<TMessage>> Fault()
        {
            return Execute(context =>
                throw new SagaException("An existing saga instance was not found", typeof(TSaga), typeof(TMessage), context.CorrelationId ?? Guid.Empty));
        }

        public IPipe<ConsumeContext<TMessage>> ExecuteAsync(Func<ConsumeContext<TMessage>, Task> callback)
        {
            return callback.ToPipe();
        }

        public IPipe<ConsumeContext<TMessage>> Execute(Action<ConsumeContext<TMessage>> callback)
        {
            return Pipe.Execute(callback);
        }
    }
}
