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
namespace MassTransit.Internal
{
	using System;

	public class InboundMessageContext :
		MessageContextBase,
		IInboundMessageContext
	{
		private ServiceBus _bus;

		public IServiceBus Bus
		{
			get { return _bus; }
		}

		public override void Clear()
		{
			base.Clear();

			_bus = null;
		}

		public void SetSourceAddress(Uri uri)
		{
			SourceAddress = uri;
		}

		public void SetDestinationAddress(Uri uri)
		{
			DestinationAddress = uri;
		}

		public void SetResponseAddress(Uri uri)
		{
			ResponseAddress = uri;
		}

		public void SetFaultAddress(Uri uri)
		{
			FaultAddress = uri;
		}

		public void Initialize(ServiceBus bus)
		{
			Clear();

			_bus = bus;
		}

		public IEndpoint GetResponseEndpoint()
		{
			return _bus.EndpointFactory.GetEndpoint(ResponseAddress);
		}

		public void SetMessage(object message)
		{
			Message = message;
		}

		public void SetRetryCount(int retryCount)
		{
			RetryCount = retryCount;
		}
	}
}