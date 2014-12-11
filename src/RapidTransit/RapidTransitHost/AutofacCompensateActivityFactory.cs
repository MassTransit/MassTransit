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


    public class AutofacCompensateActivityFactory<TActivity, TLog> :
        CompensateActivityFactory<TLog>
        where TActivity : CompensateActivity<TLog>
        where TLog : class
    {
        readonly ILifetimeScope _lifetimeScope;

        public AutofacCompensateActivityFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public async Task<CompensationResult> CompensateActivity(Compensation<TLog> compensation)
        {
            using (ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope(x => ConfigureScope(x, compensation)))
            {
                var activity = scope.Resolve<TActivity>(TypedParameter.From(compensation.Log));

                return await activity.Compensate(compensation);
            }
        }

        static void ConfigureScope(ContainerBuilder containerBuilder, Compensation<TLog> compensation)
        {
            containerBuilder.RegisterInstance(compensation.ConsumeContext)
                .ExternallyOwned();
        }
    }
}