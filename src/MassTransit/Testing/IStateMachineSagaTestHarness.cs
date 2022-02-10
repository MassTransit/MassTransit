namespace MassTransit.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;


    public interface ISagaStateMachineTestHarness<out TStateMachine, TInstance> :
        ISagaTestHarness<TInstance>
        where TStateMachine : SagaStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        TStateMachine StateMachine { get; }

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

        /// <summary>
        /// Waits until a saga exists with the specified correlationId in the specified state
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="stateSelector"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<IList<Guid>> Exists(Expression<Func<TInstance, bool>> expression, Func<TStateMachine, State> stateSelector, TimeSpan? timeout = default);

        /// <summary>
        /// Waits until a saga exists with the specified correlationId in the specified state
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="state">The expected state</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<IList<Guid>> Exists(Expression<Func<TInstance, bool>> expression, State state, TimeSpan? timeout = default);
    }


    [Obsolete("Use ISagaStateMachineTestHarness<TStateMachine, TInstance> instead")]
    public interface IStateMachineSagaTestHarness<TInstance, out TStateMachine> :
        ISagaStateMachineTestHarness<TStateMachine, TInstance>
        where TStateMachine : SagaStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
    }
}
