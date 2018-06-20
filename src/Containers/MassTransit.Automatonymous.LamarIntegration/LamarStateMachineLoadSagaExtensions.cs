// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Automatonymous;
    using Automatonymous.Scoping;
    using AutomatonymousLamarIntegration;
    using Internals.Extensions;
    using Lamar;


    public static class LamarStateMachineLoadSagaExtensions
    {
        /// <summary>
        /// Scans the lifetime scope and registers any state machines sagas which are found in the scope using the StructureMap saga repository
        /// and the appropriate state machine saga repository under the hood.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        public static void LoadStateMachineSagas(this IReceiveEndpointConfigurator configurator, IContainer container)
        {
            IList<Type> sagaTypes = FindStateMachineSagaTypes(container);

            var stateMachineFactory = new LamarSagaStateMachineFactory(container);

            var repositoryFactory = new LamarStateMachineSagaRepositoryFactory(container);

            foreach (var sagaType in sagaTypes)
            {
                StateMachineSagaConfiguratorCache.Configure(sagaType, configurator, stateMachineFactory, repositoryFactory);
            }
        }

        public static IList<Type> FindStateMachineSagaTypes(IContainer container)
        {
            return container
                .Model
                .AllInstances.ToArray()
                .Where(x => x.ImplementationType.HasInterface(typeof(SagaStateMachine<>)))
                .Select(x => x.ImplementationType.GetClosingArguments(typeof(SagaStateMachine<>)).First())
                .Distinct()
                .ToList();
        }
    }
}
