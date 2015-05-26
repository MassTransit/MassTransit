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
namespace RapidTransit
{
    using System;
    using Configuration;
    using MassTransit.Courier;
    using MassTransit.Logging;
    using Topshelf;


    public class ActivityService<TActivity, TArguments, TLog> :
        ServiceControl
        where TActivity : Activity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly string _activityName;
        readonly CompensateActivityFactory<TLog> _compensateActivityFactory;
        readonly int _compensateConsumerLimit;
        readonly string _compensateQueueName;
        readonly ExecuteActivityFactory<TArguments> _executeActivityFactory;
        readonly int _executeConsumerLimit;
        readonly string _executeQueueName;
        readonly ILog _log;
        readonly IServiceConfigurator _serviceConfigurator;

        public ActivityService(IConfigurationProvider configuration, IServiceConfigurator serviceConfigurator,
            IActivityQueueNameProvider queueNameProvider,
            ExecuteActivityFactory<TArguments> executeActivityFactory,
            CompensateActivityFactory<TLog> compensateActivityFactory)
        {
            _log = Logger.Get(GetType());

            _serviceConfigurator = serviceConfigurator;
            IActivityQueueNameProvider activityUriProvider1 = queueNameProvider;
            _executeActivityFactory = executeActivityFactory;
            _compensateActivityFactory = compensateActivityFactory;

            _activityName = GetActivityName();

            _executeQueueName = activityUriProvider1.GetExecuteActivityQueueName(_activityName);
            _executeConsumerLimit = GetExecuteConsumerLimit(configuration);

            _compensateQueueName = activityUriProvider1.GetCompensateActivityQueueName(_activityName);
            _compensateConsumerLimit = GetCompensateConsumerLimit(configuration);
        }

        public virtual bool Start(HostControl hostControl)
        {
            return true;
        }

        public virtual bool Stop(HostControl hostControl)
        {
            return true;
        }

        string GetActivityName()
        {
            string activityName = typeof(TActivity).Name;
            if (activityName.EndsWith("Service", StringComparison.OrdinalIgnoreCase))
                activityName = activityName.Substring(0, activityName.Length - "Service".Length);
            return activityName;
        }

        int GetExecuteConsumerLimit(IConfigurationProvider configurationProvider)
        {
            string settingName = string.Format("{0}ConsumerLimit", _activityName);

            return configurationProvider.GetSetting(settingName, Environment.ProcessorCount);
        }

        int GetCompensateConsumerLimit(IConfigurationProvider configurationProvider)
        {
            string settingName = string.Format("{0}ConsumerLimit", _activityName);

            return configurationProvider.GetSetting(settingName, Environment.ProcessorCount / 2);
        }

        protected virtual void CreateExecuteServiceBus()
        {
            _log.InfoFormat("Creating Execute {0} Receive Endpoint", _activityName);

            Uri compensateAddress = null; // compensateServiceBus.Endpoint.Address;

            _serviceConfigurator.Configure(_executeQueueName, _executeConsumerLimit, x =>
            {
                x.ExecuteActivityHost<TActivity, TArguments>(compensateAddress, _executeActivityFactory);
            });
        }

        protected virtual void CreateCompensateServiceBus()
        {
            _log.InfoFormat("Creating Compensate {0} Receive Endpoint", _activityName);

            _serviceConfigurator.Configure(_compensateQueueName, _compensateConsumerLimit, x =>
            {
                x.CompensateActivityHost<TActivity, TLog>(_compensateActivityFactory);
            });
        }
    }
}