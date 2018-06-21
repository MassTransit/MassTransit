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


    public class LamarExecuteActivityScopeProvider<TActivity, TArguments> :
        IExecuteActivityScopeProvider<TActivity, TArguments>
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        private readonly IContainer _container;

        public LamarExecuteActivityScopeProvider(IContainer container)
        {
            _container = container;
        }

        public IExecuteActivityScopeContext<TActivity, TArguments> GetScope(ExecuteContext<TArguments> context)
        {
            if (context.TryGetPayload<IContainer>(out var existingContainer))
            {
                var activityFactory = existingContainer.GetInstance<LamarActivityFactory>();
                var activity = activityFactory.Get<TActivity, TArguments>(context.Arguments);

                var activityContext = new HostExecuteActivityContext<TActivity, TArguments>(activity, context);

                return new ExistingExecuteActivityScopeContext<TActivity, TArguments>(activityContext);
            }

            var scopeContainer = _container.GetNestedContainer(context.ConsumeContext);
            try
            {
                var activityFactory = scopeContainer.GetInstance<LamarActivityFactory>();
                var activity = activityFactory.Get<TActivity, TArguments>(context.Arguments);

                ExecuteActivityContext<TActivity, TArguments> activityContext = new HostExecuteActivityContext<TActivity, TArguments>(activity, context);

                var scope = scopeContainer;
                activityContext.GetOrAddPayload(() => scope);

                return new CreatedExecuteActivityScopeContext<IContainer, TActivity, TArguments>(scope, activityContext);
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
