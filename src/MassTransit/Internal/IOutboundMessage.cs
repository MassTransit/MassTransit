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

	public interface IOutboundMessage :
		ISetMessageHeaders
	{
		void SendResponseTo(IServiceBus bus);
		void SendResponseTo(IEndpoint endpoint);
		void SendResponseTo(Uri uri);

		void SendFaultTo(IServiceBus bus);
		void SendFaultTo(IEndpoint endpoint);
		void SendFaultTo(Uri uri);

		void ExpiresAt(DateTime value);

		void IfNoSubscribers<T>(Action<T> action);
		void ForEachSubscriber<T>(Action<T, IEndpoint> action);
	}

	public interface IOutboundMessageContext :
		IOutboundMessage
	{
		void NotifyForMessageConsumer<T>(T message, IEndpoint endpoint);
		void NotifyNoSubscribers<T>(T message);
		bool WasEndpointAlreadySent(Uri endpointUri);
	}
}