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
namespace MassTransit.AzureServiceBusTransport.Topology.Conventions.SessionId
{
    using MassTransit.Topology;
    using MassTransit.Topology.Conventions;


    public class SessionIdSendTopologyConvention :
        ISessionIdSendTopologyConvention
    {
        readonly IConventionTypeCache<IMessageSendTopologyConvention> _typeCache;

        public SessionIdSendTopologyConvention()
        {
            DefaultFormatter = new EmptySessionIdFormatter();

            _typeCache = new ConventionTypeCache<IMessageSendTopologyConvention>(typeof(ISessionIdMessageSendTopologyConvention<>),
                new CacheFactory(this));
        }

        bool IMessageSendTopologyConvention.TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
        {
            return _typeCache.GetOrAdd<T, IMessageSendTopologyConvention<T>>().TryGetMessageSendTopologyConvention(out convention);
        }

        public ISessionIdFormatter DefaultFormatter { get; set; }


        class CacheFactory :
            IConventionTypeCacheFactory<IMessageSendTopologyConvention>
        {
            readonly ISessionIdSendTopologyConvention _convention;

            public CacheFactory(ISessionIdSendTopologyConvention convention)
            {
                _convention = convention;
            }

            IMessageSendTopologyConvention IConventionTypeCacheFactory<IMessageSendTopologyConvention>.Create<T>()
            {
                return new SessionIdMessageSendTopologyConvention<T>(_convention.DefaultFormatter);
            }
        }
    }
}