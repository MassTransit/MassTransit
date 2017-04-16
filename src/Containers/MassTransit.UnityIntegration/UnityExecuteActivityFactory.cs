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
    using System.Threading.Tasks;
    using Courier;
    using Courier.Hosts;
    using GreenPipes;
    using Logging;
    using Microsoft.Practices.Unity;
    using Util;


    public class UnityExecuteActivityFactory<TActivity, TArguments> :
        ExecuteActivityFactory<TActivity, TArguments>
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        static readonly ILog _log = Logger.Get<UnityExecuteActivityFactory<TActivity, TArguments>>();
        readonly IUnityContainer _container;

        public UnityExecuteActivityFactory(IUnityContainer container)
        {
            _container = container;
        }

        public async Task<ResultContext<ExecutionResult>> Execute(ExecuteContext<TArguments> context,
            IRequestPipe<ExecuteActivityContext<TActivity, TArguments>, ExecutionResult> next)
        {
            using (var childContainer = _container.CreateChildContainer())
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("ExecuteActivityFactory: Executing: {0}", TypeMetadataCache<TActivity>.ShortName);

                childContainer.RegisterInstance(typeof(ExecuteContext), context);


                var activity = childContainer.Resolve<TActivity>();


                ExecuteActivityContext<TActivity, TArguments> activityContext = new HostExecuteActivityContext<TActivity, TArguments>(activity, context);

                var consumerLifetimeScope = childContainer;
                activityContext.GetOrAddPayload(() => consumerLifetimeScope);

                return await next.Send(activityContext).ConfigureAwait(false);
            }
        }
    }
}