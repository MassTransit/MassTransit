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
namespace MassTransit.Courier
{
    using Hosts;


    public static class ActivityFactoryExtensions
    {
        /// <summary>
        /// Created an activity factory for the specified activity type
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        /// <typeparam name="TLog"></typeparam>
        /// <param name="activityFactory"></param>
        /// <returns></returns>
        public static ActivityFactory<TArguments, TLog> CreateActivityFactory<TActivity, TArguments, TLog>(
            this ActivityFactory activityFactory)
            where TActivity : ExecuteActivity<TArguments>, CompensateActivity<TLog>
            where TArguments : class
            where TLog : class
        {
            return new GenericActivityFactory<TActivity, TArguments, TLog>(activityFactory);
        }
    }
}