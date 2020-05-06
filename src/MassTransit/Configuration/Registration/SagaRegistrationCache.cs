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
namespace MassTransit.Registration
{
    using System;
    using System.Collections.Concurrent;
    using Saga;


    public static class SagaRegistrationCache
    {
        static CachedRegistration GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ => (CachedRegistration)Activator.CreateInstance(typeof(CachedRegistration<>).MakeGenericType(type)));
        }

        public static void Register(Type sagaType, IContainerRegistrar registrar)
        {
            GetOrAdd(sagaType).Register(registrar);
        }

        public static ISagaRegistration CreateRegistration(Type sagaType, Type sagaDefinitionType, IContainerRegistrar registrar)
        {
            return GetOrAdd(sagaType).CreateRegistration(sagaDefinitionType, registrar);
        }

        /// <summary>
        /// Sets a saga type so that it will not be registered. This is used to allow state machines to register without a conflicting
        /// standard saga from also being registered.
        /// </summary>
        /// <param name="sagaType"></param>
        public static void DoNotRegister(Type sagaType)
        {
            GetOrAdd(sagaType).DoNotRegister();
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedRegistration> Instance = new ConcurrentDictionary<Type, CachedRegistration>();
        }


        interface CachedRegistration
        {
            void Register(IContainerRegistrar registrar);
            ISagaRegistration CreateRegistration(Type sagaDefinitionType, IContainerRegistrar registrar);

            void DoNotRegister();
        }


        class CachedRegistration<T> :
            CachedRegistration
            where T : class, ISaga
        {
            bool _doNotRegister;
            IContainerRegistrar _registrar;

            public void Register(IContainerRegistrar registrar)
            {
                _registrar = registrar;
                if (_doNotRegister)
                    return;

                _registrar.RegisterSaga<T>();
            }

            public ISagaRegistration CreateRegistration(Type sagaDefinitionType, IContainerRegistrar registrar)
            {
                Register(registrar);

                if (sagaDefinitionType != null)
                    SagaDefinitionRegistrationCache.Register(sagaDefinitionType, registrar);

                return new SagaRegistration<T>();
            }

            public void DoNotRegister()
            {
                _doNotRegister = true;
            }
        }
    }
}
