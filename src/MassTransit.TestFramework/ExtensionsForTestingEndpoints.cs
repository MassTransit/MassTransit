// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
	using Context;
	using Magnum.TestFramework;
	using MassTransit.Transports;
	using NUnit.Framework;
	using Serialization;

	public static class ExtensionsForTestingEndpoints
	{
		public static void ShouldContain<TMessage>(this IInboundTransport transport, IMessageSerializer serializer)
			where TMessage : class
		{
			var future = new Future<TMessage>();

			transport.Receive(context =>
				{
					context.ShouldNotBeNull();
					context.ShouldBeAnInstanceOf<IReceiveContext>();

					context.SetSerializer(serializer);

					IConsumeContext<TMessage> messageContext;
					if (context.TryGetContext(out messageContext))
					{
						if (!future.IsCompleted)
							future.Complete(messageContext.Message);
					}

					return null;
				}, TimeSpan.Zero);

			future.IsCompleted.ShouldBeTrue(transport.Address + " should contain a message of type " + typeof (TMessage).Name);
		}

		public static void ShouldContain<TMessage>(this IEndpoint endpoint)
			where TMessage : class
		{
			var future = new Future<TMessage>();

			endpoint.Receive(context =>
				{
					context.ShouldNotBeNull();
					context.ShouldBeAnInstanceOf<IReceiveContext>();

					IConsumeContext<TMessage> messageContext;
					if (context.TryGetContext(out messageContext))
					{
						if (!future.IsCompleted)
							future.Complete(messageContext.Message);
					}

					return null;
				}, TimeSpan.Zero);

			future.IsCompleted.ShouldBeTrue(endpoint.Address + " should contain a message of type " + typeof (TMessage).Name);
		}

		public static void ShouldContain<TMessage>(this IEndpoint endpoint, TMessage expectedMessage)
			where TMessage : class, CorrelatedBy<Guid>
		{
			endpoint.ShouldContain(expectedMessage, TimeSpan.Zero);
		}

		public static void ShouldContain<TMessage>(this IEndpoint endpoint, TMessage expectedMessage, TimeSpan timeout)
			where TMessage : class, CorrelatedBy<Guid>
		{
			var future = new Future<TMessage>();

			endpoint.Receive(context =>
				{
					context.ShouldNotBeNull();
					context.ShouldBeAnInstanceOf<IReceiveContext>();

					IConsumeContext<TMessage> messageContext;
					if (context.TryGetContext(out messageContext))
					{
						if (messageContext.Message.CorrelationId == expectedMessage.CorrelationId && !future.IsCompleted)
							future.Complete(messageContext.Message);
					}

					return null;
				}, timeout);

			future.IsCompleted.ShouldBeTrue(endpoint.Address + " should contain a message of type " + typeof (TMessage).Name +
			                                " with correlation id " + expectedMessage.CorrelationId);
		}

		public static void ShouldNotContain<TMessage>(this IEndpoint endpoint)
			where TMessage : class
		{
			endpoint.Receive(context =>
				{
					context.ShouldNotBeNull();
					context.ShouldBeAnInstanceOf<IReceiveContext>();

					IConsumeContext<TMessage> messageContext;
					if (context.TryGetContext(out messageContext))
					{
						Assert.Fail(endpoint.Address + " should not contain a message of type " + typeof (TMessage).Name);
					}

					return null;
				}, TimeSpan.Zero);
		}

		public static void ShouldNotContain<TMessage>(this IEndpoint endpoint, TMessage expectedMessage)
			where TMessage : class, CorrelatedBy<Guid>
		{
			endpoint.Receive(context =>
				{
					context.ShouldNotBeNull();
					context.ShouldBeAnInstanceOf<IReceiveContext>();

					IConsumeContext<TMessage> messageContext;
					if (context.TryGetContext(out messageContext))
					{
						if (messageContext.Message.CorrelationId != expectedMessage.CorrelationId)
							return null;

						Assert.Fail(endpoint.Address + " should not contain a message of type " + typeof (TMessage).Name +
						            " with correlation id " + expectedMessage.CorrelationId);
					}

					return null;
				}, TimeSpan.Zero);
		}
	}
}