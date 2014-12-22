// Copyright 2007-2013 Chris Patterson
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
namespace MassTransit.Courier.Hosts
{
    using System;
    using System.Threading.Tasks;


    public class FactoryMethodCompensateActivityFactory<TActivity, TLog> :
        CompensateActivityFactory<TLog>
        where TActivity : CompensateActivity<TLog>
        where TLog : class
    {
        readonly Func<TLog, TActivity> _compensateFactory;

        public FactoryMethodCompensateActivityFactory(Func<TLog, TActivity> compensateFactory)
        {
            _compensateFactory = compensateFactory;
        }

        public Task<CompensationResult> CompensateActivity(Compensation<TLog> compensation)
        {
            TActivity activity = _compensateFactory(compensation.Log);

            return activity.Compensate(compensation);
        }
    }
}