// Copyright 2007-2008 The Apache Software Foundation.
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
namespace SubscriptionManagerGUI
{
    using System.Windows.Forms;
    using Castle.Core;
    using log4net;
	using MassTransit.Host;
    using MassTransit.Host.Configurations;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Services.HealthMonitoring;
    using MassTransit.ServiceBus.Services.Timeout;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.ServiceBus.Subscriptions.ServerHandlers;
    using MassTransit.WindsorIntegration;
	using Microsoft.Practices.ServiceLocation;

    internal static class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		private static void Main(string[] args)
		{
			_log.Info("SubscriptionManagerGUI Loading...");

		    var container = new DefaultMassTransitContainer("SubscriptionManager.Castle.xml");
            container.AddComponentLifeStyle("followerRepository", typeof(FollowerRepository), LifestyleType.Singleton);

            container.AddComponent<ISubscriptionRepository, InMemorySubscriptionRepository>();
            
            container.AddComponent<IHostedService, SubscriptionService>();
            container.AddComponent<AddSubscriptionHandler>();
            container.AddComponent<RemoveSubscriptionHandler>();
            container.AddComponent<CancelUpdatesHandler>();
            container.AddComponent<CacheUpdateRequestHandler>();


            container.AddComponent<IHealthCache, LocalHealthCache>();
            container.AddComponent<IHeartbeatTimer, InMemoryHeartbeatTimer>();
            container.AddComponent<IHostedService, HealthService>();
            container.AddComponent<HeartbeatMonitor>();
            container.AddComponent<Investigator>();
            container.AddComponent<Reporter>();

            container.AddComponent<ITimeoutRepository, InMemoryTimeoutRepository>();
            container.AddComponent<IHostedService, TimeoutService>();
            container.AddComponent<ScheduleTimeoutConsumer>();
            container.AddComponent<CancelTimeoutConsumer>();

            container.AddComponent<Form, SubscriptionManagerForm>();

		    var credentials = Credentials.LocalSystem;
		    var settings = WinServiceSettings.Custom(
		        "SubscriptionManagerGUI",
		        "Sample GUI Subscription Service",
		        "Coordinates subscriptions between multiple systems",
		        KnownServiceNames.Msmq);
		    var lifecycle = new SubscriptionManagerLifeCycle(ServiceLocator.Current);


			Runner.Run(credentials, settings, lifecycle, args);
		}
	}
}