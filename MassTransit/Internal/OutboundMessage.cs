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
	using Magnum.Common;

	public class OutboundMessage :
		MessageHeadersBase,
		IOutboundMessage
	{
		public static IOutboundMessage Headers
		{
			get { return LocalContext.Current.Retrieve(TypedKey<OutboundMessage>.UniqueKey, () => new OutboundMessage()); }
		}

		public static void Set(Action<IOutboundMessage> setAction)
		{
			var headers = Headers;

			setAction(headers);
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
	}
}