// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Courier.Factories
{
    using System;
    using System.Threading.Tasks;
    using Hosts;
    using MassTransit.Pipeline;


    public class FactoryMethodCompensateActivityFactory<TActivity, TLog> :
        CompensateActivityFactory<TLog>
        where TActivity : class, CompensateActivity<TLog>
        where TLog : class
    {
        readonly Func<TLog, TActivity> _compensateFactory;

        public FactoryMethodCompensateActivityFactory(Func<TLog, TActivity> compensateFactory)
        {
            _compensateFactory = compensateFactory;
        }

        public async Task Compensate(CompensateContext<TLog> context, IPipe<CompensateActivityContext<TLog>> next)
        {
            TActivity activity = null;
            try
            {
                activity = _compensateFactory(context.Log);

                var activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);

                await next.Send(activityContext).ConfigureAwait(false);
            }
            finally
            {
                var disposable = activity as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }
    }
}