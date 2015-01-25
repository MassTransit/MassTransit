// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.QuartzService
{
    using System;
    using System.Linq;
    using System.Threading;
    using Configuration;
    using Logging;
    using Quartz;
    using Quartz.Impl;
    using QuartzIntegration;
    using RabbitMqTransport.Configuration;
    using Topshelf;


    public class ScheduleMessageService :
        ServiceControl
    {
        readonly IConfigurationProvider _configurationProvider;
        readonly int _consumerLimit;
        readonly string _queueName;
        readonly ILog _log = Logger.Get<ScheduleMessageService>();
        readonly IScheduler _scheduler;
        IBusControl _bus;
        BusHandle _busHandle;

        public ScheduleMessageService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
            _queueName = configurationProvider.GetSetting("ControlQueueName");
            _consumerLimit = configurationProvider.GetSetting("ConsumerLimit", Math.Min(2, Environment.ProcessorCount));

            _scheduler = CreateScheduler();
        }

        public bool Start(HostControl hostControl)
        {
            try
            {
                var serviceBusUri = _configurationProvider.GetServiceBusUri("ignored");

                if (serviceBusUri.Scheme.Equals("rabbitmq", StringComparison.OrdinalIgnoreCase))
                {
                    _bus = Bus.Factory.CreateUsingRabbitMq(x =>
                    {
                        var host = x.Host(serviceBusUri, h => _configurationProvider.GetHostSettings(h));
                        x.UseJsonSerializer();

                        x.ReceiveEndpoint(host, _queueName, e =>
                        {
                            e.PrefetchCount = (ushort)_consumerLimit;

                            e.Consumer(() => new ScheduleMessageConsumer(_scheduler));
                            e.Consumer(() => new CancelScheduledMessageConsumer(_scheduler));
                        });
                    });
                }

                _busHandle = _bus.Start(new CancellationTokenSource(TimeSpan.FromMinutes(1)).Token)
                    .Result;

                _scheduler.JobFactory = new MassTransitJobFactory(_bus);

                _scheduler.Start();
            }
            catch (Exception)
            {
                _scheduler.Shutdown();
                throw;
            }

            return true;
        }


        public bool Stop(HostControl hostControl)
        {
            _scheduler.Standby();

            if (_busHandle != null)
                _busHandle.Stop();

            _scheduler.Shutdown();

            return true;
        }

        static IScheduler CreateScheduler()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();

            IScheduler scheduler = schedulerFactory.GetScheduler();

            return scheduler;
        }
    }
}