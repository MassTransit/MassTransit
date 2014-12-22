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
namespace MassTransit.TestFramework
{
    using System;
    using System.Collections.Generic;
    using Courier;
    using NUnit.Framework;
    using Saga;


    [TestFixture]
    public abstract class ActivityTestFixture :
        InMemoryTestFixture
    {
        protected ActivityTestFixture()
        {
            ActivityTestContexts = new Dictionary<Type, ActivityTestContext>();
        }

        protected IDictionary<Type, ActivityTestContext> ActivityTestContexts { get; private set; }

        protected override void ConfigureBus(IInMemoryServiceBusFactoryConfigurator configurator)
        {
            SetupActivities();

            foreach (ActivityTestContext activityTestContext in ActivityTestContexts.Values)
                activityTestContext.Configure(configurator);
        }

        protected void AddActivityContext<T, TArguments, TLog>(Func<T> activityFactory)
            where TArguments : class
            where TLog : class
            where T : Activity<TArguments, TLog>
        {
            var context = new ActivityTestContext<T, TArguments, TLog>(new Uri("loopback://localhost/"), activityFactory);

            ActivityTestContexts.Add(typeof(T), context);
        }

        protected ActivityTestContext GetActivityContext<T>()
        {
            return ActivityTestContexts[typeof(T)];
        }

        protected static InMemorySagaRepository<TSaga> SetupSagaRepository<TSaga>()
            where TSaga : class, ISaga
        {
            var sagaRepository = new InMemorySagaRepository<TSaga>();

            return sagaRepository;
        }

        protected abstract void SetupActivities();
    }
}