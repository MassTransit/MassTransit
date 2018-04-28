// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using Courier;
    using Courier.Hosts;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Scoping;
    using Scoping.CourierContexts;


    public class DependencyInjectionExecuteActivityScopeProvider<TActivity, TArguments> :
        IExecuteActivityScopeProvider<TActivity, TArguments>
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IServiceProvider _serviceProvider;

        public DependencyInjectionExecuteActivityScopeProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IExecuteActivityScopeContext<TActivity, TArguments> GetScope(ExecuteContext<TArguments> context)
        {
            if (context.TryGetPayload<IServiceProvider>(out var existingContainer))
            {
                var activity = existingContainer.GetRequiredService<TActivity>();

                ExecuteActivityContext<TActivity, TArguments> activityContext = new HostExecuteActivityContext<TActivity, TArguments>(activity, context);

                return new ExistingExecuteActivityScopeContext<TActivity, TArguments>(activityContext);
            }

            var serviceScope = _serviceProvider.CreateScope();
            try
            {
                var activity = serviceScope.ServiceProvider.GetRequiredService<TActivity>();

                ExecuteActivityContext<TActivity, TArguments> activityContext = new HostExecuteActivityContext<TActivity, TArguments>(activity, context);

                var scope = serviceScope;
                activityContext.GetOrAddPayload(() => scope);
                activityContext.GetOrAddPayload(() => scope.ServiceProvider);

                return new CreatedExecuteActivityScopeContext<IServiceScope, TActivity, TArguments>(scope, activityContext);
            }
            catch
            {
                serviceScope.Dispose();

                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "microsoft");
        }
    }
}