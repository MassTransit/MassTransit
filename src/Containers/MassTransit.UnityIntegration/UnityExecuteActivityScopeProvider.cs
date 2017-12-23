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
namespace MassTransit.UnityIntegration
{
    using Courier;
    using Courier.Hosts;
    using GreenPipes;
    using Scoping;
    using Scoping.CourierContexts;
    using Unity;
    using Unity.Injection;
    using Unity.Resolution;


    public class UnityExecuteActivityScopeProvider<TActivity, TArguments> :
        IExecuteActivityScopeProvider<TActivity, TArguments>
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IUnityContainer _container;

        public UnityExecuteActivityScopeProvider(IUnityContainer container)
        {
            _container = container;
        }

        public IExecuteActivityScopeContext<TActivity, TArguments> GetScope(ExecuteContext<TArguments> context)
        {
            if (context.TryGetPayload<IUnityContainer>(out var existingContainer))
            {
                var activity = existingContainer.Resolve<TActivity>(new DependencyOverride(typeof(TArguments), InjectionParameterValue.ToParameter(context.Arguments)));

                ExecuteActivityContext<TActivity, TArguments> activityContext = new HostExecuteActivityContext<TActivity, TArguments>(activity, context);

                return new ExistingExecuteActivityScopeContext<TActivity, TArguments>(activityContext);
            }

            var scope = _container.CreateChildContainer();
            try
            {
                var activity = scope.Resolve<TActivity>(new DependencyOverride(typeof(TArguments), InjectionParameterValue.ToParameter(context.Arguments)));

                ExecuteActivityContext<TActivity, TArguments> activityContext = new HostExecuteActivityContext<TActivity, TArguments>(activity, context);

                var contextScope = scope;
                activityContext.GetOrAddPayload(() => contextScope);

                return new CreatedExecuteActivityScopeContext<IUnityContainer, TActivity, TArguments>(contextScope, activityContext);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "unity");
        }
    }
}