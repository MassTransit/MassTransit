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
    using Internals.Extensions;
    using Pipeline;
    using Util;


    public class InstanceConnector<TConsumer> :
        IInstanceConnector
        where TConsumer : class
    {
        readonly IEnumerable<IInstanceMessageConnector<TConsumer>> _connectors;

        public InstanceConnector()
        {
            if (TypeMetadataCache<TConsumer>.HasSagaInterfaces)
                throw new ConfigurationException("A saga cannot be registered as a consumer");

            _connectors = Consumes()
                .ToList();
        }

        public ConnectHandle ConnectInstance<T>(IConsumePipeConnector pipeConnector, T instance, IConsumerSpecification<T> specification)
            where T : class
        {
            var handles = new List<ConnectHandle>();
            try
            {
                foreach (IInstanceMessageConnector<T> connector in _connectors.Cast<IInstanceMessageConnector<T>>())
                {
                    var handle = connector.ConnectInstance(pipeConnector, instance, specification);

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

        ConnectHandle IInstanceConnector.ConnectInstance(IConsumePipeConnector pipeConnector, object instance)
        {
            var consumer = instance as TConsumer;
            if (consumer == null)
            {
                throw new ConsumerException(
                    $"The instance type {instance.GetType().GetTypeName()} does not match the consumer type: {TypeMetadataCache<TConsumer>.ShortName}");
            }

            IConsumerSpecification<TConsumer> specification = CreateConsumerSpecification<TConsumer>();

            return ConnectInstance(pipeConnector, consumer, specification);
        }

        public IConsumerSpecification<T> CreateConsumerSpecification<T>()
            where T : class
        {
            List<IConsumerMessageSpecification<T>> messageSpecifications =
                _connectors.Select(x => x.CreateConsumerMessageSpecification())
                    .Cast<IConsumerMessageSpecification<T>>()
                    .ToList();

            return new ConsumerSpecification<T>(messageSpecifications);
        }

        static IEnumerable<IInstanceMessageConnector<TConsumer>> Consumes()
        {
            return ConsumerMetadataCache<TConsumer>.ConsumerTypes.Select(x => x.GetInstanceConnector<TConsumer>());
        }
    }
}