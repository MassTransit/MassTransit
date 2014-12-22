// Copyright 2007-2014 Chris Patterson
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
    using System.Threading.Tasks;


    public class GenericActivityFactory<TActivity, TArguments, TLog> :
        ActivityFactory<TArguments, TLog>
        where TActivity : ExecuteActivity<TArguments>, CompensateActivity<TLog>
        where TArguments : class
        where TLog : class
    {
        readonly ActivityFactory _activityFactory;

        public GenericActivityFactory(ActivityFactory activityFactory)
        {
            _activityFactory = activityFactory;
        }

        public Task<ExecutionResult> ExecuteActivity(Execution<TArguments> execution)
        {
            return _activityFactory.ExecuteActivity<TActivity, TArguments>(execution);
        }

        public Task<CompensationResult> CompensateActivity(Compensation<TLog> compensation)
        {
            return _activityFactory.CompensateActivity<TActivity, TLog>(compensation);
        }
    }
}