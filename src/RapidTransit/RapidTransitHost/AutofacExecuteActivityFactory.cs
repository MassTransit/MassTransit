// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace RapidTransit
{
    using System.Threading.Tasks;
    using Autofac;
    using MassTransit.Courier;
    using MassTransit.Logging;


    public class AutofacExecuteActivityFactory<TActivity, TArguments> :
        ExecuteActivityFactory<TArguments>
        where TActivity : ExecuteActivity<TArguments>
        where TArguments : class
    {
        static readonly ILog _log = Logger.Get<AutofacExecuteActivityFactory<TActivity, TArguments>>();
        readonly ILifetimeScope _lifetimeScope;

        public AutofacExecuteActivityFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public async Task<ExecutionResult> ExecuteActivity(Execution<TArguments> execution)
        {
            using (ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope(x => ConfigureScope(x, execution)))
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("ExecuteActivityFactory: Executing: {0}", typeof(TActivity).Name);

                var activity = scope.Resolve<TActivity>(TypedParameter.From(execution.Arguments));

                return await activity.Execute(execution);
            }
        }

        static void ConfigureScope(ContainerBuilder containerBuilder, Execution<TArguments> execution)
        {
            containerBuilder.RegisterInstance(execution.ConsumeContext)
                .ExternallyOwned();
        }
    }
}