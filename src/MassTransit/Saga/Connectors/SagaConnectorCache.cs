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
namespace MassTransit.Saga.Connectors
{
    using System;
    using System.Threading;


    /// <summary>
    /// Caches the saga connectors for the saga
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    public class SagaConnectorCache<TSaga> :
        ISagaConnectorCache
        where TSaga : class, ISaga
    {
        readonly Lazy<SagaConnector<TSaga>> _connector;

        SagaConnectorCache()
        {
            _connector = new Lazy<SagaConnector<TSaga>>(() => new SagaConnector<TSaga>(), LazyThreadSafetyMode.PublicationOnly);
        }

        public static SagaConnector Connector
        {
            get { return Cached.Instance.Value.Connector; }
        }

        SagaConnector ISagaConnectorCache.Connector
        {
            get { return _connector.Value; }
        }


        static class Cached
        {
            internal static readonly Lazy<ISagaConnectorCache> Instance = new Lazy<ISagaConnectorCache>(
                () => new SagaConnectorCache<TSaga>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}