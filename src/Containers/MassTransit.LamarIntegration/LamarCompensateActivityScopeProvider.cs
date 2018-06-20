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
namespace MassTransit.LamarIntegration
{
    using Courier;
    using Courier.Hosts;
    using GreenPipes;
    using Lamar;
    using Scoping;
    using Scoping.CourierContexts;


    public class LamarCompensateActivityScopeProvider<TActivity, TLog> : ICompensateActivityScopeProvider<TActivity, TLog>
        where TActivity : class, CompensateActivity<TLog>
        where TLog : class
    {
        private readonly IContainer _container;

        public LamarCompensateActivityScopeProvider(IContainer container)
        {
            _container = container;
        }

        public ICompensateActivityScopeContext<TActivity, TLog> GetScope(CompensateContext<TLog> context)
        {
            if (context.TryGetPayload<IContainer>(out var existingContainer))
            {
                var activityFactory = existingContainer.GetInstance<LamarActivityFactory>();
                var activity = activityFactory.Get<TActivity, TLog>(context.Log);

                var activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);
                return new ExistingCompensateActivityScopeContext<TActivity, TLog>(activityContext);
            }

            var scopeContainer = _container.GetNestedContainer(context.ConsumeContext);
            try
            {
                var activityFactory = scopeContainer.GetInstance<LamarActivityFactory>();
                var activity = activityFactory.Get<TActivity, TLog>(context.Log);

                CompensateActivityContext<TActivity, TLog> activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);

                var scope = scopeContainer;
                activityContext.GetOrAddPayload(() => scope);

                return new CreatedCompensateActivityScopeContext<IContainer, TActivity, TLog>(scope, activityContext);
            }
            catch
            {
                scopeContainer.Dispose();
                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "lamar");
        }
    }
}
