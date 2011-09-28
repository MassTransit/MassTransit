// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.SubscriptionConnectors
{
    using System;
    using Magnum.Caching;
    using Magnum.Reflection;

    public class InstanceConnectorCache
    {
        [ThreadStatic]
        static InstanceConnectorCache _current;

        readonly GenericTypeCache<InstanceConnector> _connectors;

        InstanceConnectorCache()
        {
            _connectors = new GenericTypeCache<InstanceConnector>(typeof (InstanceConnector<>));
        }

        static InstanceConnectorCache Instance
        {
            get
            {
                if (_current == null)
                    _current = new InstanceConnectorCache();

                return _current;
            }
        }

        public static InstanceConnector GetInstanceConnector(Type type)
        {
            return Instance._connectors.Get(type, InstanceConnectorFactory);
        }

        public static InstanceConnector GetInstanceConnector<T>()
            where T : class
        {
            return Instance._connectors.Get(typeof (T), _ => new InstanceConnector<T>());
        }

        static InstanceConnector InstanceConnectorFactory(Type type)
        {
            return (InstanceConnector) FastActivator.Create(typeof (InstanceConnector<>), new[] {type});
        }
    }
}