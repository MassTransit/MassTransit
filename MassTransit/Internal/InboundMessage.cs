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
	using Magnum;

	public class InboundMessageHeaders :
		MessageHeadersBase,
		IInboundMessageHeaders,
		ISetInboundMessageHeaders
	{
		public static IInboundMessageHeaders Current
		{
			get { return LocalContext.Current.Retrieve(TypedKey<InboundMessageHeaders>.UniqueKey, () => new InboundMessageHeaders()); }
		}

		public IObjectBuilder ObjectBuilder { get; private set; }

		public object Message { get; private set; }

		public IServiceBus Bus { get; private set; }

		public override void Reset()
		{
			base.Reset();

			Bus = null;
		}

		public void ReceivedOn(IServiceBus bus)
		{
			Bus = bus;
		}

		public void ReceivedAs(object message)
		{
			Message = message;
		}

		public void SetObjectBuilder(IObjectBuilder objectBuilder)
		{
			ObjectBuilder = objectBuilder;
		}

		public static void SetCurrent(Action<ISetInboundMessageHeaders> action)
		{
			var context = LocalContext.Current.Retrieve(TypedKey<InboundMessageHeaders>.UniqueKey, () => new InboundMessageHeaders());

			action(context);
		}
	}
}