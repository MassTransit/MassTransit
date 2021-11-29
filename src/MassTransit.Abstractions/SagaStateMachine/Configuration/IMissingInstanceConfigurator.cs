namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    public interface IMissingInstanceConfigurator<TSaga, TMessage>
        where TSaga : SagaStateMachineInstance
        where TMessage : class
    {
        /// <summary>
        /// Discard the event, silently ignoring the missing instance for the event
        /// </summary>
        IPipe<ConsumeContext<TMessage>> Discard();

        /// <summary>
        /// Fault the saga consumer, which moves the message to the error queue
        /// </summary>
        IPipe<ConsumeContext<TMessage>> Fault();

        /// <summary>
        /// Execute an asynchronous method when the instance is missed, allowing a custom behavior to be specified.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        IPipe<ConsumeContext<TMessage>> ExecuteAsync(Func<ConsumeContext<TMessage>, Task> callback);

        /// <summary>
        /// Execute a method when the instance is missed, allowing a custom behavior to be specified.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        IPipe<ConsumeContext<TMessage>> Execute(Action<ConsumeContext<TMessage>> callback);
    }
}
