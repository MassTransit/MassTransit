namespace MassTransit
{
    using System.Collections.Generic;
    using SagaStateMachine;


    public interface EventActivities<TInstance>
        where TInstance : class, ISaga
    {
        IEnumerable<IActivityBinder<TInstance>> GetStateActivityBinders();
    }
}
