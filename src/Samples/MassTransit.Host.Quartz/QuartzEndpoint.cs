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
namespace MassTransit.Host.Quartz
{
    using System;
    using GreenPipes;
    using Hosting;
    using QuartzIntegration;
    using Scheduling;


    public class QuartzEndpoint :
        IEndpointSpecification
    {
        readonly IConsumerFactory<CancelScheduledMessageConsumer> _cancelScheduledMessageConsumerFactory;
        readonly IConfigureMessageScheduler _configureMessageScheduler;
        readonly IConsumerFactory<ScheduleMessageConsumer> _scheduleMessageConsumerFactory;

        public QuartzEndpoint(IConsumerFactory<ScheduleMessageConsumer> scheduleMessageConsumerFactory,
            IConsumerFactory<CancelScheduledMessageConsumer> cancelScheduledMessageConsumerFactory, IConfigureMessageScheduler configureMessageScheduler)
        {
            _scheduleMessageConsumerFactory = scheduleMessageConsumerFactory;
            _cancelScheduledMessageConsumerFactory = cancelScheduledMessageConsumerFactory;
            _configureMessageScheduler = configureMessageScheduler;
        }

        public string QueueName => "masstransit_quartz_scheduler";

        public int ConsumerLimit => Environment.ProcessorCount;

        public void Configure(IReceiveEndpointConfigurator configurator)
        {
            var partitioner = configurator.CreatePartitioner(ConsumerLimit);

            configurator.Consumer(_scheduleMessageConsumerFactory, x =>
                x.Message<ScheduleMessage>(m => m.UsePartitioner(partitioner, p => p.Message.CorrelationId)));

            configurator.Consumer(_cancelScheduledMessageConsumerFactory, x =>
                x.Message<CancelScheduledMessage>(m => m.UsePartitioner(partitioner, p => p.Message.TokenId)));

            _configureMessageScheduler.SchedulerAddress = configurator.InputAddress;
        }
    }
}