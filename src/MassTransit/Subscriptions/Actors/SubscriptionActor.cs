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
	using Magnum.Extensions;
	using Messages;
	using Services.Subscriptions.Messages;
	using Stact;


	public class MessageTypeToEndpointActor :
		Actor
	{
		public MessageTypeToEndpointActor(Inbox inbox)
		{
			inbox.Receive<Request<InitializeEndpointMessageSinkActor>>(init =>
				{
					var owner = init.ResponseChannel;

					var subscribers = new HashSet<UntypedChannel>();

					inbox.Loop(loop =>
						{
							loop.Receive<Request<SubscribeTo>>(request =>
								{
									bool added = subscribers.Add(request.ResponseChannel);
									if(added && subscribers.Count == 1)
									{
										// bind message sink to output pipeline
									}

									loop.Continue();
								});

							loop.Receive<Request<UnsubscribeFrom>>(request =>
								{
									bool removed = subscribers.Remove(request.ResponseChannel);
									if(removed && subscribers.Count == 0)
									{
										// unbind message sink from outbound pipeline
										
									}

									loop.Continue();
								});
						});
				});
		}
	}

	public class InitializeEndpointMessageSinkActor
	{
	}


	public class SubscriptionActor :
		Actor
	{
		readonly Fiber _fiber;
		readonly Scheduler _scheduler;
		readonly UntypedChannel _output;
		string _messageType;
		HashSet<Guid> _ids;
		TimeSpan _unsubscribeTimeout;
		ScheduledOperation _unsubscribeScheduledAction;

		public SubscriptionActor(Fiber fiber, Scheduler scheduler, Inbox inbox, UntypedChannel output)
		{
			_fiber = fiber;
			_scheduler = scheduler;
			_output = output;
			_ids = new HashSet<Guid>();
			_unsubscribeTimeout = 30.Seconds();

			inbox.Receive<Message<InitializeSubscriptionActor>>(message =>
				{
					_messageType = message.Body.MessageName;

					Guid subscriptionId = Guid.Empty;

					inbox.Loop(loop =>
						{
							loop.Receive<Message<SubscribeTo>>(added =>
								{
									bool wasAdded = _ids.Add(added.Body.SubscriptionId);

									if(wasAdded && _ids.Count == 1)
									{
										if(_unsubscribeScheduledAction != null)
										{
											_unsubscribeScheduledAction.Cancel();
											_unsubscribeScheduledAction = null;
										}
										else
										{
											subscriptionId = CombGuid.Generate();

											var add = new SubscriptionAdded()
												{
													SubscriptionId = subscriptionId,
													MessageName = _messageType,
												};

											_output.Send(add);
										}
									}

									loop.Continue();
								});

							loop.Receive<Message<UnsubscribeFrom>>(removed =>
								{
									bool wasRemoved = _ids.Remove(removed.Body.SubscriptionId);

									if(wasRemoved && _ids.Count == 0)
									{
										var remove = new SubscriptionRemoved()
										{
											SubscriptionId = subscriptionId,
											MessageName = _messageType,
										};

										_output.Send(remove);
										subscriptionId = Guid.Empty;

//										// we have no more subscriptions
//										_unsubscribeScheduledAction = _scheduler.Schedule(_unsubscribeTimeout, _fiber, () =>
//											{
//											});
									}

									loop.Continue();
							});

						});
				});
		}
	}

	public class RemoteEndpointActor :
		Actor
	{
		Fiber _fiber;
		Scheduler _scheduler;
		Inbox _inbox;
		IDictionary<Type, ActorInstance> _subscriptionTypes;
		ActorFactory<SubscriptionActor> _factory;
		Uri _controlUri;

		public RemoteEndpointActor(Fiber fiber, Scheduler scheduler, Inbox inbox, UntypedChannel parent)
		{
			_fiber = fiber;
			_scheduler = scheduler;
			_inbox = inbox;
			_subscriptionTypes = new Dictionary<Type, ActorInstance>();

			_factory = ActorFactory.Create((f, s, i) => new SubscriptionActor(f, s, i, _inbox));

			inbox.Receive<Message<InitializeRemoteEndpointActor>>(message =>
				{
					_controlUri = message.Body.ControlUri;

					inbox.Loop(loop =>
						{
							loop.Receive<Message<RemoveSubscriptionClient>>(m =>
								{

									
								});
						});

				});
		}

	}

	public class InitializeRemoteEndpointActor
	{
		public Uri ControlUri { get; set; }
	}
}
