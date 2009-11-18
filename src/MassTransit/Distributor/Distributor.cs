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
namespace MassTransit.Distributor
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Messages;

	public class Distributor<T> :
		IDistributor<T>,
		Consumes<T>.Selected
		where T : class, CorrelatedBy<Guid>
	{
		private readonly IEndpointFactory _endpointFactory;
		private readonly IList<Uri> _workers = new List<Uri>();
		private UnsubscribeAction _unsubscribeAction = () => false;

		public Distributor(IEndpointFactory endpointFactory)
		{
			_endpointFactory = endpointFactory;
		}

		public void Consume(T message)
		{
			IEndpoint endpoint = _endpointFactory.GetEndpoint(_workers.First());

			var distributed = new Distributed<T>(message, CurrentMessage.Headers.ResponseAddress);

			endpoint.Send(distributed);
		}

		public bool Accept(T message)
		{
			return _workers.Count > 0;
		}

		public void Consume(WorkerAvailable<T> message)
		{
			if (_workers.Contains(CurrentMessage.Headers.SourceAddress))
				return;

			_workers.Add(CurrentMessage.Headers.SourceAddress);
		}

		public void Dispose()
		{
		}

		public void Start(IServiceBus bus)
		{
			_unsubscribeAction = bus.ControlBus.Subscribe<WorkerAvailable<T>>(Consume);

			// don't plan to unsubscribe this since it's an important thing
			bus.Subscribe(this);
		}

		public void Stop()
		{
			_workers.Clear();

			_unsubscribeAction();
		}
	}
}