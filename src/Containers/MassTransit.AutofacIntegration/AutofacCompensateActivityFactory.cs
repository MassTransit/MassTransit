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
namespace MassTransit.AutofacIntegration
{
    using System.Threading.Tasks;
    using Autofac;
    using Courier;
    using GreenPipes;
    using Scoping;


    /// <summary>
    /// A factory to create an activity from Autofac, that manages the lifetime scope of the activity
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TLog"></typeparam>
    public class AutofacCompensateActivityFactory<TActivity, TLog> :
        CompensateActivityFactory<TActivity, TLog>
        where TActivity : class, CompensateActivity<TLog>
        where TLog : class
    {
        readonly CompensateActivityFactory<TActivity, TLog> _factory;

        public AutofacCompensateActivityFactory(ILifetimeScope lifetimeScope, string name)
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(lifetimeScope);

            var compensateActivityScopeProvider = new AutofacCompensateActivityScopeProvider<TActivity, TLog>(lifetimeScopeProvider, name);

            _factory = new ScopeCompensateActivityFactory<TActivity, TLog>(compensateActivityScopeProvider);
        }

        public Task<ResultContext<CompensationResult>> Compensate(CompensateContext<TLog> context, IRequestPipe<CompensateActivityContext<TActivity, TLog>, CompensationResult> next)
        {
            return _factory.Compensate(context, next);
        }

        public void Probe(ProbeContext context)
        {
            _factory.Probe(context);
        }
    }
}