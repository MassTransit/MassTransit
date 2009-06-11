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
namespace MassTransit.Tests.Grid
{
	using System;
	using System.Diagnostics;
	using log4net;
	using MassTransit.Saga;

	public class ServiceGrid :
		Grid,
		Consumes<NotifyNewNodeAvailable>.All
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (ServiceGrid));
		private readonly IEndpointFactory _endpointFactory;
		private readonly ISagaRepository<NodeState> _nodeStateRepository;
		private IServiceBus _bus;
		private IServiceBus _controlBus;
		private UnsubscribeAction _unsubscribeAction;
		private DateTime _created;


		public ServiceGrid(IEndpointFactory endpointFactory, ISagaRepository<NodeState> nodeStateRepository)
		{
			_endpointFactory = endpointFactory;
			_nodeStateRepository = nodeStateRepository;
		}


		public bool IsHealthy
		{
			get { throw new NotImplementedException(); }
		}

		public void Start(IServiceBus bus)
		{
			_bus = bus;
			_controlBus = bus.ControlBus;

			_created = DateTime.UtcNow;


			_unsubscribeAction = _controlBus.Subscribe(this);
			_unsubscribeAction += _controlBus.Subscribe<NodeState>();


			NotifyAvailable();
		}

		private void NotifyAvailable()
		{
			_controlBus.Publish(NewNotifyNodeAvailableMessage());
		}

		private NotifyNodeAvailable NewNotifyNodeAvailableMessage()
		{
			return new NotifyNodeAvailable
				{
					ControlEndpointUri = _controlBus.Endpoint.Uri,
					DataEndpointUri = _bus.Endpoint.Uri,
					LastUpdated = DateTime.UtcNow,
					Created = _created,
				};
		}

		public void Execute<T>(T command)
		{
			// begin saga for this command

			// publish message

			// get message from worker that is processing this command

			// get completion message for this command
		}

		public void ConfigureService<T>(Action<T> action) 
			where T : class
		{
			_unsubscribeAction += _bus.Subscribe<T>();
		}

		public void Stop()
		{
			_unsubscribeAction();
		}

		public void Consume(NotifyNewNodeAvailable message)
		{
			if (message.ControlEndpointUri != _controlBus.Endpoint.Uri)
			{
				_log.DebugFormat("{0} sending node available response to {1}", _controlBus.Endpoint.Uri, message.ControlEndpointUri);

				_endpointFactory.GetEndpoint(message.ControlEndpointUri).Send(NewNotifyNodeAvailableMessage());
			}
		}
	}
}