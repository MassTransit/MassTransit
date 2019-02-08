// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace Automatonymous.Registration
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using MassTransit;
    using MassTransit.Internals.Extensions;
    using MassTransit.Registration;
    using MassTransit.Util;


    public static class SagaStateMachineRegistrationCache
    {
        static CachedRegistration GetOrAdd(Type type)
        {
            if (!type.HasInterface(typeof(SagaStateMachine<>)))
                throw new ArgumentException($"The type is not a state machine saga: {TypeMetadataCache.GetShortName(type)}", nameof(type));

            var instanceType = type.GetClosingArguments(typeof(SagaStateMachine<>)).Single();

            return Cached.Instance.GetOrAdd(instanceType,
                _ => (CachedRegistration)Activator.CreateInstance(typeof(CachedRegistration<,>).MakeGenericType(type, instanceType)));
        }

        public static void Register(Type stateMachineType, ISagaStateMachineRegistrar registrar)
        {
            GetOrAdd(stateMachineType).Register(registrar);
        }

        public static void AddSagaStateMachine(IRegistrationConfigurator configurator, Type stateMachineType, Type sagaDefinitionType = null,
            ISagaStateMachineRegistrar registrar = null)
        {
            GetOrAdd(stateMachineType).AddSaga(configurator, sagaDefinitionType, registrar ?? new NullSagaStateMachineRegistrar());
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedRegistration> Instance = new ConcurrentDictionary<Type, CachedRegistration>();
        }


        interface CachedRegistration
        {
            void Register(ISagaStateMachineRegistrar registrar);
            void AddSaga(IRegistrationConfigurator registry, Type sagaDefinitionType, ISagaStateMachineRegistrar registrar);
        }


        class CachedRegistration<TStateMachine, TInstance> :
            CachedRegistration
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            public void Register(ISagaStateMachineRegistrar registrar)
            {
                registrar.RegisterStateMachineSaga<TStateMachine, TInstance>();

                SagaRegistrationCache.DoNotRegister(typeof(TInstance));
            }

            public void AddSaga(IRegistrationConfigurator registry, Type sagaDefinitionType, ISagaStateMachineRegistrar registrar)
            {
                registry.AddSagaStateMachine<TStateMachine, TInstance>(registrar, sagaDefinitionType);
            }
        }
    }
}
