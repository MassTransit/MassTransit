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
namespace MassTransit.Services.HealthMonitoring
{
	using System;
	using Internal;
	using Magnum;
	using Magnum.Actors;
	using Magnum.Actors.CommandQueues;
	using Magnum.Actors.Schedulers;
	using Messages;

	public class HealthClient :
		IBusService,
		Consumes<PingEndpoint>.All
	{
		private readonly int _heartbeatIntervalInMilliseconds;
		private readonly int _heartbeatIntervalInSeconds;
		private readonly CommandQueue _queue = new ThreadPoolCommandQueue();
		private IServiceBus _bus;
		private Uri _controlUri;
		private Uri _dataUri;
		private volatile bool _disposed;
		private Scheduler _scheduler = new ThreadPoolScheduler();

		public HealthClient()
			: this(3)
		{
		}

		/// <summary>
		/// Constructs a new HealthClient object
		/// </summary>
		/// <param name="intervalInSeconds">The heartbeat interval in seconds</param>
		public HealthClient(int intervalInSeconds)
		{
			_heartbeatIntervalInSeconds = intervalInSeconds;
			_heartbeatIntervalInMilliseconds = (int) TimeSpan.FromSeconds(_heartbeatIntervalInSeconds).TotalMilliseconds;

			SystemId = CombGuid.Generate();
		}

		public Guid SystemId { get; private set; }

		public void Consume(PingEndpoint message)
		{
			var response = new PingEndpointResponse(SystemId, _controlUri, _dataUri, _heartbeatIntervalInSeconds);

			CurrentMessage.Respond(response);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Start(IServiceBus bus)
		{
			_bus = bus;

			_controlUri = _bus.ControlBus.Endpoint.Uri;
			_dataUri = _bus.Endpoint.Uri;

			var message = new EndpointCameOnline(SystemId, _controlUri, _dataUri, _heartbeatIntervalInSeconds);
			_bus.ControlBus.Publish(message);

			_scheduler.Schedule(_heartbeatIntervalInMilliseconds, _heartbeatIntervalInMilliseconds, PublishHeartbeat);
		}

		public void Stop()
		{
			_bus.ControlBus.Publish(new EndpointWentOffline(SystemId, _controlUri, _dataUri, _heartbeatIntervalInSeconds));
			_scheduler.Dispose();
			_queue.Disable();
		}

		public virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			_scheduler.Dispose();
			_scheduler = null;

			_queue.Disable();

			_disposed = true;
		}

		public void PublishHeartbeat()
		{
			var message = new Heartbeat(SystemId, _controlUri, _dataUri, _heartbeatIntervalInSeconds);
			_bus.ControlBus.Publish(message);
		}

		~HealthClient()
		{
			Dispose(false);
		}
	}
}