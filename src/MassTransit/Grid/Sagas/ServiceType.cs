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
namespace MassTransit.Grid.Sagas
{
	using System;
	using Magnum.StateMachine;
	using Messages;
	using Saga;
	using Util;

	public class ServiceType :
		SagaStateMachine<ServiceType>,
		ISaga
	{
		static ServiceType()
		{
			Define(() =>
				{
					Correlate(ServiceNodeAdded).By((saga, message) => saga.CorrelationId == message.ServiceId);

					Initially(
						When(ServiceNodeAdded)
							.Then((saga, message) =>
								{
									saga.ServiceName = message.ServiceName;

									var notification = new ServiceTypeAdded
										{
											ServiceId = saga.CorrelationId,
											ServiceName = saga.ServiceName,
										};

									saga.Bus.Endpoint.Send(notification);
								})
							.TransitionTo(Active));

					During(Active,
						When(ServiceNodeAdded)
							.Then((saga, message) =>
								{
									// nothing really to do here I suppose, we already know about the service
								}));
				});
		}

		public ServiceType(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		protected ServiceType()
		{
		}

		public static State Initial { get; set; }
		public static State Active { get; set; }
		public static State Completed { get; set; }

		public static Event<ServiceNodeAdded> ServiceNodeAdded { get; set; }

		public string ServiceName { get; set; }

		[Indexed]
		public Guid CorrelationId { get; set; }

		public IServiceBus Bus { get; set; }
	}
}