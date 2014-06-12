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
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using Helpers;
	using Logging;
	using Testing;
	using Magnum.Extensions;
	using NUnit.Framework;
	using Pipeline;
	using Pipeline.Inspectors;
	using Saga;
	using Saga.Pipeline;
	using Util;

	public static class ExtensionMethodsForSubscriptions
	{
		static readonly ILog _log = Logger.Get(typeof (ExtensionMethodsForSubscriptions));

		public static TimeSpan Timeout { get; set; }

		static ExtensionMethodsForSubscriptions()
		{
			Timeout = 12.Seconds();
		}

		public static void ShouldHaveRemoteSubscriptionFor<TMessage>(this IServiceBus bus)
		{
			DateTime giveUpAt = DateTime.Now + Timeout;

			while (DateTime.Now < giveUpAt)
			{
				var inspector = new EndpointSinkLocator(typeof (TMessage));

				bus.OutboundPipeline.Inspect(inspector);

				if (inspector.DestinationAddress != null)
					return;

				Thread.Sleep(20);
			}

			PipelineViewer.Trace(bus.OutboundPipeline, text => _log.ErrorFormat("Pipeline Inspection Result: " + text));

			Assert.Fail("A subscription for " + typeof (TMessage).ToFriendlyName() + " was not found on " + bus.Endpoint.Address.Uri);
		}

		public static IEnumerable<IPipelineSink<TMessage>> ShouldHaveSubscriptionFor<TMessage>(this IServiceBus bus)
			where TMessage : class
		{
			return bus.OutboundPipeline.ShouldHaveSubscriptionFor<TMessage>();
		}

		public static IEnumerable<IPipelineSink<TMessage>> ShouldHaveSagaSubscriptionFor<TSaga, TMessage>(this IServiceBus bus, Type policyType)
			where TMessage : class 
			where TSaga : class, ISaga
		{
			return bus.OutboundPipeline.ShouldHaveSubscriptionFor<TMessage>()
				.Where(sink => sink.GetType().Implements(typeof(SagaMessageSinkBase<TSaga,TMessage>)))
				.Where(sink => ((ISagaMessageSink<TSaga,TMessage>)sink).Policy.GetType().GetGenericTypeDefinition() == policyType);
		}

		public static IEnumerable<IPipelineSink<TMessage>> ShouldHaveSubscriptionFor<TMessage>(this IOutboundMessagePipeline pipeline) 
			where TMessage : class
		{
			DateTime giveUpAt = DateTime.Now + Timeout;

			while (DateTime.Now < giveUpAt)
			{
				var inspector = new PipelineSinkLocator<TMessage>();

				pipeline.Inspect(inspector);

				if (inspector.Result.Any())
					return inspector.Result;

				Thread.Sleep(20);
			}

			Assert.Fail("A subscription for " + typeof (TMessage).ToFriendlyName() + " was not found on the pipeline");

			return null;
		}

		public static IEnumerable<IPipelineSink<TMessage>> ShouldHaveSubscriptionFor<TMessage>(this IInboundMessagePipeline pipeline) 
			where TMessage : class
		{
			DateTime giveUpAt = DateTime.Now + Timeout;

			while (DateTime.Now < giveUpAt)
			{
				var inspector = new PipelineSinkLocator<TMessage>();

				pipeline.Inspect(inspector);

				if (inspector.Result.Any())
					return inspector.Result;

				Thread.Sleep(20);
			}

			Assert.Fail("A subscription for " + typeof (TMessage).ToFriendlyName() + " was not found on the pipeline");

			return null;
		}

		public static void ShouldNotHaveSubscriptionFor<TMessage>(this IServiceBus bus)
		{
			DateTime giveUpAt = DateTime.Now + Timeout;

			while (DateTime.Now < giveUpAt)
			{
				var inspector = new EndpointSinkLocator(typeof (TMessage));

				bus.OutboundPipeline.Inspect(inspector);

				if (inspector.DestinationAddress == null)
					return;

				Thread.Sleep(20);
			}

			Assert.Fail("A subscription for " + typeof (TMessage).ToFriendlyName() + " was found on " + bus.Endpoint.Address.Uri);
		}
	}
}