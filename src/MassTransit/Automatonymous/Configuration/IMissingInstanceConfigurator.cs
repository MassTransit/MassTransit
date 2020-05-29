namespace Automatonymous
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;


    public interface IMissingInstanceConfigurator<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
    {
        /// <summary>
        /// Discard the event, silently ignoring the missing instance for the event
        /// </summary>
        IPipe<ConsumeContext<TData>> Discard();

        /// <summary>
        /// Fault the saga consumer, which moves the message to the error queue
        /// </summary>
        IPipe<ConsumeContext<TData>> Fault();

        /// <summary>
        /// Execute an asynchronous method when the instance is missed, allowing a custom behavior to be specified.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IPipe<ConsumeContext<TData>> ExecuteAsync(Func<ConsumeContext<TData>, Task> action);

        /// <summary>
        /// Execute a method when the instance is missed, allowing a custom behavior to be specified.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IPipe<ConsumeContext<TData>> Execute(Action<ConsumeContext<TData>> action);
    }
}
