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
namespace MassTransit.ExtensionsDependencyInjectionIntegration.ScopeProviders
{
    using System;
    using Courier;
    using Courier.Hosts;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Scoping;
    using Scoping.CourierContexts;
    using Util;


    public class DependencyInjectionCompensateActivityScopeProvider<TActivity, TLog> :
        ICompensateActivityScopeProvider<TActivity, TLog>
        where TActivity : class, CompensateActivity<TLog>
        where TLog : class
    {
        readonly IServiceProvider _serviceProvider;

        public DependencyInjectionCompensateActivityScopeProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICompensateActivityScopeContext<TActivity, TLog> GetScope(CompensateContext<TLog> context)
        {
            if (context.TryGetPayload<IServiceScope>(out var existingServiceScope))
            {
                existingServiceScope.UpdateScope(context.ConsumeContext);

                var activity = existingServiceScope.ServiceProvider.GetService<TActivity>();
                if (activity == null)
                    throw new ConsumerException($"Unable to resolve activity type '{TypeMetadataCache<TActivity>.ShortName}'.");

                CompensateActivityContext<TActivity, TLog> activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);

                return new ExistingCompensateActivityScopeContext<TActivity, TLog>(activityContext);
            }

            if (!context.TryGetPayload(out IServiceProvider serviceProvider))
                serviceProvider = _serviceProvider;

            var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            try
            {
                serviceScope.UpdateScope(context.ConsumeContext);

                var activity = serviceScope.ServiceProvider.GetService<TActivity>();
                if (activity == null)
                    throw new ConsumerException($"Unable to resolve activity type '{TypeMetadataCache<TActivity>.ShortName}'.");

                CompensateActivityContext<TActivity, TLog> activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);

                activityContext.UpdatePayload(serviceScope);

                return new CreatedCompensateActivityScopeContext<IServiceScope, TActivity, TLog>(serviceScope, activityContext);
            }
            catch
            {
                serviceScope.Dispose();

                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "dependencyInjection");
        }
    }
}
