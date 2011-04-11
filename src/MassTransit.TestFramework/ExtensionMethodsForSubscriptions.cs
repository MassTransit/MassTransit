// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.TestFramework
{
	using System;
	using System.Threading;
	using Helpers;
	using Magnum.Extensions;
	using NUnit.Framework;
	using Pipeline;

	public static class ExtensionMethodsForSubscriptions
	{
		public static TimeSpan Timeout { get; set; }

		static ExtensionMethodsForSubscriptions()
		{
			Timeout = 8.Seconds();
		}

		public static void ShouldHaveSubscriptionFor<TMessage>(this IServiceBus bus)
		{
			DateTime giveUpAt = DateTime.Now + Timeout;

			while (DateTime.Now < giveUpAt)
			{
				var inspector = new EndpointSinkLocator(typeof (TMessage));

				bus.OutboundPipeline.Inspect(inspector);

				if (inspector.DestinationAddress != null)
					return;

				Thread.Sleep(10);
			}

			Assert.Fail("A subscription for " + typeof (TMessage).ToFriendlyName() + " was not found on " + bus.Endpoint.Uri);
		}

		public static void ShouldHaveSubscriptionFor<TMessage>(this IMessagePipeline pipeline)
		{
			DateTime giveUpAt = DateTime.Now + Timeout;

			while (DateTime.Now < giveUpAt)
			{
				var inspector = new EndpointSinkLocator(typeof (TMessage));

				pipeline.Inspect(inspector);

				if (inspector.DestinationAddress != null)
					return;

				Thread.Sleep(10);
			}

			Assert.Fail("A subscription for " + typeof (TMessage).ToFriendlyName() + " was not found on the pipeline");
		}
	}
}