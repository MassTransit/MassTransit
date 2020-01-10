namespace MassTransit.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// The modern saga repository, which can be used with any storage engine. Leverages the new interfaces for consume and query context.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class SagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>,
        ILoadSagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISagaRepositoryContextFactory<TSaga> _repositoryContextFactory;

        public SagaRepository(ISagaRepositoryContextFactory<TSaga> repositoryContextFactory)
        {
            _repositoryContextFactory = repositoryContextFactory;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("sagaRepository");

            _repositoryContextFactory.Probe(scope);
        }

        public async Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The CorrelationId was not specified", typeof(TSaga), typeof(T));

            var correlationId = context.CorrelationId.Value;

            SagaRepositoryContext<TSaga, T> repositoryContext = await _repositoryContextFactory.CreateContext(context, correlationId).ConfigureAwait(false);
            try
            {
                SagaConsumeContext<TSaga, T> sagaConsumeContext = null;

                if (policy.PreInsertInstance(context, out var instance))
                    sagaConsumeContext = await repositoryContext.Insert(instance).ConfigureAwait(false);

                if (sagaConsumeContext == null)
                    sagaConsumeContext = await repositoryContext.Load(correlationId).ConfigureAwait(false);

                if (sagaConsumeContext != null)
                {
                    try
                    {
                        sagaConsumeContext.LogUsed();

                        await policy.Existing(sagaConsumeContext, next).ConfigureAwait(false);
                    }
                    finally
                    {
                        switch (sagaConsumeContext)
                        {
                            case IAsyncDisposable asyncDisposable:
                                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                                break;
                            case IDisposable disposable:
                                disposable.Dispose();
                                break;
                        }
                    }
                }
                else
                {
                    var missingPipe = new MissingSagaPipe<TSaga, T>(repositoryContext, next);

                    await policy.Missing(context, missingPipe).ConfigureAwait(false);
                }
            }
            catch (SagaException exception)
            {
                await repositoryContext.Faulted(exception).ConfigureAwait(false);

                context.LogFault<TSaga, T>(this, exception, correlationId);

                throw;
            }
            catch (Exception exception)
            {
                await repositoryContext.Faulted(exception).ConfigureAwait(false);

                context.LogFault<TSaga, T>(this, exception, correlationId);

                throw new SagaException(exception.Message, typeof(TSaga), typeof(T), correlationId, exception);
            }
            finally
            {
                switch (repositoryContext)
                {
                    case IAsyncDisposable asyncDisposable:
                        await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                        break;
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                }
            }
        }

        public async Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            SagaRepositoryContext<TSaga, T> repositoryContext = await _repositoryContextFactory.CreateContext(context).ConfigureAwait(false);
            try
            {
                SagaRepositoryQueryContext<TSaga, T> queryContext = await repositoryContext.Query(context.Query).ConfigureAwait(false);
                if (queryContext.Count > 0)
                {
                    async Task LoadInstance(Guid correlationId)
                    {
                        SagaConsumeContext<TSaga, T> sagaConsumeContext = await queryContext.Load(correlationId).ConfigureAwait(false);
                        if (sagaConsumeContext != null)
                        {
                            sagaConsumeContext.LogUsed();

                            try
                            {
                                await policy.Existing(sagaConsumeContext, next).ConfigureAwait(false);
                            }
                            finally
                            {
                                switch (sagaConsumeContext)
                                {
                                    case IAsyncDisposable asyncDisposable:
                                        await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                                        break;
                                    case IDisposable disposable:
                                        disposable.Dispose();
                                        break;
                                }
                            }
                        }
                    }

                    await Task.WhenAll(queryContext.Select(LoadInstance)).ConfigureAwait(false);
                }
                else
                {
                    var missingPipe = new MissingSagaPipe<TSaga, T>(queryContext, next);

                    await policy.Missing(context, missingPipe).ConfigureAwait(false);
                }
            }
            catch (SagaException exception)
            {
                await repositoryContext.Faulted(exception).ConfigureAwait(false);

                context.LogFault<TSaga, T>(this, exception);

                throw;
            }
            catch (Exception exception)
            {
                await repositoryContext.Faulted(exception).ConfigureAwait(false);

                throw new SagaException(exception.Message, typeof(TSaga), typeof(T), Guid.Empty, exception);
            }
            finally
            {
                switch (repositoryContext)
                {
                    case IAsyncDisposable asyncDisposable:
                        await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                        break;
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                }
            }
        }

        public async Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
        {
            SagaRepositoryContext<TSaga> repositoryContext = await _repositoryContextFactory.CreateContext().ConfigureAwait(false);
            try
            {
                return await repositoryContext.Query(query).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await repositoryContext.Faulted(exception).ConfigureAwait(false);

                throw;
            }
            finally
            {
                switch (repositoryContext)
                {
                    case IAsyncDisposable asyncDisposable:
                        await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                        break;
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                }
            }
        }

        public async Task<TSaga> Load(Guid correlationId)
        {
            SagaRepositoryContext<TSaga> repositoryContext = await _repositoryContextFactory.CreateContext().ConfigureAwait(false);
            try
            {
                return await repositoryContext.Load(correlationId).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await repositoryContext.Faulted(exception).ConfigureAwait(false);

                throw;
            }
            finally
            {
                switch (repositoryContext)
                {
                    case IAsyncDisposable asyncDisposable:
                        await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                        break;
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                }
            }
        }
    }
}
