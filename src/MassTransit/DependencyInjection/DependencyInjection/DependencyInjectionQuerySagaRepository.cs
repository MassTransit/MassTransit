namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Saga;


    public class DependencyInjectionQuerySagaRepository<TSaga> :
        QuerySagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        public DependencyInjectionQuerySagaRepository(IServiceProvider provider)
            : base(new DependencyInjectionQuerySagaRepositoryContextFactory(provider))
        {
        }


        class DependencyInjectionQuerySagaRepositoryContextFactory :
            IQuerySagaRepositoryContextFactory<TSaga>
        {
            readonly IServiceProvider _serviceProvider;

            public DependencyInjectionQuerySagaRepositoryContextFactory(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public void Probe(ProbeContext context)
            {
                context.Add("provider", "dependencyInjection");
            }

            public async Task<T> Execute<T>(Func<QuerySagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken)
                where T : class
            {
                var serviceScope = _serviceProvider.CreateScope();

                try
                {
                    var factory = serviceScope.ServiceProvider.GetRequiredService<IQuerySagaRepositoryContextFactory<TSaga>>();

                    return await factory.Execute(asyncMethod, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    if (serviceScope is IAsyncDisposable asyncDisposable)
                        await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                    else
                        serviceScope.Dispose();
                }
            }
        }
    }
}
