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
namespace MassTransit.Host.Activities
{
    using System;
    using Courier;
    using Hosting;


    public class ActivityService<TActivity, TArguments, TLog> :
        IBusServiceConfigurator
        where TActivity : class, Activity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly string _activityName;
        readonly CompensateActivityFactory<TActivity, TLog> _compensateActivityFactory;
        readonly int _compensateConsumerLimit;
        readonly string _compensateQueueName;
        readonly ExecuteActivityFactory<TActivity, TArguments> _executeActivityFactory;
        readonly int _executeConsumerLimit;
        readonly string _executeQueueName;

        public ActivityService(IConfigurationProvider configuration, IActivityQueueNameProvider queueNameProvider,
            ExecuteActivityFactory<TActivity, TArguments> executeActivityFactory, CompensateActivityFactory<TActivity, TLog> compensateActivityFactory)
        {
            _executeActivityFactory = executeActivityFactory;
            _compensateActivityFactory = compensateActivityFactory;

            _activityName = GetActivityName();

            _executeQueueName = queueNameProvider.GetExecuteActivityQueueName(_activityName);
            _executeConsumerLimit = GetExecuteConsumerLimit(configuration);

            _compensateQueueName = queueNameProvider.GetCompensateActivityQueueName(_activityName);
            _compensateConsumerLimit = GetCompensateConsumerLimit(configuration);
        }

        public virtual void Configure(IServiceConfigurator configurator)
        {
            var compensateAddress = CreateCompensateReceiveEndpoint(configurator);

            CreateExecuteReceiveEndpoint(configurator, compensateAddress);
        }

        static string GetActivityName()
        {
            var activityName = typeof(TActivity).Name;
            if (activityName.EndsWith("Service", StringComparison.OrdinalIgnoreCase))
                activityName = activityName.Substring(0, activityName.Length - "Service".Length);
            return activityName;
        }

        int GetExecuteConsumerLimit(IConfigurationProvider configurationProvider)
        {
            string settingName = $"{_activityName}ConsumerLimit";

            return configurationProvider.GetSetting(settingName, Environment.ProcessorCount);
        }

        int GetCompensateConsumerLimit(IConfigurationProvider configurationProvider)
        {
            string settingName = $"{_activityName}ConsumerLimit";

            return configurationProvider.GetSetting(settingName, Environment.ProcessorCount / 2);
        }

        protected virtual void CreateExecuteReceiveEndpoint(IServiceConfigurator configurator, Uri compensateAddress)
        {

            configurator.ReceiveEndpoint(_executeQueueName, _executeConsumerLimit, x =>
            {
                x.ExecuteActivityHost<TActivity, TArguments>(compensateAddress, _executeActivityFactory);
            });
        }

        protected virtual Uri CreateCompensateReceiveEndpoint(IServiceConfigurator configurator)
        {

            Uri inputAddress = null;

            configurator.ReceiveEndpoint(_compensateQueueName, _compensateConsumerLimit, x =>
            {
                inputAddress = x.InputAddress;
                x.CompensateActivityHost<TActivity, TLog>(_compensateActivityFactory);
            });

            return inputAddress;
        }
    }
}
