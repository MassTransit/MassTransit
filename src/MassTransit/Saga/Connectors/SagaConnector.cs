namespace MassTransit.Saga.Connectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Pipeline;
    using PipeConfigurators;
    using Util;


    public class SagaConnector<TSaga> :
        ISagaConnector
        where TSaga : class, ISaga
    {
        readonly List<ISagaMessageConnector> _connectors;

        public SagaConnector()
        {
            try
            {
                if (!TypeMetadataCache<TSaga>.HasSagaInterfaces)
                {
                    throw new ConfigurationException("The specified type is does not support any saga methods: "
                        + TypeMetadataCache<TSaga>.ShortName);
                }

                _connectors = Initiates()
                    .Concat(Orchestrates())
                    .Concat(Observes())
                    .Distinct((x, y) => x.MessageType == y.MessageType)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ConfigurationException("Failed to create the saga connector for " + TypeMetadataCache<TSaga>.ShortName, ex);
            }
        }

        public IEnumerable<ISagaMessageConnector> Connectors => _connectors;

        public ConnectHandle ConnectSaga<T>(IConsumePipeConnector consumePipe, ISagaRepository<T> sagaRepository,
            params IPipeSpecification<SagaConsumeContext<T>>[] pipeSpecifications) 
            where T : class, ISaga
        {
            var handles = new List<ConnectHandle>();
            try
            {
                foreach (ISagaMessageConnector connector in _connectors)
                {
                    ConnectHandle handle = connector.ConnectSaga(consumePipe, sagaRepository, pipeSpecifications);

                    handles.Add(handle);
                }

                return new MultipleConnectHandle(handles);
            }
            catch (Exception)
            {
                foreach (ConnectHandle handle in handles)
                    handle.Dispose();
                throw;
            }
        }

        static IEnumerable<ISagaMessageConnector> Initiates()
        {
            return SagaMetadataCache<TSaga>.InitiatedByTypes.Select(x => x.GetInitiatedByConnector());
        }

        static IEnumerable<ISagaMessageConnector> Orchestrates()
        {
            return SagaMetadataCache<TSaga>.OrchestratesTypes.Select(x => x.GetOrchestratesConnector());
        }

        static IEnumerable<ISagaMessageConnector> Observes()
        {
            return SagaMetadataCache<TSaga>.ObservesTypes.Select(x => x.GetObservesConnector());
        }
    }
}