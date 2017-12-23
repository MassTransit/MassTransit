namespace MassTransit.Scoping
{
    using Saga;


    public interface ISagaRepositoryFactory
    {
        ISagaRepository<T> CreateSagaRepository<T>()
            where T : class, ISaga;
    }
}