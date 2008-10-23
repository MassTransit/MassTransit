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
    using MassTransit.Host;
    using MassTransit.Host.Configurations;
    using Microsoft.Practices.ServiceLocation;

    public class SubscriptionManagerEnvironment :
		LocalSystemConfiguration
	{
		private readonly IApplicationLifeCycle _lifeCycle;

		public SubscriptionManagerEnvironment(IServiceLocator serviceLocator)
		{
			_lifeCycle = new SubscriptionManagerLifeCycle(serviceLocator);
		}

		public override string ServiceName
		{
			get { return "SubscriptionManagerGUI"; }
		}

		public override string DisplayName
		{
			get { return "Sample GUI Subscription Service"; }
		}

		public override string Description
		{
			get { return "Coordinates subscriptions between multiple systems"; }
		}

		public override IApplicationLifeCycle LifeCycle
		{
			get { return _lifeCycle; }
		}
	}
}