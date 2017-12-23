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
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Autofac.Core;
    using AutofacIntegration;
    using Automatonymous;
    using Automatonymous.Scoping;
    using AutomatonymousAutofacIntegration;
    using Internals.Extensions;


    public static class AutofacStateMachineLoadSagaExtensions
    {
        /// <summary>
        /// Scans the lifetime scope and registers any state machines sagas which are found in the scope using the Autofac saga repository
        /// and the appropriate state machine saga repository under the hood.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        /// <param name="name"></param>
        public static void LoadStateMachineSagas(this IReceiveEndpointConfigurator configurator, IComponentContext context, string name = "message")
        {
            var scope = context.Resolve<ILifetimeScope>();

            IEnumerable<Type> sagaTypes = FindStateMachineSagaTypes(context);

            var stateMachineFactory = new AutofacSagaStateMachineFactory(scope);

            var scopeProvider = new SingleLifetimeScopeProvider(scope);
            var repositoryFactory = new AutofacStateMachineSagaRepositoryFactory(scopeProvider, name);

            foreach (var sagaType in sagaTypes)
            {
                StateMachineSagaConfiguratorCache.Configure(sagaType, configurator, stateMachineFactory, repositoryFactory);
            }
        }

        static IEnumerable<Type> FindStateMachineSagaTypes(IComponentContext context)
        {
            return context.ComponentRegistry.Registrations
                .SelectMany(r => r.Services.OfType<IServiceWithType>(), (r, s) => new {r, s})
                .Where(rs => rs.s.ServiceType.HasInterface(typeof(SagaStateMachine<>)))
                .Select(rs => rs.s.ServiceType.GetClosingArguments(typeof(SagaStateMachine<>)).Single())
                .Distinct()
                .ToList();
        }
    }
}