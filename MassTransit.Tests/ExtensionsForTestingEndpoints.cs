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
namespace MassTransit.Tests
{
	using System;
	using Magnum.Actors;
	using NUnit.Framework;

	public static class ExtensionsForTestingEndpoints
	{
		public static void ShouldContain<TMessage>(this IEndpoint endpoint)
			where TMessage : class
		{
			var future = new Future<TMessage>();

			endpoint.Receive(message =>
				{
					message.ShouldNotBeNull();

					message.ShouldBeSameType<TMessage>();

					TMessage tm = (TMessage) message;

					future.Complete(tm);

					return null;
				});

			future.IsAvailable().ShouldBeTrue(endpoint.Address + " should contain a message of type " + typeof (TMessage).Name);
		}

        public static void ShouldContain<TMessage>(this IEndpoint endpoint, TMessage expectedMessage)
            where TMessage : CorrelatedBy<Guid>
        {
            endpoint.ShouldContain(expectedMessage, TimeSpan.Zero);
        }

	    public static void ShouldContain<TMessage>(this IEndpoint endpoint, TMessage expectedMessage, TimeSpan timeout)
			where TMessage : CorrelatedBy<Guid>
		{
			var future = new Future<TMessage>();

	        endpoint.Receive(message =>
	            {
	                message.ShouldNotBeNull();

	                message.ShouldBeSameType<TMessage>();

	                TMessage tm = (TMessage) message;

	                Assert.AreEqual(expectedMessage.CorrelationId, tm.CorrelationId);

	                future.Complete(tm);

	                return null;
	            }, timeout);

			future.IsAvailable().ShouldBeTrue(endpoint.Address + " should contain a message of type " + typeof (TMessage).Name + " with correlation id " + expectedMessage.CorrelationId);
		}

		public static void ShouldNotContain<TMessage>(this IEndpoint endpoint)
			where TMessage : class
		{
			endpoint.Receive(message =>
				{
					message.ShouldNotBeNull();

					if (message.GetType() == typeof (TMessage))
					{
						Assert.Fail(endpoint.Address + " should not contain a message of type " + typeof (TMessage).Name);
					}

					return null;
				});
		}

		public static void ShouldNotContain<TMessage>(this IEndpoint endpoint, TMessage expectedMessage)
			where TMessage : CorrelatedBy<Guid>
		{
			endpoint.Receive(message =>
				{
					message.ShouldNotBeNull();

					if (message.GetType() != expectedMessage.GetType())
						return null;

					TMessage tm = (TMessage) message;

					if (tm.CorrelationId != expectedMessage.CorrelationId)
						return null;

					Assert.Fail(endpoint.Address + " should not contain a message of type " + typeof (TMessage).Name + " with correlation id " + expectedMessage.CorrelationId);

					return null;
				});
		}
	}
}