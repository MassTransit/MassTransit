// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Configuration;
    using Logging;
    using Quartz;
    using Quartz.Impl;
    using QuartzIntegration;
    using RabbitMqTransport;
    using Scheduling;
    using Topshelf;


    public class ScheduleMessageService :
        ServiceControl
    {
        readonly IConfigurationProvider _configurationProvider;
        readonly int _consumerLimit;
        readonly ILog _log = Logger.Get<ScheduleMessageService>();
        readonly string _queueName;
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
                Uri serviceBusUri = _configurationProvider.GetServiceBusUri();

                if (serviceBusUri.Scheme.Equals("rabbitmq", StringComparison.OrdinalIgnoreCase))
                {
                    _bus = Bus.Factory.CreateUsingRabbitMq(busConfig =>
                    {
                        IRabbitMqHost host = busConfig.Host(serviceBusUri, h => _configurationProvider.GetHostSettings(h));
                        busConfig.UseJsonSerializer();

                        busConfig.ReceiveEndpoint(host, _queueName, endpoint =>
                        {
                            endpoint.PrefetchCount = (ushort)_consumerLimit;

                            var partitioner = endpoint.CreatePartitioner(_consumerLimit);

                            endpoint.Consumer(() => new ScheduleMessageConsumer(_scheduler), x =>
                                x.ConfigureMessage<ScheduleMessage>(m => m.UsePartitioner(partitioner, p => p.Message.CorrelationId)));
                            endpoint.Consumer(() => new CancelScheduledMessageConsumer(_scheduler), x =>
                                x.ConfigureMessage<CancelScheduledMessage>(m => m.UsePartitioner(partitioner, p => p.Message.TokenId)));
                        });
                    });
                }

                _busHandle = _bus.Start();

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

            _busHandle?.Stop();

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