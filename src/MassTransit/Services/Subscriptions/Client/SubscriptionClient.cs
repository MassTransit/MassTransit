// Copyright 2007-2011 The Apache Software Foundation.
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
namespace MassTransit.Services.Subscriptions.Client
{
	using System;
	using System.Threading;
	using Exceptions;
	using log4net;
	using Magnum.Extensions;

	public class SubscriptionClient :
		IBusService
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (SubscriptionClient));
		private readonly ManualResetEvent _ready = new ManualResetEvent(false);
		private SubscriptionCoordinator _coordinator;
		private volatile bool _disposed;
		private IEndpoint _subscriptionServiceEndpoint;
		string _network;


		public SubscriptionClient(Uri subscriptionServiceUri)
		{
			SubscriptionServiceUri = subscriptionServiceUri;
			StartTimeout = 1.Minutes();
			_network = null;
		}

		public TimeSpan StartTimeout { get; set; }

		public Uri SubscriptionServiceUri { get; private set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Start(IServiceBus bus)
		{
			if (_log.IsInfoEnabled)
				_log.InfoFormat("Starting SubscriptionClient on {0}", bus.Endpoint.Address.Uri);

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Getting endpoint for subscription service at {0}", SubscriptionServiceUri);

			_subscriptionServiceEndpoint = bus.GetEndpoint(SubscriptionServiceUri);

			VerifyClientAndServiceNotOnSameEndpoint(bus);

			_ready.Reset();

			_coordinator = new SubscriptionCoordinator(bus.ControlBus, _subscriptionServiceEndpoint, _network, false);
			_coordinator.OnRefresh += CoordinatorOnRefresh;
			_coordinator.Start(bus);

			WaitForSubscriptionServiceResponse();
		}

		public void Stop()
		{
			_coordinator.Stop();
			_coordinator.OnRefresh -= CoordinatorOnRefresh;
		}

		public virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			using (_ready)
			{
			}

			_coordinator.Dispose();

			_disposed = true;
		}

		private void CoordinatorOnRefresh()
		{
			_ready.Set();
		}

		private void WaitForSubscriptionServiceResponse()
		{
			if (_log.IsDebugEnabled)
				_log.Debug("Waiting for response from the subscription service");

			bool received = _ready.WaitOne(StartTimeout);
			if (!received)
			{
				throw new InvalidOperationException("Timeout waiting for subscription service to respond");
			}
		}

		private void VerifyClientAndServiceNotOnSameEndpoint(IServiceBus bus)
		{
			if (!bus.ControlBus.Endpoint.Address.Uri.Equals(_subscriptionServiceEndpoint.Address.Uri))
				return;

			string message = "The service bus and subscription service cannot use the same endpoint: " + bus.ControlBus.Endpoint.Address.Uri;
			throw new EndpointException(bus.ControlBus.Endpoint.Address.Uri, message);
		}

		~SubscriptionClient()
		{
			Dispose(false);
		}
	}
}