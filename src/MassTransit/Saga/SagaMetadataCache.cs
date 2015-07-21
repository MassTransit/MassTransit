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
namespace MassTransit.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Util;


    public class SagaMetadataCache<TSaga> :
        ISagaMetadataCache<TSaga>
        where TSaga : class, ISaga
    {
        static readonly ILog _log = Logger.Get<SagaMetadataCache<TSaga>>();

        readonly SagaInterfaceType[] _initiatedByTypes;
        readonly SagaInterfaceType[] _observesTypes;
        readonly SagaInterfaceType[] _orchestratesTypes;
        SagaInstanceFactoryMethod<TSaga> _factoryMethod;

        SagaMetadataCache()
        {
            _initiatedByTypes = GetInitiatingTypes().ToArray();
            _orchestratesTypes = GetOrchestratingTypes().ToArray();
            _observesTypes = GetObservingTypes().ToArray();

            _factoryMethod = correlationId => (TSaga)Activator.CreateInstance(typeof(TSaga), correlationId);

            // ReSharper disable once CSharpWarnings::CS4014
            GenerateFactoryMethodAsynchronously();
        }

        public static SagaInterfaceType[] InitiatedByTypes => Cached.Instance.Value.InitiatedByTypes;

        public static SagaInterfaceType[] OrchestratesTypes => Cached.Instance.Value.OrchestratesTypes;

        public static SagaInterfaceType[] ObservesTypes => Cached.Instance.Value.ObservesTypes;

        public static SagaInstanceFactoryMethod<TSaga> FactoryMethod => Cached.Instance.Value.FactoryMethod;

        SagaInstanceFactoryMethod<TSaga> ISagaMetadataCache<TSaga>.FactoryMethod => _factoryMethod;

        SagaInterfaceType[] ISagaMetadataCache<TSaga>.InitiatedByTypes => _initiatedByTypes;

        SagaInterfaceType[] ISagaMetadataCache<TSaga>.OrchestratesTypes => _orchestratesTypes;

        SagaInterfaceType[] ISagaMetadataCache<TSaga>.ObservesTypes => _observesTypes;

        /// <summary>
        /// Creates a task to generate a compiled saga factory method that is faster than the 
        /// regular Activator, but doing this asynchronously ensures we don't slow down startup
        /// </summary>
        /// <returns></returns>
        async void GenerateFactoryMethodAsynchronously()
        {
            await Task.Yield();

            try
            {
                var factory = new ConstructorSagaInstanceFactory<TSaga>();

                Interlocked.Exchange(ref _factoryMethod, factory.FactoryMethod);
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.Error($"Failed to generate constructor instance factory for {TypeMetadataCache<TSaga>.ShortName}", ex);
            }
        }

        static IEnumerable<SagaInterfaceType> GetInitiatingTypes()
        {
            return typeof(TSaga).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(InitiatedBy<>))
                .Select(x => new SagaInterfaceType(x, x.GetGenericArguments()[0], typeof(TSaga)))
                .Where(x => x.MessageType.IsValueType == false && x.MessageType != typeof(string));
        }

        static IEnumerable<SagaInterfaceType> GetOrchestratingTypes()
        {
            return typeof(TSaga).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(Orchestrates<>))
                .Select(x => new SagaInterfaceType(x, x.GetGenericArguments()[0], typeof(TSaga)))
                .Where(x => x.MessageType.IsValueType == false && x.MessageType != typeof(string));
        }

        static IEnumerable<SagaInterfaceType> GetObservingTypes()
        {
            return typeof(TSaga).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(Observes<,>))
                .Select(x => new SagaInterfaceType(x, x.GetGenericArguments()[0], typeof(TSaga)))
                .Where(x => x.MessageType.IsValueType == false && x.MessageType != typeof(string));
        }


        static class Cached
        {
            internal static readonly Lazy<ISagaMetadataCache<TSaga>> Instance = new Lazy<ISagaMetadataCache<TSaga>>(
                () => new SagaMetadataCache<TSaga>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}