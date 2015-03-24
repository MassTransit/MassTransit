// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Saga.Connectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Pipeline;
    using PipeConfigurators;
    using Util;


    public interface SagaConnector
    {
        ConnectHandle Connect<T>(IConsumePipeConnector consumePipe, ISagaRepository<T> sagaRepository,
            params IPipeSpecification<SagaConsumeContext<T>>[] pipeSpecifications)
            where T : class, ISaga;
    }


    public class SagaConnector<TSaga> :
        SagaConnector
        where TSaga : class, ISaga
    {
        readonly List<SagaMessageConnector> _connectors;

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

        public IEnumerable<SagaMessageConnector> Connectors
        {
            get { return _connectors; }
        }

        public ConnectHandle Connect<T>(IConsumePipeConnector consumePipe, ISagaRepository<T> sagaRepository,
            params IPipeSpecification<SagaConsumeContext<T>>[] pipeSpecifications) 
            where T : class, ISaga
        {
            var handles = new List<ConnectHandle>();
            try
            {
                foreach (SagaMessageConnector connector in _connectors)
                {
                    ConnectHandle handle = connector.Connect(consumePipe, sagaRepository, pipeSpecifications);

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

        static IEnumerable<SagaMessageConnector> Initiates()
        {
            return SagaMetadataCache<TSaga>.InitiatedByTypes.Select(x => x.GetInitiatedByConnector());
        }

        static IEnumerable<SagaMessageConnector> Orchestrates()
        {
            return SagaMetadataCache<TSaga>.OrchestratesTypes.Select(x => x.GetOrchestratesConnector());
        }

        static IEnumerable<SagaMessageConnector> Observes()
        {
            return SagaMetadataCache<TSaga>.ObservesTypes.Select(x => x.GetObservesConnector());
        }
    }
}