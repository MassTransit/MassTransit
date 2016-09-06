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
namespace MassTransit.ConsumeConnectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using PipeConfigurators;
    using Pipeline;
    using Util;


    public class ConsumerConnector<T> :
        IConsumerConnector
        where T : class
    {
        readonly IEnumerable<IConsumerMessageConnector> _connectors;

        public ConsumerConnector()
        {
            if (TypeMetadataCache<T>.HasSagaInterfaces)
                throw new ConfigurationException("A saga cannot be registered as a consumer");

            _connectors = Consumes()
                .Distinct((x, y) => x.MessageType == y.MessageType)
                .ToList();
        }

        public IEnumerable<IConsumerMessageConnector> Connectors => _connectors;

        ConnectHandle IConsumerConnector.ConnectConsumer<TConsumer>(IConsumePipeConnector consumePipe, IConsumerFactory<TConsumer> consumerFactory,
            IPipeSpecification<ConsumerConsumeContext<TConsumer>>[] pipeSpecifications)
        {
            var handles = new List<ConnectHandle>();
            try
            {
                foreach (IConsumerMessageConnector connector in _connectors)
                {
                    ConnectHandle handle = connector.ConnectConsumer(consumePipe, consumerFactory, pipeSpecifications);

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

        static IEnumerable<IConsumerMessageConnector> Consumes()
        {
            return ConsumerMetadataCache<T>.ConsumerTypes.Select(x => x.GetConsumerConnector());
        }
    }
}