namespace MassTransit.Testing
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;


    public interface IStateMachineSagaTestHarness<TInstance, out TStateMachine> :
        ISagaTestHarness<TInstance>
        where TInstance : class, SagaStateMachineInstance
        where TStateMachine : SagaStateMachine<TInstance>
    {
        /// <summary>
        /// Waits until a saga exists with the specified correlationId in the specified state
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="stateSelector"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<Guid?> Exists(Guid correlationId, Func<TStateMachine, State> stateSelector, TimeSpan? timeout = default);

        /// <summary>
        /// Waits until a saga exists with the specified correlationId in the specified state
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="state">The expected state</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<Guid?> Exists(Guid correlationId, State state, TimeSpan? timeout = default);
    }
}
