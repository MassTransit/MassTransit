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
    using System.Collections.Generic;
    using MassTransit.Courier;
    using Testing;


    public abstract class InMemoryActivityTestFixture :
        InMemoryTestFixture
    {
        protected InMemoryActivityTestFixture()
        {
            ActivityTestContexts = new Dictionary<Type, ActivityTestContext>();

            InMemoryTestHarness.PreCreateBus += PreCreateBus;
        }

        protected IDictionary<Type, ActivityTestContext> ActivityTestContexts { get; private set; }

        void PreCreateBus(BusTestHarness harness)
        {
            SetupActivities(harness);
        }

        protected void AddActivityContext<T, TArguments, TLog>(Func<T> activityFactory,
            Action<IExecuteActivityConfigurator<T, TArguments>> configureExecute = null,
            Action<ICompensateActivityConfigurator<T, TLog>> configureCompensate = null)
            where TArguments : class
            where TLog : class
            where T : class, Activity<TArguments, TLog>
        {
            var context = new ActivityTestContext<T, TArguments, TLog>(BusTestHarness, activityFactory, configureExecute, configureCompensate);

            ActivityTestContexts.Add(typeof(T), context);
        }

        protected void AddActivityContext<T, TArguments>(Func<T> activityFactory, Action<IExecuteActivityConfigurator<T, TArguments>> configureExecute = null)
            where TArguments : class
            where T : class, ExecuteActivity<TArguments>
        {
            var context = new ActivityTestContext<T, TArguments>(BusTestHarness, activityFactory, configureExecute);

            ActivityTestContexts.Add(typeof(T), context);
        }

        protected ActivityTestContext GetActivityContext<T>()
        {
            return ActivityTestContexts[typeof(T)];
        }

        protected abstract void SetupActivities(BusTestHarness testHarness);
    }
}