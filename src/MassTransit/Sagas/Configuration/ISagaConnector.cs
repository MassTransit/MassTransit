namespace MassTransit.Configuration
{
    public interface ISagaConnector
    {
        ISagaSpecification<T> CreateSagaSpecification<T>()
            where T : class, ISaga;

        ConnectHandle ConnectSaga<T>(IConsumePipeConnector consumePipe, ISagaRepository<T> repository, ISagaSpecification<T> specification)
            where T : class, ISaga;
    }
}
