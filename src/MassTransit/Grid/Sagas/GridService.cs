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
	using System.Security.Cryptography;
	using System.Text;
	using Magnum.StateMachine;
	using Messages;
	using Saga;

	public class GridService :
		SagaStateMachine<GridService>,
		ISaga
	{
		private static readonly MD5CryptoServiceProvider _cryptoProvider = new MD5CryptoServiceProvider();

		static GridService()
		{
			Define(() =>
				{
					Correlate(ServiceNodeAdded).By((saga, message) => saga.CorrelationId == message.ServiceId);

					Initially(
						When(ServiceNodeAdded)
							.Then((saga, message) =>
								{
									saga.ServiceName = message.ServiceName;

									saga.NotifyGridServiceAdded();
								})
							.TransitionTo(Known));

					During(Known,
						When(ServiceNodeAdded)
							.Then((saga, message) =>
								{
									// nothing really to do here I suppose, we already know about the service
								}));
				});
		}

		public GridService(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		protected GridService()
		{
		}

		public static State Initial { get; set; }
		public static State Known { get; set; }
		public static State Completed { get; set; }

		public static Event<GridServiceAddedToNode> ServiceNodeAdded { get; set; }

		public string ServiceName { get; set; }

		public Guid CorrelationId { get; set; }
		public IServiceBus Bus { get; set; }

		private void NotifyGridServiceAdded()
		{
			var message = new GridServiceAdded
				{
					ServiceId = CorrelationId,
					ServiceName = ServiceName,
				};

			Bus.Endpoint.Send(message);
		}

		public static Guid GenerateIdForType(Type type)
		{
			string key = type.AssemblyQualifiedName;

			var bytes = Encoding.UTF8.GetBytes(key);

			var hash = _cryptoProvider.ComputeHash(bytes);

			return new Guid(hash);
		}
	}
}