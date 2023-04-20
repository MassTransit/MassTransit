namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Saga;


    public class DependencyInjectionLoadSagaRepository<TSaga> :
        LoadSagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        public DependencyInjectionLoadSagaRepository(IServiceProvider provider)
            : base(new DependencyInjectionLoadSagaRepositoryContextFactory(provider))
        {
        }


        class DependencyInjectionLoadSagaRepositoryContextFactory :
            ILoadSagaRepositoryContextFactory<TSaga>
        {
            readonly IServiceProvider _serviceProvider;

            public DependencyInjectionLoadSagaRepositoryContextFactory(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public async Task<T> Execute<T>(Func<LoadSagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
                where T : class
            {
                var serviceScope = _serviceProvider.CreateScope();

                try
                {
                    var factory = serviceScope.ServiceProvider.GetRequiredService<ILoadSagaRepositoryContextFactory<TSaga>>();

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

            public void Probe(ProbeContext context)
            {
                context.Add("provider", "dependencyInjection");
            }
        }
    }
}
