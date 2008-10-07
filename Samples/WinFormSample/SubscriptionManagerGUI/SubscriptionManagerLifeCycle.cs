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
	using MassTransit.Host.Actions;
	using MassTransit.Host.LifeCycles;
	using MassTransit.ServiceBus;
	using MassTransit.ServiceBus.HealthMonitoring;
	using MassTransit.ServiceBus.Subscriptions;
	using MassTransit.ServiceBus.Timeout;

	public class SubscriptionManagerLifeCycle :
		HostedLifeCycle
	{
		public SubscriptionManagerLifeCycle(string xmlFile) : base(xmlFile)
		{
		}

		public override NamedAction DefaultAction
		{
			get { return NamedAction.Gui; }
		}

		public override void Start()
		{
			Container.AddComponentLifeStyle("followerRepository", typeof (FollowerRepository), LifestyleType.Singleton);

			Container.AddComponent<ISubscriptionRepository, InMemorySubscriptionRepository>();
			Container.AddComponent<IHostedService, SubscriptionService>();

		    Container.AddComponent<IHealthCache, LocalHealthCache>();
		    Container.AddComponent<IHeartbeatTimer, InMemoryHeartbeatTimer>();
            Container.AddComponent<IHostedService, HealthService>();

			Container.AddComponent<ITimeoutRepository, InMemoryTimeoutRepository>();
			Container.AddComponent<IHostedService, TimeoutService>();

			Container.AddComponent<Form, SubscriptionManagerForm>();
		}

		public override void Stop()
		{
		}
	}
}