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
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using log4net;
	using Magnum.DateTimeExtensions;
	using Messages;
	using Saga;
	using Sagas;

	public class ServiceGrid :
		IGridControl,
		Consumes<NotifyNewNodeAvailable>.All,
		Consumes<ServiceTypeAdded>.All,
		Consumes<NullMessage>.All
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (ServiceGrid));
		private readonly ISagaRepository<ServiceMessage> _messageNodeRepository;
		private IServiceBus _bus;
		private IServiceBus _controlBus;
		private DateTime _created;
		private volatile bool _disposed;
		private IEndpointFactory _endpointFactory;
		private ISagaRepository<Node> _nodeRepository;
		private ISagaRepository<ServiceNode> _serviceNodeRepository;
		private UnsubscribeAction _unsubscribeAction;

		public ServiceGrid(IEndpointFactory endpointFactory, ISagaRepository<Node> nodeRepository, ISagaRepository<ServiceNode> serviceNodeRepository, ISagaRepository<ServiceMessage> messageNodeRepository)
		{
			_endpointFactory = endpointFactory;
			_nodeRepository = nodeRepository;
			_serviceNodeRepository = serviceNodeRepository;
			_messageNodeRepository = messageNodeRepository;
		}

		public Action WhenStarted { get; set; }
		public Uri ProposerUri { get; set; }

		public void Consume(ServiceTypeAdded message)
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

		public void Consume(NullMessage message)
		{
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

		public void RegisterServiceInterceptor<TService>(GridServiceInterceptor<TService> interceptor)
			where TService : class
		{
			_unsubscribeAction += _bus.Subscribe(interceptor);

			Guid serviceId = typeof (TService).ToServiceTypeId();

			var future = new SelectedFutureMessage<ServiceNodeAdded>(x => x.ServiceId == serviceId &&
			                                                                    x.ControlUri == _controlBus.Endpoint.Uri);

			UnsubscribeAction unsubscribeFuture = _bus.ControlBus.Subscribe(future);
			try
			{
				var message = new AddServiceNode
					{
						ControlUri = _controlBus.Endpoint.Uri,
						DataUri = _bus.Endpoint.Uri,
						ServiceId = serviceId,
						ServiceName = typeof (TService).ToMessageName(),
					};

				_nodeRepository
					.Where(x => x.CurrentState == Node.Available)
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
			ServiceMessage node = _messageNodeRepository
				.Where(x => x.CorrelationId == correlationId)
				.SingleOrDefault();

			if (node == null)
			{
				if (ProposerUri != null && ControlUri == ProposerUri)
				{
					ProposeMessageNodeToQuorum(serviceId, correlationId);
				}

				return false;
			}

			bool accept = (node.CurrentState == ServiceMessage.WaitingForReceive ||
			               (node.CurrentState == ServiceMessage.WaitingForCompletion &&
			                node.DataUri == DataUri &&
			                node.ControlUri == ControlUri
			               ));

			return accept;
		}

		public bool ConsumeMessage(Guid serviceId, Guid correlationId)
		{
			return _messageNodeRepository
			       	.Where(x => x.CorrelationId == correlationId &&
			       	            x.CurrentState == ServiceMessage.WaitingForCompletion &&
			       	            x.DataUri == DataUri &&
			       	            x.ControlUri == ControlUri
			       	).Count() > 0;
		}

		public void NotifyMessageComplete(Guid correlationId)
		{
			_controlBus.Publish(new ServiceMessageCompleted
				{
					CorrelationId = correlationId,
				});

			_controlBus.Endpoint.Send(new ServiceMessageReceived {CorrelationId = correlationId});
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
			List<ServiceNode> nodes = _serviceNodeRepository
				.Where(x => x.ServiceId == serviceId)
				.ToList();

			ServiceNode selectedNode = nodes.SelectNodeToUse();

			if (_log.IsDebugEnabled)
				_log.DebugFormat("{0} sending proposal message for {1} recommending {2}", ControlUri, correlationId, selectedNode.ControlUri);

			List<Uri> quorum = nodes.Select(x => x.ControlUri).ToList();
			if(quorum.Count == 1)
			{
				SendMessageToNodes<ServiceMessageAgreed>(nodes, correlationId, 1, selectedNode.ControlUri, selectedNode.DataUri, quorum);
			}
			else
			{
				SendMessageToNodes<ProposeServiceMessage>(nodes, correlationId, 1, selectedNode.ControlUri, selectedNode.DataUri, quorum);
			}

			if (quorum.Contains(ControlUri))
			{
				WaitUntilMessageNodeIsAvailable(correlationId);
			}
		}

		private void SendMessageToNodes<T>(IEnumerable<ServiceNode> nodes, Guid correlationId, long ballotId, Uri controlUri, Uri dataUri, IList<Uri> quorum)
			where T : AbstractServiceMessageMessage, new()
		{
			var message = new T
			{
				BallotId = ballotId,
				ControlUri = controlUri,
				DataUri = dataUri,
				CorrelationId = correlationId,
				Quorum = quorum,
			};

			nodes.Each(node => { _endpointFactory.GetEndpoint(node.ControlUri).Send(message, context => context.SetSourceAddress(ControlUri)); });
		}

		public ServiceMessage GetMessageNode(Guid correlationId)
		{
			return _messageNodeRepository.Where(x => x.CorrelationId == correlationId).FirstOrDefault();
		}

		public bool IsAssignedToMessage(ServiceMessage messageNode)
		{
			if (messageNode.CurrentState != ServiceMessage.WaitingForCompletion)
				return false;

			_log.InfoFormat("{0} WAITER: {1}.{2}:{3}", ControlUri,
				messageNode.CorrelationId, messageNode.BallotId, messageNode.ControlUri);

			return messageNode.CurrentState == ServiceMessage.WaitingForCompletion &&
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
			_bus = null;
			_controlBus = null;
			_endpointFactory = null;

			_disposed = true;
		}

		private void WaitUntilMessageNodeIsAvailable(Guid correlationId)
		{
			DateTime giveUpAt = DateTime.Now + 2.Seconds();
			var neverSurrender = new ManualResetEvent(false);

			while (DateTime.Now < giveUpAt)
			{
				int count = _messageNodeRepository.Where(x => x.CorrelationId == correlationId).Count();
				if (count > 0)
					break;

				neverSurrender.WaitOne(30, true);
			}
		}

		private void SendNodeOurServices(NotifyNewNodeAvailable message)
		{
			_log.InfoFormat("{0} sending node available response to {1}", _controlBus.Endpoint.Uri, message.ControlUri);

			IEndpoint endpoint = _endpointFactory.GetEndpoint(message.ControlUri);
			endpoint.Send(NewNotifyNodeAvailableMessage());

			_serviceNodeRepository
				.Where(x => x.ControlUri == _controlBus.Endpoint.Uri)
				.Each(x => endpoint.Send(new AddServiceNode
					{
						ControlUri = x.ControlUri,
						DataUri = x.DataUri,
						ServiceId = x.ServiceId,
						ServiceName = x.ServiceName,
					}, context => context.SendResponseTo(_controlBus.Endpoint)));
		}

		private void SubscribeGridSagas()
		{
			_unsubscribeAction += _controlBus.Subscribe<Node>();
			_unsubscribeAction += _controlBus.Subscribe<ServiceType>();
			_unsubscribeAction += _controlBus.Subscribe<ServiceNode>();
			_unsubscribeAction += _controlBus.Subscribe<ServiceMessage>();
		}

		private void NotifyAvailable()
		{
			var future = new SelectedFutureMessage<NotifyNewNodeAvailable>(x => x.ControlUri == _controlBus.Endpoint.Uri);

			UnsubscribeAction unsubscribeFuture = _controlBus.Subscribe(future);
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
			return new NotifyNodeAvailable(_controlBus.Endpoint.Uri, _bus.Endpoint.Uri, _created, DateTime.UtcNow);
		}

		~ServiceGrid()
		{
			Dispose(false);
		}
	}
}