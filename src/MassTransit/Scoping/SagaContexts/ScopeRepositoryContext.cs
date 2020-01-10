namespace MassTransit.Scoping.SagaContexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Util;
    using Saga;


    public class ScopeRepositoryContext<TSaga, T> :
        SagaRepositoryContext<TSaga, T>,
        IAsyncDisposable
        where TSaga : class, ISaga
        where T : class
    {
        readonly SagaRepositoryContext<TSaga, T> _repositoryContext;
        readonly IDisposable _scope;

        public ScopeRepositoryContext(SagaRepositoryContext<TSaga, T> repositoryContext, IDisposable scope)
        {
            _repositoryContext = repositoryContext;
            _scope = scope;
        }

        Task<SagaConsumeContext<TSaga, T>> SagaRepositoryContext<TSaga, T>.Add(TSaga instance)
        {
            return _repositoryContext.Add(instance);
        }

        Task<SagaConsumeContext<TSaga, T>> SagaRepositoryContext<TSaga, T>.Insert(TSaga instance)
        {
            return _repositoryContext.Insert(instance);
        }

        Task<SagaConsumeContext<TSaga, T>> SagaRepositoryContext<TSaga, T>.Load(Guid correlationId)
        {
            return _repositoryContext.Load(correlationId);
        }

        Task<SagaRepositoryQueryContext<TSaga, T>> SagaRepositoryContext<TSaga, T>.Query(ISagaQuery<TSaga> query)
        {
            return _repositoryContext.Query(query);
        }

        Task SagaRepositoryContext<TSaga, T>.Faulted(Exception exception)
        {
            return _repositoryContext.Faulted(exception);
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            _scope?.Dispose();

            switch (_repositoryContext)
            {
                case IAsyncDisposable asyncDisposable:
                    return asyncDisposable.DisposeAsync(cancellationToken);

                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }

            return TaskUtil.Completed;
        }
    }


    public class ScopeRepositoryContext<TSaga> :
        SagaRepositoryContext<TSaga>,
        IAsyncDisposable
        where TSaga : class, ISaga
    {
        readonly SagaRepositoryContext<TSaga> _repositoryContext;
        readonly IDisposable _scope;

        public ScopeRepositoryContext(SagaRepositoryContext<TSaga> repositoryContext, IDisposable scope)
        {
            _repositoryContext = repositoryContext;
            _scope = scope;
        }

        Task<SagaRepositoryQueryContext<TSaga>> SagaRepositoryContext<TSaga>.Query(ISagaQuery<TSaga> query)
        {
            return _repositoryContext.Query(query);
        }

        Task<TSaga> SagaRepositoryContext<TSaga>.Load(Guid correlationId)
        {
            return _repositoryContext.Load(correlationId);
        }

        Task SagaRepositoryContext<TSaga>.Faulted(Exception exception)
        {
            return _repositoryContext.Faulted(exception);
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            _scope.Dispose();

            switch (_repositoryContext)
            {
                case IAsyncDisposable asyncDisposable:
                    return asyncDisposable.DisposeAsync(cancellationToken);

                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }

            return TaskUtil.Completed;
        }
    }
}
