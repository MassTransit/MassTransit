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
namespace MassTransit.ConsumeConnectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsumerSpecifications;
    using GreenPipes;
    using Metadata;
    using Pipeline;
    using Util;


    public class ConsumerConnector<T> :
        IConsumerConnector
        where T : class
    {
        readonly IEnumerable<IConsumerMessageConnector<T>> _connectors;

        public ConsumerConnector()
        {
            if (TypeMetadataCache<T>.HasSagaInterfaces)
                throw new ConfigurationException("A saga cannot be registered as a consumer");

            _connectors = Consumes()
                .ToList();
        }

        public IEnumerable<IConsumerMessageConnector> Connectors => _connectors;

        ConnectHandle IConsumerConnector.ConnectConsumer<TConsumer>(IConsumePipeConnector consumePipe, IConsumerFactory<TConsumer> consumerFactory,
            IConsumerSpecification<TConsumer> specification)
        {
            var handles = new List<ConnectHandle>();
            try
            {
                foreach (IConsumerMessageConnector<TConsumer> connector in _connectors.Cast<IConsumerMessageConnector<TConsumer>>())
                {
                    var handle = connector.ConnectConsumer(consumePipe, consumerFactory, specification);

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

        IConsumerSpecification<TConsumer> IConsumerConnector.CreateConsumerSpecification<TConsumer>()
        {
            List<IConsumerMessageSpecification<TConsumer>> messageSpecifications =
                _connectors.Select(x => x.CreateConsumerMessageSpecification())
                    .Cast<IConsumerMessageSpecification<TConsumer>>()
                    .ToList();

            return new ConsumerSpecification<TConsumer>(messageSpecifications);
        }

        static IEnumerable<IConsumerMessageConnector<T>> Consumes()
        {
            return ConsumerMetadataCache<T>.ConsumerTypes.Select(x => x.GetConsumerConnector<T>());
        }
    }
}