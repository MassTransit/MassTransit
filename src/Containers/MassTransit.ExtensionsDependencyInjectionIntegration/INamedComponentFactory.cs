namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;


    public interface INamedComponentFactory
    {
        IEnumerable<string> Names { get; }
        IBusControl GetBus(string name);
        IClientFactory GetClientFactory(string name);
        IClientFactory GetServiceClientFactory(string name);
    }


    public class NamedComponentFactory : INamedComponentFactory
    {
        readonly IReadOnlyDictionary<string, IComponentRegistry> _registries;
        readonly ConcurrentDictionary<string, IBusControl> _busCache = new ConcurrentDictionary<string, IBusControl>();
        readonly ConcurrentDictionary<string, IClientFactory> _clientCache = new ConcurrentDictionary<string, IClientFactory>();

        public NamedComponentFactory(IEnumerable<IComponentRegistry> registries)
        {
            _registries = registries.ToDictionary(x => x.Name);
        }

        public IEnumerable<string> Names => _registries.Keys;

        public IBusControl GetBus(string name)
        {
            if (!_registries.TryGetValue(name, out var registry))
                throw new ApplicationException();
            return _busCache.GetOrAdd(name, registry.BusControl);
        }

        public IClientFactory GetClientFactory(string name)
        {
            return _clientCache.GetOrAdd(name, n => GetBus(n).CreateClientFactory());
        }

        public IClientFactory GetServiceClientFactory(string name)
        {
            throw new NotImplementedException();
        }
    }
}
