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
namespace MassTransit.Grid
{
	using System;
	using System.Linq;
	using System.Threading;
	using Internal;
	using log4net;
	using Magnum.DateTimeExtensions;
	using Messages;
	using Saga;
	using Sagas;

	public class ServiceGrid :
		IGridControl,
		Consumes<NotifyNewNodeAvailable>.All,
		Consumes<GridServiceAdded>.All
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (ServiceGrid));
		private readonly ISagaRepository<GridMessageNode> _messageNodeRepository;
		private IServiceBus _bus;
		private IServiceBus _controlBus;
		private DateTime _created;
		private volatile bool _disposed;
		private IEndpointFactory _endpointFactory;
		private ISagaRepository<GridNode> _nodeRepository;
		private ISagaRepository<GridServiceNode> _serviceNodeRepository;
		private ISagaRepository<GridService> _serviceRepository;
		private UnsubscribeAction _unsubscribeAction;

		public ServiceGrid(IEndpointFactory endpointFactory,
		                   ISagaRepository<GridNode> nodeRepository,
		                   ISagaRepository<GridService> serviceRepository,
		                   ISagaRepository<GridServiceNode> serviceNodeRepository,
		                   ISagaRepository<GridMessageNode> messageNodeRepository)
		{
			_endpointFactory = endpointFactory;
			_nodeRepository = nodeRepository;
			_serviceRepository = serviceRepository;
			_serviceNodeRepository = serviceNodeRepository;
			_messageNodeRepository = messageNodeRepository;
		}

		public Action WhenStarted { get; set; }
		public Uri ProposerUri { get; set; }

		public void Consume(GridServiceAdded message)
		{
			_log.Info("New Grid Service Detected: " + message.ServiceName);
		}

		public void Consume(NotifyNewNodeAvailable message)
		{
			if (message.ControlUri != _controlBus.Endpoint.Uri)
			{
				SendNodeOurServices(message);
			}
		}

		public Uri ControlUri { get; private set; }

		public Uri DataUri { get; private set; }

		public bool IsHealthy
		{
			get { throw new NotImplementedException(); }
		}

		public void Start(IServiceBus bus)
		{
			_bus = bus;
			_controlBus = bus.ControlBus;

			ControlUri = _controlBus.Endpoint.Uri;
			DataUri = _bus.Endpoint.Uri;

			_created = DateTime.UtcNow;

			_unsubscribeAction = _controlBus.Subscribe(this);

			SubscribeGridSagas();

			NotifyAvailable();

			WhenStarted();
		}

		public void Execute<T>(T command) where T : class
		{
			Execute(command, x => { });
		}

		public void Execute<T>(T command, Action<IOutboundMessage> action) where T : class
		{
			// begin saga for this command
			// publish message
			// get message from worker that is processing this command
			// get completion message for this command

			//			OutboundMessage.Set(action);

			Guid serviceId = GridService.GenerateIdForType(typeof (T));

			var node = _serviceNodeRepository
				.Where(x => x.ServiceId == serviceId)
				.Where(x => x.CurrentState == GridServiceNode.Active)
				.First();

			_endpointFactory
				.GetEndpoint(node.DataUri)
				.Send(command, action);
		}

		public void RegisterServiceInterceptor<TService>(GridServiceInterceptor<TService> interceptor)
			where TService : class
		{
			_unsubscribeAction += _bus.Subscribe(interceptor);

			Guid serviceId = GridService.GenerateIdForType(typeof (TService));

			var future = new SelectedFutureMessage<GridServiceAddedToNode>(x => x.ServiceId == serviceId &&
			                                                                    x.ControlUri == _controlBus.Endpoint.Uri);

			var unsubscribeFuture = _bus.ControlBus.Subscribe(future);
			try
			{
				var message = new AddGridServiceToNode
					{
						ControlUri = _controlBus.Endpoint.Uri,
						DataUri = _bus.Endpoint.Uri,
						ServiceId = serviceId,
						ServiceName = typeof (TService).ToMessageName(),
					};

				_nodeRepository
					.Where(x => x.CurrentState == GridNode.Available)
					.Select(x => _endpointFactory.GetEndpoint(x.ControlUri))
					.ToList()
					.Each(x => x.Send(message));

				if (!future.WaitUntilAvailable(10.Seconds()))
					_log.WarnFormat("Timeout waiting for interceptor to register: " + typeof (TService).Name);
			}
			finally
			{
				unsubscribeFuture();
			}
		}

		public RemoveActiveInterceptor AddActiveInterceptor(Guid serviceId, Guid correlationId, IGridServiceInteceptor interceptor)
		{
			return () => { };
		}

		public bool AcceptMessage(Guid serviceId, Guid correlationId)
		{
			bool accept = false;
			bool found = false;
			_messageNodeRepository.Find<int>(x => x.CorrelationId == correlationId, (saga, message) =>
				{
					accept = saga.CurrentState == GridMessageNode.Completed ||
					         IsAssignedToMessage(saga);

					found = true;
				})
				.Each(x => x(0));

			if (accept)
				return true;

			if (!found)
			{
				if (ProposerUri != null && ControlUri == ProposerUri)
				{
					ProposeMessageNodeToQuorum(serviceId, correlationId);
				}
			}

			return false;
		}

		public bool ConsumeMessage(Guid serviceId, Guid correlationId)
		{
			bool consume = false;
			_messageNodeRepository.Find<int>(x => x.CorrelationId == correlationId, (saga, message) => { consume = IsAssignedToMessage(saga); })
				.Each(x => x(0));

			return consume;
		}

		public void NotifyMessageComplete(Guid correlationId)
		{
			_controlBus.Publish(new MessageCompleted
				{
					CorrelationId = correlationId,
				});
		}

		public void Stop()
		{
			_unsubscribeAction();
			_unsubscribeAction = () => true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void ProposeMessageNodeToQuorum(Guid serviceId, Guid correlationId)
		{
			var nodes = _serviceNodeRepository
				.Where(x => x.ServiceId == serviceId)
				.ToList();

			var selectedNode = nodes.SelectNodeToUse();

			_log.InfoFormat("{0} sending proposal message for {1}", ControlUri, correlationId);

			ProposeMessageNode message = new ProposeMessageNode
				{
					BallotId = 1,
					ControlUri = selectedNode.ControlUri,
					DataUri = selectedNode.DataUri,
					CorrelationId = correlationId,
					Quorum = nodes.Select(x => x.ControlUri).ToList(),
				};

			nodes.Each(node => { _endpointFactory.GetEndpoint(node.ControlUri).Send(message, context => context.SetSourceAddress(ControlUri)); });

			WaitUntilMessageNodeIsAvailable(correlationId);
		}

		private void WaitUntilMessageNodeIsAvailable(Guid correlationId)
		{
			DateTime giveUpAt = DateTime.Now + 2.Seconds();
			ManualResetEvent neverSurrender = new ManualResetEvent(false);

			while (DateTime.Now < giveUpAt)
			{
				var count = _messageNodeRepository.Where(x => x.CorrelationId == correlationId).Count();
				if (count > 0)
					break;

				neverSurrender.WaitOne(30, true);
			}
		}
		public GridMessageNode GetMessageNode(Guid correlationId)
		{
			return _messageNodeRepository.Where(x => x.CorrelationId == correlationId).FirstOrDefault();
		}

		public bool IsAssignedToMessage(GridMessageNode messageNode)
		{
			if (messageNode.CurrentState != GridMessageNode.WaitingForCompletion)
				return false;

			_log.InfoFormat("{0} WAITER: {1}.{2}:{3}", ControlUri,
				messageNode.CorrelationId, messageNode.BallotId, messageNode.ControlUri);

			return messageNode.CurrentState == GridMessageNode.WaitingForCompletion &&
			       messageNode.DataUri == DataUri &&
			       messageNode.ControlUri == ControlUri;
		}

		public RemoveActiveInterceptor AddActiveInterceptor(Guid serviceId, IGridServiceInteceptor interceptor)
		{
			return () => { };
		}

		public virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			Stop();

			_nodeRepository = null;
			_serviceNodeRepository = null;
			_serviceRepository = null;
			_bus = null;
			_controlBus = null;
			_endpointFactory = null;

			_disposed = true;
		}

		private void SendNodeOurServices(NotifyNewNodeAvailable message)
		{
			_log.InfoFormat("{0} sending node available response to {1}", _controlBus.Endpoint.Uri, message.ControlUri);

			IEndpoint endpoint = _endpointFactory.GetEndpoint(message.ControlUri);
			endpoint.Send(NewNotifyNodeAvailableMessage());

			_serviceNodeRepository
				.Where(x => x.ControlUri == _controlBus.Endpoint.Uri)
				.Each(x => endpoint.Send(new AddGridServiceToNode
					{
						ControlUri = x.ControlUri,
						DataUri = x.DataUri,
						ServiceId = x.ServiceId,
						ServiceName = x.ServiceName,
					}, context => context.SendResponseTo(_controlBus.Endpoint)));
		}

		private void SubscribeGridSagas()
		{
			_unsubscribeAction += _controlBus.Subscribe<GridNode>();
			_unsubscribeAction += _controlBus.Subscribe<GridService>();
			_unsubscribeAction += _controlBus.Subscribe<GridServiceNode>();
			_unsubscribeAction += _controlBus.Subscribe<GridMessageNode>();
		}

		private void NotifyAvailable()
		{
			var future = new SelectedFutureMessage<NotifyNewNodeAvailable>(x => x.ControlUri == _controlBus.Endpoint.Uri);

			var unsubscribeFuture = _controlBus.Subscribe(future);
			try
			{
				_controlBus.Publish(NewNotifyNodeAvailableMessage());

				if (!future.WaitUntilAvailable(10.Seconds()))
					_log.Warn("Timeout waiting for node to become available: " + _controlBus.Endpoint.Uri);
			}
			finally
			{
				unsubscribeFuture();
			}
		}

		private NotifyNodeAvailable NewNotifyNodeAvailableMessage()
		{
			return new NotifyNodeAvailable
				{
					ControlUri = _controlBus.Endpoint.Uri,
					DataUri = _bus.Endpoint.Uri,
					LastUpdated = DateTime.UtcNow,
					Created = _created,
				};
		}

		~ServiceGrid()
		{
			Dispose(false);
		}
	}
}