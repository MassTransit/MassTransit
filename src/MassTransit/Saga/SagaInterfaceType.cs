// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Saga
{
    using System;
    using Connectors;


    public class SagaInterfaceType
    {
        readonly Lazy<SagaConnectorFactory> _initiatedByConnectorFactory;
        readonly Lazy<SagaConnectorFactory> _orchestratesConnectorFactory;
        readonly Lazy<SagaConnectorFactory> _observesConnectorFactory;

        public SagaInterfaceType(Type interfaceType, Type messageType, Type sagaType)
        {
            InterfaceType = interfaceType;
            MessageType = messageType;

            _initiatedByConnectorFactory = new Lazy<SagaConnectorFactory>(() => (SagaConnectorFactory)
                Activator.CreateInstance(typeof(InitiatedBySagaConnectorFactory<,>).MakeGenericType(sagaType,
                    messageType)));

            _observesConnectorFactory = new Lazy<SagaConnectorFactory>(() => (SagaConnectorFactory)
                Activator.CreateInstance(typeof(ObservesSagaConnectorFactory<,>).MakeGenericType(sagaType,
                    messageType)));

            _orchestratesConnectorFactory =
                new Lazy<SagaConnectorFactory>(() => (SagaConnectorFactory)
                    Activator.CreateInstance(typeof(OrchestratesSagaConnectorFactory<,>).MakeGenericType(
                        sagaType, messageType)));
        }

        public Type InterfaceType { get; private set; }
        public Type MessageType { get; private set; }

        public SagaMessageConnector GetInitiatedByConnector()
        {
            return _initiatedByConnectorFactory.Value.CreateMessageConnector();
        }

        public SagaMessageConnector GetOrchestratesConnector()
        {
            return _orchestratesConnectorFactory.Value.CreateMessageConnector();
        }

        public SagaMessageConnector GetObservesConnector()
        {
            return _observesConnectorFactory.Value.CreateMessageConnector();
        }
    }
}