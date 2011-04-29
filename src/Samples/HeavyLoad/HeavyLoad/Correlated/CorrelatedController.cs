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
namespace HeavyLoad.Correlated
{
	using System;
	using Magnum.Extensions;
	using MassTransit;

	internal class CorrelatedController
	{
		private static readonly TimeSpan _timeout = 6.Seconds();
		private readonly IServiceBus _bus;
		private readonly Guid _id;
		private readonly Action<CorrelatedController> _successAction;
		private readonly Action<CorrelatedController> _timeoutAction;

		public CorrelatedController(IServiceBus bus, Action<CorrelatedController> successAction, Action<CorrelatedController> timeoutAction)
		{
			_bus = bus;
			_successAction = successAction;
			_timeoutAction = timeoutAction;

			_id = Guid.NewGuid();
		}

		public void SimulateRequestResponse()
		{
			_bus.MakeRequest(x => x.Publish(new SimpleRequestMessage(_id), y => y.SetResponseAddress(_bus.Endpoint.Uri)))
				.When<SimpleResponseMessage>().RelatedTo(_id).IsReceived(message =>
					{
						if (message.CorrelationId != _id)
							throw new ArgumentException("Unknown message response received");

						// we got a response, that's a happy ending!
						_successAction(this);
					})
				.OnTimeout(() =>
					{
						// we timed out, not so happy
						_timeoutAction(this);
					})
				.TimeoutAfter(_timeout)
				.Send();
		}
	}
}