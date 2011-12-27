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
namespace MassTransit.Services.HealthMonitoring
{
	using System;
	using Magnum;
	using Magnum.Extensions;
	using Messages;
	using Stact;
	using Stact.Internal;

	public class HealthClient :
		IBusService,
		Consumes<PingEndpoint>.All,
        DiagnosticsSource
	{
		private readonly int _heartbeatIntervalInMilliseconds;
		private readonly int _heartbeatIntervalInSeconds;
		private IServiceBus _bus;
		private Uri _controlUri;
		private Uri _dataUri;
		private volatile bool _disposed;
		private Fiber _fiber;
		private Scheduler _scheduler;
		private ScheduledOperation _unschedule;
		private UnsubscribeAction _unsubscribe;

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
			_fiber = new PoolFiber();
			_scheduler = new TimerScheduler(new PoolFiber());

			_heartbeatIntervalInSeconds = intervalInSeconds;
			_heartbeatIntervalInMilliseconds = (int) TimeSpan.FromSeconds(_heartbeatIntervalInSeconds).TotalMilliseconds;

			SystemId = CombGuid.Generate();
		}

		public Guid SystemId { get; private set; }

		public void Consume(PingEndpoint message)
		{
			var response = new PingEndpointResponse(SystemId, _controlUri, _dataUri, _heartbeatIntervalInSeconds);

			_bus.Context().Respond(response);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Start(IServiceBus bus)
		{
			_bus = bus;

			_controlUri = _bus.ControlBus.Endpoint.Address.Uri;
			_dataUri = _bus.Endpoint.Address.Uri;

			_unsubscribe = _bus.ControlBus.SubscribeInstance(this);

			var message = new EndpointCameOnline(SystemId, _controlUri, _dataUri, _heartbeatIntervalInSeconds);
			_bus.ControlBus.Publish(message);

			_unschedule = _scheduler.Schedule(_heartbeatIntervalInMilliseconds, _heartbeatIntervalInMilliseconds, _fiber, PublishHeartbeat);
		}

		public void Stop()
		{
			_bus.ControlBus.Publish(new EndpointWentOffline(SystemId, _controlUri, _dataUri, _heartbeatIntervalInSeconds));
			_unschedule.Cancel();
			_unsubscribe();
		}

	    public void Diagnose(DiagnosticsProbe probe)
	    {
	        probe.Add("health_client","on");
            probe.Add("health_client.interval", _heartbeatIntervalInSeconds);
	    }

	    public virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			_scheduler.Stop(60.Seconds());
			_scheduler = null;

			_fiber.Shutdown(60.Seconds());
			_fiber = null;

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