namespace MassTransit.Configuration
{
    using System;


    public class SagaInterfaceType
    {
        readonly Lazy<ISagaConnectorFactory> _initiatedByConnectorFactory;
        readonly Lazy<ISagaConnectorFactory> _initiatedByOrOrchestratesConnectorFactory;
        readonly Lazy<ISagaConnectorFactory> _observesConnectorFactory;
        readonly Lazy<ISagaConnectorFactory> _orchestratesConnectorFactory;

        public SagaInterfaceType(Type interfaceType, Type messageType, Type sagaType)
        {
            InterfaceType = interfaceType;
            MessageType = messageType;

            _initiatedByConnectorFactory = new Lazy<ISagaConnectorFactory>(() => (ISagaConnectorFactory)
                Activator.CreateInstance(typeof(InitiatedBySagaConnectorFactory<,>).MakeGenericType(sagaType,
                    messageType)));

            _observesConnectorFactory = new Lazy<ISagaConnectorFactory>(() => (ISagaConnectorFactory)
                Activator.CreateInstance(typeof(ObservesSagaConnectorFactory<,>).MakeGenericType(sagaType,
                    messageType)));

            _orchestratesConnectorFactory =
                new Lazy<ISagaConnectorFactory>(() => (ISagaConnectorFactory)
                    Activator.CreateInstance(typeof(OrchestratesSagaConnectorFactory<,>).MakeGenericType(
                        sagaType, messageType)));

            _initiatedByOrOrchestratesConnectorFactory =
                new Lazy<ISagaConnectorFactory>(() => (ISagaConnectorFactory)
                    Activator.CreateInstance(typeof(InitiatedByOrOrchestratesSagaConnectorFactory<,>).MakeGenericType(
                        sagaType, messageType)));
        }

        public Type InterfaceType { get; private set; }
        public Type MessageType { get; private set; }

        public ISagaMessageConnector<T> GetInitiatedByConnector<T>()
            where T : class, ISaga
        {
            return _initiatedByConnectorFactory.Value.CreateMessageConnector<T>();
        }

        public ISagaMessageConnector<T> GetOrchestratesConnector<T>()
            where T : class, ISaga
        {
            return _orchestratesConnectorFactory.Value.CreateMessageConnector<T>();
        }

        public ISagaMessageConnector<T> GetObservesConnector<T>()
            where T : class, ISaga
        {
            return _observesConnectorFactory.Value.CreateMessageConnector<T>();
        }

        public ISagaMessageConnector<T> GetInitiatedByOrOrchestratesConnector<T>()
            where T : class, ISaga
        {
            return _initiatedByOrOrchestratesConnectorFactory.Value.CreateMessageConnector<T>();
        }
    }
}
