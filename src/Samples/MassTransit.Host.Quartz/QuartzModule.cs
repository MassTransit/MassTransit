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
using IScheduler = Quartz.IScheduler;
using ISchedulerFactory = Quartz.ISchedulerFactory;
using StdSchedulerFactory = Quartz.Impl.StdSchedulerFactory;

namespace MassTransit.Host.Quartz
{
    using Autofac;
    using QuartzIntegration;
    using QuartzIntegration.Configuration;
    using Util;


    public class QuartzModule :
        Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(context => new SchedulerBusObserver(context.Resolve<IScheduler>(), context.Resolve<IConfigureMessageScheduler>().SchedulerAddress))
                .As<IBusObserver>();

            builder.RegisterType<ConfigureMessageScheduler>()
                .As<IConfigureMessageScheduler>()
                .SingleInstance();

            builder.Register(CreateScheduler)
                .As<IScheduler>()
                .SingleInstance();

            builder.RegisterType<ScheduleMessageConsumer>()
                .AsSelf();

            builder.RegisterType<CancelScheduledMessageConsumer>()
                .AsSelf();
        }

        static IScheduler CreateScheduler(IComponentContext context)
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();

            var scheduler = TaskUtil.Await(() => schedulerFactory.GetScheduler());

            return scheduler;
        }
    }
}