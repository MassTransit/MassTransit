namespace MassTransit.AutofacIntegration
{
    using Autofac;


    public class RegistryLifetimeScopeProvider<TId> :
        ILifetimeScopeProvider
    {
        readonly ILifetimeScopeRegistry<TId> _registry;

        public RegistryLifetimeScopeProvider(ILifetimeScopeRegistry<TId> registry)
        {
            _registry = registry;
        }

        public ILifetimeScope LifetimeScope => _registry.GetLifetimeScope(default);

        public ILifetimeScope GetLifetimeScope(ConsumeContext context)
        {
            return _registry.GetLifetimeScope(default);
        }

        ILifetimeScope ILifetimeScopeProvider.GetLifetimeScope<T>(ConsumeContext<T> context)
        {
            var scopeId = GetScopeId(context);

            return _registry.GetLifetimeScope(scopeId);
        }

        TId GetScopeId<T>(ConsumeContext<T> context)
            where T : class
        {
            var scopeId = default(TId);

            // first, try to use the message-based scopeId provider
            if (_registry.TryResolve(out ILifetimeScopeIdAccessor<T, TId> provider) && provider.TryGetScopeId(context.Message, out scopeId))
                return scopeId;

            // second, try to use the consume context based message version
            var idProvider =
                _registry.ResolveOptional<ILifetimeScopeIdProvider<TId>>(TypedParameter.From(context), TypedParameter.From<ConsumeContext>(context));
            if (idProvider != null && idProvider.TryGetScopeId(out scopeId))
                return scopeId;

            // okay, give up, default it is
            return scopeId;
        }
    }
}
