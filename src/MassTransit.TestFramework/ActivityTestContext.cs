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
namespace MassTransit.TestFramework
{
    using System;
    using MassTransit.Courier;
    using MassTransit.Courier.Factories;
    using Testing;


    public interface ActivityTestContext
    {
        string Name { get; }

        Uri ExecuteUri { get; }
    }


    public class ActivityTestContext<TActivity, TArguments, TLog> :
        ActivityTestContext
        where TActivity : class, Activity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        ActivityTestHarness<TActivity, TArguments, TLog> _harness;

        public ActivityTestContext(BusTestHarness testHarness, Func<TActivity> activityFactory,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate)
        {
            var factory = new FactoryMethodActivityFactory<TActivity, TArguments, TLog>(_ => activityFactory(), _ => activityFactory());

            _harness = new ActivityTestHarness<TActivity, TArguments, TLog>(testHarness, factory, configureExecute, configureCompensate);
        }

        public Uri ExecuteUri => _harness.ExecuteAddress;
        public string Name => _harness.Name;
    }


    public class ActivityTestContext<TActivity, TArguments> :
        ActivityTestContext
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly ExecuteActivityTestHarness<TActivity, TArguments> _harness;

        public ActivityTestContext(BusTestHarness testHarness, Func<TActivity> activityFactory,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute)
        {
            var factory = new FactoryMethodExecuteActivityFactory<TActivity, TArguments>(_ => activityFactory());

            _harness = new ExecuteActivityTestHarness<TActivity, TArguments>(testHarness, factory, configureExecute);
        }

        public Uri ExecuteUri => _harness.ExecuteAddress;
        public string Name => _harness.Name;
    }
}