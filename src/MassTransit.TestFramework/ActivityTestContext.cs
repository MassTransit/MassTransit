// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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


    public interface ActivityTestContext
    {
        string Name { get; }

        Uri ExecuteUri { get; }

        void Configure(ActivityTestContextConfigurator configurator);
    }


    public class ActivityTestContext<TActivity, TArguments, TLog> :
        ActivityTestContext
        where TActivity : class, Activity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly Action<IExecuteActivityConfigurator<TActivity, TArguments>> _configureExecute;
        readonly Action<ICompensateActivityConfigurator<TActivity, TLog>> _configureCompensate;
        readonly ActivityFactory<TActivity, TArguments, TLog> _activityFactory;

        public ActivityTestContext(Uri baseUri, Func<TActivity> activityFactory, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute, Action<ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate)
        {
            _configureExecute = configureExecute;
            _configureCompensate = configureCompensate;
            _activityFactory = new FactoryMethodActivityFactory<TActivity, TArguments, TLog>(_ => activityFactory(), _ => activityFactory());

            Name = GetActivityName();

            ExecuteQueueName = BuildQueueName("execute");
            CompensateQueueName = BuildQueueName("compensate");

            ExecuteUri = BuildQueueUri(baseUri, ExecuteQueueName);
            CompensateUri = BuildQueueUri(baseUri, CompensateQueueName);
        }

        public string ExecuteQueueName { get; private set; }
        public string CompensateQueueName { get; private set; }
        public Uri CompensateUri { get; private set; }
        public string Name { get; private set; }
        public Uri ExecuteUri { get; private set; }

        public void Configure(ActivityTestContextConfigurator configurator)
        {
            configurator.ReceiveEndpoint(ExecuteQueueName, x => x.ExecuteActivityHost(CompensateUri, _activityFactory, _configureExecute));

            configurator.ReceiveEndpoint(CompensateQueueName, x => x.CompensateActivityHost(_activityFactory, _configureCompensate));
        }

        static string GetActivityName()
        {
            var name = typeof(TActivity).Name;
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
            return $"{prefix}_{typeof(TActivity).Name.ToLowerInvariant()}";
        }
    }


    public class ActivityTestContext<TActivity, TArguments> :
        ActivityTestContext
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly ExecuteActivityFactory<TActivity, TArguments> _activityFactory;
        readonly Action<IExecuteActivityConfigurator<TActivity, TArguments>> _configure;

        public ActivityTestContext(Uri baseUri, Func<TActivity> activityFactory, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
        {
            _configure = configure;
            _activityFactory = new FactoryMethodExecuteActivityFactory<TActivity, TArguments>(_ => activityFactory());

            Name = GetActivityName();

            ExecuteQueueName = BuildQueueName("execute");

            ExecuteUri = BuildQueueUri(baseUri, ExecuteQueueName);
        }

        public string ExecuteQueueName { get; private set; }
        public string Name { get; private set; }
        public Uri ExecuteUri { get; private set; }

        public void Configure(ActivityTestContextConfigurator configurator)
        {
            configurator.ReceiveEndpoint(ExecuteQueueName, x => x.ExecuteActivityHost(_activityFactory, h => _configure?.Invoke(h)));
        }

        static string GetActivityName()
        {
            var name = typeof(TActivity).Name;
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
            return $"{prefix}_{typeof(TActivity).Name.ToLowerInvariant()}";
        }
    }
}