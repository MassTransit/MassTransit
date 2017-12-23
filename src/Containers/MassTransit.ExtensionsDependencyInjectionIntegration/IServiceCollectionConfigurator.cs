namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using Saga;


    public interface IServiceCollectionConfigurator
    {
        void AddConsumer<T>()
            where T : class, IConsumer;

        void AddSaga<T>()
            where T : class, ISaga;
    }
}