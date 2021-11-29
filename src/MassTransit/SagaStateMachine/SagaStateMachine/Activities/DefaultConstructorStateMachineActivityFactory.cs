namespace MassTransit.SagaStateMachine
{
    using System;


    public class DefaultConstructorStateMachineActivityFactory :
        IStateMachineActivityFactory
    {
        public T GetService<T>(PipeContext context)
            where T : class
        {
            return Activator.CreateInstance<T>();
        }
    }
}
