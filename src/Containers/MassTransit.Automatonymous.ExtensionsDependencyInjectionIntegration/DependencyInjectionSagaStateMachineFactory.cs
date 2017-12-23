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
namespace MassTransit.AutomatonymousExtensionsDependencyInjectionIntegration
{
    using System;
    using Automatonymous;
    using Automatonymous.Scoping;
    using Microsoft.Extensions.DependencyInjection;


    public class DependencyInjectionSagaStateMachineFactory :
        ISagaStateMachineFactory
    {
        readonly IServiceProvider _serviceProvider;

        public DependencyInjectionSagaStateMachineFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        SagaStateMachine<T> ISagaStateMachineFactory.CreateStateMachine<T>()
        {
            return _serviceProvider.GetRequiredService<SagaStateMachine<T>>();
        }
    }
}