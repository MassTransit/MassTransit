namespace MassTransit.NinjectIntegration
{
    using Saga;


    public interface IKernelConfigurator
    {
        void AddConsumer<T>()
            where T : class, IConsumer;

        void AddSaga<T>()
            where T : class, ISaga;
    }
}
