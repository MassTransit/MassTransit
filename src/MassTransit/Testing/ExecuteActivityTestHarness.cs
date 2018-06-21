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
    using System;
    using Courier;


    public class ExecuteActivityTestHarness<TActivity, TArguments>
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly ExecuteActivityFactory<TActivity, TArguments> _activityFactory;
        readonly Action<IExecuteActivityConfigurator<TActivity, TArguments>> _configureExecute;

        public ExecuteActivityTestHarness(BusTestHarness testHarness, ExecuteActivityFactory<TActivity, TArguments> activityFactory,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute)
        {
            _configureExecute = configureExecute;
            _activityFactory = activityFactory;

            Name = GetActivityName();

            ExecuteQueueName = BuildQueueName("execute");

            testHarness.OnConfigureBus += ConfigureBus;
        }

        public string ExecuteQueueName { get; private set; }
        public string Name { get; private set; }
        public Uri ExecuteAddress { get; private set; }

        void ConfigureBus(IBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint(ExecuteQueueName, x =>
            {
                x.ExecuteActivityHost(_activityFactory, _configureExecute);

                ExecuteAddress = x.InputAddress;
            });
        }

        static string GetActivityName()
        {
            var name = typeof(TActivity).Name;
            if (name.EndsWith("Activity"))
                name = name.Substring(0, name.Length - "Activity".Length);
            return name;
        }

        string BuildQueueName(string prefix)
        {
            return $"{prefix}_{typeof(TActivity).Name.ToLowerInvariant()}";
        }
    }
}