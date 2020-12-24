namespace MassTransit.StructureMapIntegration
{
    using Automatonymous;
    using GreenPipes;
    using StructureMap;


    public class StructureMapStateMachineActivityFactory :
        IStateMachineActivityFactory
    {
        public static readonly IStateMachineActivityFactory Instance = new StructureMapStateMachineActivityFactory();

        public T GetService<T>(PipeContext context)
            where T : class
        {
            var container = context.GetPayload<IContainer>();

            return container.GetInstance<T>();
        }
    }
}
