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
	using System.Collections.Generic;
	using Magnum;

	public class OutboundMessage :
		MessageHeadersBase,
		IOutboundMessageContext
	{
		private Action<object, IEndpoint> _eachSubscriberAction = Ignore;
		private Action<object> _noSubscribersAction = Ignore;
		private HashSet<Uri> _endpoints = new HashSet<Uri>();

		public static IOutboundMessage Headers
		{
			get { return Context; }
		}

		internal static IOutboundMessageContext Context
		{
			get { return LocalContext.Current.Retrieve(TypedKey<OutboundMessage>.UniqueKey, () => new OutboundMessage()); }
		}

		public void SendResponseTo(IServiceBus bus)
		{
			SetResponseAddress(bus.Endpoint.Uri);
		}

		public void SendResponseTo(IEndpoint endpoint)
		{
			SetResponseAddress(endpoint.Uri);
		}

		public void SendResponseTo(Uri uri)
		{
			SetResponseAddress(uri);
		}

		public void SendFaultTo(IServiceBus bus)
		{
			SetFaultAddress(bus.Endpoint.Uri);
		}

		public void SendFaultTo(IEndpoint endpoint)
		{
			SetFaultAddress(endpoint.Uri);
		}

		public void SendFaultTo(Uri uri)
		{
			SetFaultAddress(uri);
		}

		public override void Reset()
		{
			base.Reset();

			_noSubscribersAction = Ignore;
			_eachSubscriberAction = Ignore;
			_endpoints.Clear();
		}

		public void IfNoSubscribers<T>(Action<T> action)
		{
			_noSubscribersAction = x => action((T) x);
		}

		public void ForEachSubscriber<T>(Action<T, IEndpoint> action)
		{
			_eachSubscriberAction = (message, endpoint) => action((T) message, endpoint);
		}

		public void NotifyForMessageConsumer<T>(T message, IEndpoint endpoint)
		{
			_endpoints.Add(endpoint.Uri);

			_eachSubscriberAction(message, endpoint);
		}

		public bool WasEndpointAlreadySent(Uri endpointUri)
		{
			return _endpoints.Contains(endpointUri);
		}

		public void NotifyNoSubscribers<T>(T message)
		{
			_noSubscribersAction(message);
		}

		public static void Set(Action<IOutboundMessage> setAction)
		{
			var headers = Headers;

			setAction(headers);
		}

		private static void Ignore<T>(T message)
		{
		}

		private static void Ignore<T>(T message, IEndpoint endpoint)
		{
		}
	}
}