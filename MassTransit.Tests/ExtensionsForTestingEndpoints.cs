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
	using Magnum.Common.DateTimeExtensions;
	using NUnit.Framework;

	public static class ExtensionsForTestingEndpoints
	{
		public static void ShouldContain<TMessage>(this IEndpoint endpoint, TMessage expectedMessage)
			where TMessage : CorrelatedBy<Guid>
		{
			foreach (var selector in endpoint.SelectiveReceive(5.Seconds()))
			{
				object message = selector.DeserializeMessage();
				Assert.IsNotNull(message);

				Assert.IsInstanceOfType(expectedMessage.GetType(), message);

				TMessage pingMessage = (TMessage) message;

				Assert.AreEqual(expectedMessage.CorrelationId, pingMessage.CorrelationId);

				return;
			}

			Assert.Fail(string.Format("Message ({0}) not found in queue", typeof (TMessage).FullName));
		}

		public static void ShouldNotContain<TMessage>(this IEndpoint endpoint, TMessage expectedMessage)
			where TMessage : CorrelatedBy<Guid>
		{
			foreach (var selector in endpoint.SelectiveReceive(2.Seconds()))
			{
				object message = selector.DeserializeMessage();
				Assert.IsNotNull(message);

				if (message.GetType() != expectedMessage.GetType())
					continue;

				TMessage pingMessage = (TMessage) message;

				if (expectedMessage.CorrelationId != pingMessage.CorrelationId)
					continue;

				Assert.Fail(string.Format("The message ({0}) was found in the queue", typeof (TMessage).FullName));
			}
		}
	}
}