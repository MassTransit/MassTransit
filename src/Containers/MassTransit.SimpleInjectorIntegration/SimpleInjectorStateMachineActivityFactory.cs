namespace MassTransit.SimpleInjectorIntegration
{
    using Automatonymous;
    using GreenPipes;
    using SimpleInjector;


    public class SimpleInjectorStateMachineActivityFactory :
        IStateMachineActivityFactory
    {
        public static readonly IStateMachineActivityFactory Instance = new SimpleInjectorStateMachineActivityFactory();

        public T GetService<T>(PipeContext context)
            where T : class
        {
            var container = context.GetPayload<Scope>();

            return container.GetInstance<T>();
        }
    }
}
