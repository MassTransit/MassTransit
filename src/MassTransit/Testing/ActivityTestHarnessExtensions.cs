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
namespace MassTransit.Testing
{
    using Courier;
    using Courier.Factories;


    public static class ActivityTestHarnessExtensions
    {
        public static ActivityTestHarness<TActivity, TArguments, TLog> Activity<TActivity, TArguments, TLog>(this BusTestHarness harness)
            where TActivity : class, Activity<TArguments, TLog>, new()
            where TArguments : class
            where TLog : class
        {
            var activityFactory = new FactoryMethodActivityFactory<TActivity, TArguments, TLog>(x => new TActivity(), x => new TActivity());

            return new ActivityTestHarness<TActivity, TArguments, TLog>(harness, activityFactory, x =>
            {
            }, x =>
            {
            });
        }
    }
}