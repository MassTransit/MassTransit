namespace Automatonymous
{
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public interface SagaStateMachine<TInstance> :
        StateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        /// <summary>
        /// Returns the event correlations for the state machine
        /// </summary>
        IEnumerable<EventCorrelation> Correlations { get; }

        /// <summary>
        /// Returns true if the saga state machine instance is complete and can be removed from the repository
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        Task<bool> IsCompleted(TInstance instance);
    }
}
