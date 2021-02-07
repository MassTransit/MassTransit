namespace MassTransit.Conductor.Directory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using GreenPipes.Internals.Extensions;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public class ServiceRegistration<TResult> :
        IServiceRegistration<TResult>
        where TResult : class
    {
        readonly Dictionary<Type, IProviderRegistration<TResult>> _providers;

        public ServiceRegistration()
        {
            _providers = new Dictionary<Type, IProviderRegistration<TResult>>();
        }

        string DebuggerDisplay => $"{typeof(TResult).Name}: ({string.Join(", ", _providers.Select(x => x.Key.Name))})";

        public void AddProvider<TInput>(IProviderRegistration<TInput, TResult> providerRegistration)
            where TInput : class
        {
            if (_providers.ContainsKey(typeof(TInput)))
                throw new ArgumentException($"Provider already added: {TypeCache<TInput>.ShortName}");

            _providers.Add(typeof(TInput), providerRegistration);
        }

        public Type ServiceType => typeof(TResult);

        public IEnumerable<IProviderRegistration> Providers => _providers.Values;
    }
}
