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
namespace MassTransit
{
    using System;
    using Builders;


    public interface IInMemoryBusFactoryConfigurator :
        IBusFactoryConfigurator
    {
        /// <summary>
        /// Sets the transport provider for the InMemory bus, used to share a transport cache between multiple
        /// bus instances. Normally this method is not used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transportProvider"></param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
        void SetTransportProvider<T>(T transportProvider)
            where T : ISendTransportProvider, IReceiveTransportProvider;

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        void AddBusFactorySpecification(IInMemoryBusFactorySpecification configurator);

        /// <summary>
        /// Specify a receive endpoint for the bus, with the specified queue name
        /// </summary>
        /// <param name="queueName">The queue name for the receiving endpoint</param>
        /// <param name="configure">The configuration callback</param>
        void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configure);
    }
}