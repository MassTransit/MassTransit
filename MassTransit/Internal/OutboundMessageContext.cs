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

	public class OutboundMessageContext :
		IOutboundMessageContext
	{
		private Uri _faultsTo;
		private Uri _replyTo;

		public Uri ReplyTo
		{
			get { return _replyTo; }
		}

		public Uri FaultsTo
		{
			get { return _faultsTo; }
		}

		public void SendReplyTo(IServiceBus bus)
		{
			_replyTo = bus.Endpoint.Uri;
		}

		public void SendReplyTo(IEndpoint endpoint)
		{
			_replyTo = endpoint.Uri;
		}

		public void Clear()
		{
			_replyTo = null;
			_faultsTo = null;
		}
	}
}