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
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using Pipeline;
    using Util;


    public class InstanceConnector<TConsumer> :
        IInstanceConnector
        where TConsumer : class
    {
        readonly IEnumerable<IInstanceMessageConnector> _connectors;

        public InstanceConnector()
        {
            if (TypeMetadataCache<TConsumer>.HasSagaInterfaces)
                throw new ConfigurationException("A saga cannot be registered as a consumer");

            _connectors = Consumes()
                .Distinct((x, y) => x.MessageType == y.MessageType)
                .ToList();
        }

        ConnectHandle IInstanceConnector.ConnectInstance<T>(IConsumePipeConnector pipe, T instance, IPipeSpecification<ConsumerConsumeContext<T>>[] pipeSpecifications)
        {
            return new MultipleConnectHandle(_connectors.Select(x => x.ConnectInstance(pipe, instance, pipeSpecifications)));
        }

        ConnectHandle IInstanceConnector.ConnectInstance(IConsumePipeConnector pipe, object instance)
        {
            return new MultipleConnectHandle(_connectors.Select(x => x.ConnectInstance(pipe, instance)));
        }

        static IEnumerable<IInstanceMessageConnector> Consumes()
        {
            return ConsumerMetadataCache<TConsumer>.ConsumerTypes.Select(x => x.GetInstanceConnector());
        }
    }
}