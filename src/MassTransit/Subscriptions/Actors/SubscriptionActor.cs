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
namespace MassTransit.Subscriptions.Actors
{
	using System;
	using System.Collections.Generic;
	using Magnum;
	using Messages;
	using Stact;
	using log4net;

	public class SubscriptionActor :
		Actor
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (SubscriptionActor));
		readonly UntypedChannel _output;
		HashSet<Guid> _ids;
		string _messageType;

		public SubscriptionActor(Inbox inbox, UntypedChannel output)
		{
			_output = output;
			_ids = new HashSet<Guid>();

			inbox.Receive<Message<InitializeSubscriptionActor>>(message =>
				{
					_messageType = message.Body.MessageName;

					Guid subscriptionId = Guid.Empty;

					inbox.Loop(loop =>
						{
							loop.Receive<Message<SubscribeTo>>(added =>
								{
									bool wasAdded = _ids.Add(added.Body.SubscriptionId);

									if (wasAdded && _ids.Count == 1)
									{
										subscriptionId = CombGuid.Generate();

										var add = new SubscriptionAdded(subscriptionId, _messageType, null);
										_output.Send(add);

										_log.DebugFormat("SubscribeTo: {0}, {1}", _messageType, subscriptionId);
									}

									loop.Continue();
								});

							loop.Receive<Message<UnsubscribeFrom>>(removed =>
								{
									_ids.Clear();

									var remove = new SubscriptionRemoved(subscriptionId, _messageType);
									_output.Send(remove);
									
									_log.DebugFormat("UnsubscribeFrom: {0}, {1}", _messageType, subscriptionId);

									subscriptionId = Guid.Empty;

									loop.Continue();
								});
						});
				});
		}
	}
}