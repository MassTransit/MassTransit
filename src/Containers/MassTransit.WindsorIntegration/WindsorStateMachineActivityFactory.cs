namespace MassTransit.WindsorIntegration
{
    using Automatonymous;
    using Castle.MicroKernel;
    using GreenPipes;


    public class WindsorStateMachineActivityFactory :
        IStateMachineActivityFactory
    {
        public static readonly IStateMachineActivityFactory Instance = new WindsorStateMachineActivityFactory();

        public T GetService<T>(PipeContext context)
            where T : class
        {
            var kernel = context.GetPayload<IKernel>();

            return kernel.Resolve<T>();
        }
    }
}
