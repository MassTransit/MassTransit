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

	public static class ExtensionsToBusContext
	{
		private static readonly Guid _inboundMessageContextKey = new Guid("E4DD4A53-C0B5-4B56-86A0-A7A88AE09B7A");
		private static readonly Guid _outboundMessageContextKey = new Guid("366BD633-7D83-4137-B764-422AE8B71E24");

		public static OutboundMessageContext OutboundMessage(this IBusContext busContext)
		{
			return busContext.Retrieve(_outboundMessageContextKey, () => new OutboundMessageContext());
		}

		public static void OutboundMessage(this IBusContext busContext, Action<OutboundMessageContext> action)
		{
			OutboundMessageContext context = OutboundMessage(busContext);

			action(context);
		}

		public static InboundMessageContext InboundMessage(this IBusContext busContext)
		{
			return busContext.Retrieve(_inboundMessageContextKey, () => new InboundMessageContext());
		}

		public static void InboundMessage(this IBusContext busContext, Action<InboundMessageContext> action)
		{
			InboundMessageContext context = InboundMessage(busContext);

			action(context);
		}
	}
}