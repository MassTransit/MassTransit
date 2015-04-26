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
    using Courier;
    using Courier.Factories;
    using Courier.Hosts;


    public interface ActivityTestContext
    {
        string Name { get; }

        Uri ExecuteUri { get; }
        Uri CompensateUri { get; }

        void Configure(IInMemoryBusFactoryConfigurator configurator);
    }


    public class ActivityTestContext<T, TArguments, TLog> :
        ActivityTestContext
        where T : class, Activity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly ActivityFactory<TArguments, TLog> _activityFactory;

        public ActivityTestContext(Uri baseUri, Func<T> activityFactory)
        {
            _activityFactory = new FactoryMethodActivityFactory<T, TArguments, TLog>(_ => activityFactory(), _ => activityFactory());

            Name = GetActivityName();

            ExecuteQueueName = BuildQueueName("execute");
            CompensateQueueName = BuildQueueName("compensate");

            ExecuteUri = BuildQueueUri(baseUri, ExecuteQueueName);
            CompensateUri = BuildQueueUri(baseUri, CompensateQueueName);
        }

        public string ExecuteQueueName { get; private set; }
        public string CompensateQueueName { get; private set; }
        public string Name { get; private set; }
        public Uri ExecuteUri { get; private set; }
        public Uri CompensateUri { get; private set; }

        public void Configure(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint(ExecuteQueueName, x =>
            {
                x.ExecuteActivityHost<T, TArguments>(CompensateUri, _activityFactory);
            });

            configurator.ReceiveEndpoint(CompensateQueueName, x =>
            {
                x.CompensateActivityHost<T, TLog>(_activityFactory);
            });
        }

        static string GetActivityName()
        {
            string name = typeof(T).Name;
            if (name.EndsWith("Activity"))
                name = name.Substring(0, name.Length - "Activity".Length);
            return name;
        }

        Uri BuildQueueUri(Uri baseUri, string queueName)
        {
            return new Uri(baseUri, queueName);
        }

        string BuildQueueName(string prefix)
        {
            return string.Format("{0}_{1}", prefix, typeof(T).Name.ToLowerInvariant());
        }
    }
}