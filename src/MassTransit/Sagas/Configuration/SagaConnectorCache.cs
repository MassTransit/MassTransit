namespace MassTransit.Configuration
{
    using System;
    using System.Threading;


    /// <summary>
    /// Caches the saga connectors for the saga
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    public class SagaConnectorCache<TSaga> :
        ISagaConnectorCache
        where TSaga : class, ISaga
    {
        readonly Lazy<SagaConnector<TSaga>> _connector;

        SagaConnectorCache()
        {
            _connector = new Lazy<SagaConnector<TSaga>>(() => new SagaConnector<TSaga>(), LazyThreadSafetyMode.PublicationOnly);
        }

        public static ISagaConnector Connector => Cached.Instance.Value.Connector;

        ISagaConnector ISagaConnectorCache.Connector => _connector.Value;


        static class Cached
        {
            internal static readonly Lazy<ISagaConnectorCache> Instance = new Lazy<ISagaConnectorCache>(
                () => new SagaConnectorCache<TSaga>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
