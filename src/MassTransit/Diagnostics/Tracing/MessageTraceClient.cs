// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Diagnostics.Tracing
{
    using System;
    using Pipeline;


    public class MessageTraceClient :
		Consumes<ReceivedMessageTraceList>.All,
		IDisposable
	{
		readonly Action<ReceivedMessageTraceList> _callback;
		ConnectHandle _unsubscribe;

		public MessageTraceClient(IServiceBus bus, IEndpoint target, int count, Action<ReceivedMessageTraceList> callback)
		{
			_callback = callback;
			_unsubscribe = bus.SubscribeInstance(this);

			target.Send<GetMessageTraceList>(new GetMessageTraceListImpl {Count = count}, x => x.SendResponseTo(bus));
		}

		public void Consume(ReceivedMessageTraceList message)
		{
			if (_unsubscribe != null)
				_unsubscribe.Disconnect();
			_unsubscribe = null;

			_callback(message);
		}

		public void Dispose()
		{
			if (_unsubscribe != null)
                _unsubscribe.Disconnect();
            _unsubscribe = null;
		}
	}
}