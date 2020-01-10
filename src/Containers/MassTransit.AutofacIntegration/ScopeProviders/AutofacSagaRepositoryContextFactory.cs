namespace MassTransit.AutofacIntegration.ScopeProviders
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Automatonymous;
    using Context;
    using GreenPipes;
    using Saga;
    using Scoping.SagaContexts;


    public class AutofacSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        const string DefaultScopeName = "message";
        readonly string _name;

        readonly ILifetimeScopeProvider _scopeProvider;

        public AutofacSagaRepositoryContextFactory(ILifetimeScope lifetimeScope)
        {
            _scopeProvider = new SingleLifetimeScopeProvider(lifetimeScope);
            _name = DefaultScopeName;
        }

        public AutofacSagaRepositoryContextFactory(ILifetimeScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
            _name = DefaultScopeName;
        }

        public AutofacSagaRepositoryContextFactory(ILifetimeScopeProvider scopeProvider, string name)
        {
            _scopeProvider = scopeProvider;
            _name = name ?? DefaultScopeName;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "dependencyInjection");
            context.Add("scopeTag", _name);
        }

        public Task<SagaRepositoryContext<TSaga, T>> CreateContext<T>(ConsumeContext<T> context, Guid? correlationId)
            where T : class
        {
            if (context.TryGetPayload<ILifetimeScope>(out var existingScope))
            {
                context.GetOrAddPayload(() => existingScope.ResolveOptional<IStateMachineActivityFactory>() ?? new AutofacStateMachineActivityFactory());

                var factory = existingScope.Resolve<ISagaRepositoryContextFactory<TSaga>>();

                return factory.CreateContext(context, correlationId);
            }

            var parentLifetimeScope = _scopeProvider.GetLifetimeScope(context);

            async Task<SagaRepositoryContext<TSaga, T>> CreateScope()
            {
                var scope = parentLifetimeScope.BeginLifetimeScope(_name, builder => builder.ConfigureScope(context));
                try
                {
                    var activityFactory = scope.ResolveOptional<IStateMachineActivityFactory>() ?? new AutofacStateMachineActivityFactory();

                    var consumeContextScope = new ConsumeContextScope<T>(context, scope, activityFactory);

                    var factory = scope.Resolve<ISagaRepositoryContextFactory<TSaga>>();

                    SagaRepositoryContext<TSaga, T> repositoryContext = await factory.CreateContext(consumeContextScope, correlationId).ConfigureAwait(false);

                    return new ScopeRepositoryContext<TSaga, T>(repositoryContext, scope);
                }
                catch
                {
                    scope.Dispose();

                    throw;
                }
            }

            return CreateScope();
        }

        public async Task<SagaRepositoryContext<TSaga>> CreateContext(CancellationToken cancellationToken = default, Guid? correlationId = default)
        {
            var factory = _scopeProvider.LifetimeScope.Resolve<ISagaRepositoryContextFactory<TSaga>>();

            var scope = _scopeProvider.LifetimeScope.BeginLifetimeScope(_name);
            try
            {
                SagaRepositoryContext<TSaga> repositoryContext = await factory.CreateContext(cancellationToken, correlationId).ConfigureAwait(false);

                var scopeRepositoryContext = new ScopeRepositoryContext<TSaga>(repositoryContext, scope);

                return scopeRepositoryContext;
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }
    }
}
