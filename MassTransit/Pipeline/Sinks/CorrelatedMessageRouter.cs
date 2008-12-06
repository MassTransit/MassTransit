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
namespace MassTransit.Pipeline.Sinks
{
	using System;
	using System.Collections.Generic;
	using Configuration;

	/// <summary>
	/// Splits a message path based on the correlation information in the message
	/// </summary>
	/// <typeparam name="TMessage">The type of the message to be routed</typeparam>
	/// <typeparam name="TKey">They key type for the message</typeparam>
	public class CorrelatedMessageRouter<TMessage, TKey> :
		MessageRouterBase<TMessage, TKey>
		where TMessage : class, CorrelatedBy<TKey>
	{
		public override IEnumerable<Consumes<TMessage>.All> Enumerate(TMessage message)
		{
			return EnumerateSinks(message, message.CorrelationId);
		}

		public Func<bool> Connect(TKey correlationId, IMessageSink<TMessage> sink)
		{
			IMessageSink<TMessage> keySink = null;

			if (_sinks.ReadLock(x => x.TryGetValue(correlationId, out keySink)) == false)
			{
				// we need to add the sink if it doesn't exist
				_sinks.WriteLock(x =>
					{
						if (x.TryGetValue(correlationId, out keySink) == false)
						{
							MessageRouter<TMessage> keyRouter = new MessageRouter<TMessage>();

							x.Add(correlationId, keyRouter);

							keySink = keyRouter;
						}
					});
			}

			MessageRouterConfigurator configurator = MessageRouterConfigurator.For(keySink);

			var router = configurator.FindOrCreate<TMessage>();

			return router.Connect(sink);
		}
	}
}