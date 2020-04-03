namespace MassTransit.AutofacIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Autofac;
    using Autofac.Core;
    using Autofac.Core.Lifetime;
    using Autofac.Core.Resolving;


    public class LifetimeScopeRegistry<TId> :
        ILifetimeScopeRegistry<TId>
    {
        readonly ILifetimeScopeIdProvider<TId> _currentScopeIdProvider;
        readonly Lazy<ILifetimeScope> _defaultScope;
        readonly ILifetimeScope _parentScope;
        readonly ConcurrentDictionary<TId, RegisteredLifetimeScope> _scopes;
        readonly object _tag;
        bool _disposed;

        public LifetimeScopeRegistry(ILifetimeScope parentScope, object tag)
            : this(parentScope, tag, new DefaultScopeIdProvider())
        {
        }

        public LifetimeScopeRegistry(ILifetimeScope parentScope, object tag, ILifetimeScopeIdProvider<TId> currentScopeIdProvider)
        {
            _parentScope = parentScope;
            _tag = tag;
            _currentScopeIdProvider = currentScopeIdProvider;

            _scopes = new ConcurrentDictionary<TId, RegisteredLifetimeScope>();
            _defaultScope = new Lazy<ILifetimeScope>(() => _parentScope.BeginLifetimeScope());
        }

        public object ResolveComponent(ResolveRequest request)
        {
            return GetCurrentScope().ResolveComponent(request);
        }

        public IComponentRegistry ComponentRegistry => GetCurrentScope().ComponentRegistry;

        public ILifetimeScope BeginLifetimeScope()
        {
            return GetCurrentScope().BeginLifetimeScope();
        }

        public ILifetimeScope BeginLifetimeScope(object tag)
        {
            return GetCurrentScope().BeginLifetimeScope(tag);
        }

        public ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction)
        {
            return GetCurrentScope().BeginLifetimeScope(configurationAction);
        }

        public ILifetimeScope BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction)
        {
            return GetCurrentScope().BeginLifetimeScope(tag, configurationAction);
        }

        public IDisposer Disposer => GetCurrentScope().Disposer;

        public object Tag => GetCurrentScope().Tag;

        /// <summary>
        /// Fired when a new scope based on the current scope is beginning.
        /// </summary>
        public event EventHandler<LifetimeScopeBeginningEventArgs> ChildLifetimeScopeBeginning
        {
            add => GetCurrentScope().ChildLifetimeScopeBeginning += value;

            remove => GetCurrentScope().ChildLifetimeScopeBeginning -= value;
        }

        /// <summary>
        /// Fired when this scope is ending.
        /// </summary>
        public event EventHandler<LifetimeScopeEndingEventArgs> CurrentScopeEnding
        {
            add => GetCurrentScope().CurrentScopeEnding += value;

            remove => GetCurrentScope().CurrentScopeEnding -= value;
        }

        /// <summary>
        /// Fired when a resolve operation is beginning in this scope.
        /// </summary>
        public event EventHandler<ResolveOperationBeginningEventArgs> ResolveOperationBeginning
        {
            add => GetCurrentScope().ResolveOperationBeginning += value;

            remove => GetCurrentScope().ResolveOperationBeginning -= value;
        }

        public ILifetimeScope GetLifetimeScope(TId scopeId)
        {
            if (scopeId == null)
                return _defaultScope.Value;

            return GetOrAddLifetimeScope(scopeId);
        }

        public void ConfigureLifetimeScope(TId scopeId, LifetimeScopeConfigurator<TId> configureCallback)
        {
            _scopes.GetOrAdd(scopeId, key => new RegisteredLifetimeScope(_parentScope, _tag, key, configureCallback));
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            foreach (var scope in _scopes.Values)
                scope.Dispose();

            if (_defaultScope.IsValueCreated)
                _defaultScope.Value.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var scope in _scopes.Values)
                await scope.DisposeAsync().ConfigureAwait(false);

            if (_defaultScope.IsValueCreated)
                await _defaultScope.Value.DisposeAsync().ConfigureAwait(false);

            _disposed = true;
        }

        ILifetimeScope GetOrAddLifetimeScope(TId id)
        {
            return _scopes.GetOrAdd(id, key => new RegisteredLifetimeScope(_parentScope, _tag, key)).Scope;
        }

        /// <summary>
        /// Uses the parent container content to find the scope to use
        /// </summary>
        /// <returns></returns>
        ILifetimeScope GetCurrentScope()
        {
            if (_currentScopeIdProvider.TryGetScopeId(out var scopeId))
                return GetLifetimeScope(scopeId);

            return _defaultScope.Value;
        }


        class DefaultScopeIdProvider :
            ILifetimeScopeIdProvider<TId>
        {
            public bool TryGetScopeId(out TId id)
            {
                id = default;
                return false;
            }
        }


        class RegisteredLifetimeScope :
            IAsyncDisposable
        {
            readonly LifetimeScopeConfigurator<TId> _configurator;
            readonly TId _id;
            readonly ILifetimeScope _parentScope;
            readonly Lazy<ILifetimeScope> _scope;
            readonly object _tag;

            public RegisteredLifetimeScope(ILifetimeScope parentScope, object tag, TId id)
                : this(parentScope, tag, id, DefaultConfigurator)
            {
            }

            public RegisteredLifetimeScope(ILifetimeScope parentScope, object tag, TId id, LifetimeScopeConfigurator<TId> configurator)
            {
                _parentScope = parentScope;
                _configurator = configurator;
                _tag = tag;
                _id = id;
                _scope = new Lazy<ILifetimeScope>(CreateScope);
            }

            public ILifetimeScope Scope => _scope.Value;

            public void Dispose()
            {
                if (_scope.IsValueCreated)
                    _scope.Value.Dispose();
            }

            ILifetimeScope CreateScope()
            {
                return _parentScope.BeginLifetimeScope(_tag, x => _configurator(_id, x));
            }

            static void DefaultConfigurator(TId id, ContainerBuilder containerBuilder)
            {
            }

            public async ValueTask DisposeAsync()
            {
                if (_scope.IsValueCreated)
                    await _scope.Value.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
