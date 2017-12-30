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
namespace MassTransit
{
    using System;
    using GreenPipes;
    using Quartz;
    using Quartz.Impl;
    using QuartzIntegration;
    using QuartzIntegration.Configuration;
    using Scheduling;
    using Util;


    public static class QuartzIntegrationExtensions
    {
        public static void UseInMemoryScheduler(this IBusFactoryConfigurator configurator, string queueName = "quartz")
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            var scheduler = TaskUtil.Await(() => schedulerFactory.GetScheduler());

            configurator.ReceiveEndpoint(queueName, e =>
            {
                var partitioner = configurator.CreatePartitioner(16);

                e.Consumer(() => new ScheduleMessageConsumer(scheduler), x =>
                    x.Message<ScheduleMessage>(m => m.UsePartitioner(partitioner, p => p.Message.CorrelationId)));
                e.Consumer(() => new CancelScheduledMessageConsumer(scheduler), x =>
                    x.Message<CancelScheduledMessage>(m => m.UsePartitioner(partitioner, p => p.Message.TokenId)));

                configurator.UseMessageScheduler(e.InputAddress);

                var specification = new SchedulerBusFactorySpecification(scheduler, e.InputAddress);
                configurator.AddBusFactorySpecification(specification);
            });
        }
    }
}