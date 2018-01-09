// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;
    using MassTransit.Pipeline;
    using Metadata;
    using SagaSpecifications;
    using Util;


    public class SagaConnector<TSaga> :
        ISagaConnector
        where TSaga : class, ISaga
    {
        readonly List<ISagaMessageConnector<TSaga>> _connectors;

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

        ISagaSpecification<T> ISagaConnector.CreateSagaSpecification<T>()
        {
            List<ISagaMessageSpecification<T>> messageSpecifications =
                _connectors.Select(x => x.CreateSagaMessageSpecification())
                    .Cast<ISagaMessageSpecification<T>>()
                    .ToList();

            return new SagaSpecification<T>(messageSpecifications);
        }

        ConnectHandle ISagaConnector.ConnectSaga<T>(IConsumePipeConnector consumePipe, ISagaRepository<T> repository, ISagaSpecification<T> specification)
        {
            var handles = new List<ConnectHandle>();
            try
            {
                foreach (ISagaMessageConnector<T> connector in _connectors.Cast<ISagaMessageConnector<T>>())
                {
                    var handle = connector.ConnectSaga(consumePipe, repository, specification);

                    handles.Add(handle);
                }

                return new MultipleConnectHandle(handles);
            }
            catch (Exception)
            {
                foreach (var handle in handles)
                    handle.Dispose();
                throw;
            }
        }

        static IEnumerable<ISagaMessageConnector<TSaga>> Initiates()
        {
            return SagaMetadataCache<TSaga>.InitiatedByTypes.Select(x => x.GetInitiatedByConnector<TSaga>());
        }

        static IEnumerable<ISagaMessageConnector<TSaga>> Orchestrates()
        {
            return SagaMetadataCache<TSaga>.OrchestratesTypes.Select(x => x.GetOrchestratesConnector<TSaga>());
        }

        static IEnumerable<ISagaMessageConnector<TSaga>> Observes()
        {
            return SagaMetadataCache<TSaga>.ObservesTypes.Select(x => x.GetObservesConnector<TSaga>());
        }
    }
}