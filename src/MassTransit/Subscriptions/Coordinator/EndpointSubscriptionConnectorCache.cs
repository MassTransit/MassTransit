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
namespace MassTransit.Subscriptions.Coordinator
{
    using System;
    using Logging;
    using Magnum.Caching;
    using Magnum.Reflection;
    using Util;


    public class EndpointSubscriptionConnectorCache
    {
        static readonly ILog _log = Logger.Get(typeof(EndpointSubscriptionConnectorCache));
        readonly IServiceBus _bus;

        readonly Cache<Type, EndpointSubscriptionConnector> _cache;

        public EndpointSubscriptionConnectorCache(IServiceBus bus)
        {
            _bus = bus;
            _cache = new GenericTypeCache<EndpointSubscriptionConnector>(typeof(EndpointSubscriptionConnector<>),
                CreateConnector);
        }

        public UnsubscribeAction Connect(string messageName, Uri endpointUri, string correlationId)
        {
            Type messageType = Type.GetType(messageName);
            if (messageType == null)
            {
                _log.InfoFormat("Unknown message type '{0}', unable to add subscription", messageName);
                return () => true;
            }

            EndpointSubscriptionConnector connector = _cache[messageType];

            return connector.Connect(endpointUri, correlationId);
        }

        EndpointSubscriptionConnector CreateConnector(Type messageType)
        {
            return this.FastInvoke<EndpointSubscriptionConnectorCache, EndpointSubscriptionConnector>(
                new[] {messageType}, "CreateConnector", _bus);
        }

        [UsedImplicitly]
        EndpointSubscriptionConnector CreateConnector<TMessage>(IServiceBus bus)
            where TMessage : class
        {
            return new EndpointSubscriptionConnector<TMessage>(bus);
        }
    }
}