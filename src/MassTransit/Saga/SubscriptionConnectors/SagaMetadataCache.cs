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
namespace MassTransit.Saga.SubscriptionConnectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;


    public class SagaMetadataCache<TSaga> :
        ISagaMetadataCache<TSaga>
        where TSaga : class, ISaga
    {
        SagaInstanceFactoryMethod<TSaga> _factoryMethod;
        readonly SagaInterfaceType[] _initiatedByTypes;
        readonly SagaInterfaceType[] _observesTypes;
        readonly SagaInterfaceType[] _orchestratesTypes;

        SagaMetadataCache()
        {
            _initiatedByTypes = GetInitiatingTypes().ToArray();
            _orchestratesTypes = GetOrchestratingTypes().ToArray();
            _observesTypes = GetObservingTypes().ToArray();

            _factoryMethod = (Guid correlationId) => (TSaga)Activator.CreateInstance(typeof(TSaga), correlationId);

            GenerateFactoryMethodAsynchronously();
        }

        public static SagaInterfaceType[] InitiatedByTypes
        {
            get { return Cached.Instance.Value.InitiatedByTypes; }
        }

        public static SagaInterfaceType[] OrchestratesTypes
        {
            get { return Cached.Instance.Value.OrchestratesTypes; }
        }

        public static SagaInterfaceType[] ObservesTypes
        {
            get { return Cached.Instance.Value.ObservesTypes; }
        }

        public static SagaInstanceFactoryMethod<TSaga> FactoryMethod
        {
            get { return Cached.Instance.Value.FactoryMethod; }
        }

        SagaInstanceFactoryMethod<TSaga> ISagaMetadataCache<TSaga>.FactoryMethod
        {
            get { return _factoryMethod; }
        }

        SagaInterfaceType[] ISagaMetadataCache<TSaga>.InitiatedByTypes
        {
            get { return _initiatedByTypes; }
        }

        SagaInterfaceType[] ISagaMetadataCache<TSaga>.OrchestratesTypes
        {
            get { return _orchestratesTypes; }
        }

        SagaInterfaceType[] ISagaMetadataCache<TSaga>.ObservesTypes
        {
            get { return _observesTypes; }
        }

        /// <summary>
        /// Creates a task to generate a compiled saga factory method that is faster than the 
        /// regular Activator, but doing this asynchronously ensures we don't slow down startup
        /// </summary>
        /// <returns></returns>
        async Task GenerateFactoryMethodAsynchronously()
        {
            await Task.Yield();

            try
            {
                var factory = new ConstructorSagaInstanceFactory<TSaga>();

                Interlocked.Exchange(ref _factoryMethod, factory.FactoryMethod);
            }
            catch (Exception)
            {
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