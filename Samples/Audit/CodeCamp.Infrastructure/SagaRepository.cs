namespace CodeCamp.Infrastructure
{
    using System;
    using Magnum.Common.Repository;
    using MassTransit.Saga;
    using MassTransit.ServiceBus;

    public class SagaRepository<T> :
        ISagaRepository<T>
        where T : class, CorrelatedBy<Guid>
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public SagaRepository(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public T Create(Guid correlationId)
        {
           T saga = (T) Activator.CreateInstance(typeof (T), correlationId);

            WithRepository(r => r.Save(saga));

            return saga;
        }

        public T Get(Guid id)
        {
            using (IRepository repository = _repositoryFactory.GetRepository())
            {
                return repository.Get<T>(id);
            }
        }

        public void Save(T item)
        {
            WithRepository(r => r.Update(item));
        }

        public void Dispose()
        {
        }

        private void WithRepository(Action<IRepository> action)
        {
            using (IRepository repository = _repositoryFactory.GetRepository())
            {
                action(repository);
            }
        }
    }
}