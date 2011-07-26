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
namespace MassTransit.Subscriptions.Actors
{
	using System;
	using Stact;

	public interface SubscriptionChannel :
		UntypedChannel
	{
	}












	public class SubscriptionCoordinatorBusService :
		IBusService
	{
		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public void Start(IServiceBus bus)
		{
			throw new NotImplementedException();
		}

		public void Stop()
		{
			throw new NotImplementedException();
		}
	}


	public class SubscriptionRouter :
		Actor
	{
		readonly ChannelAdapter _adapter;

		public SubscriptionRouter()
		{
			_adapter = new ChannelAdapter();
		}

		public IDisposable Connect(SubscriptionChannel channel)
		{
			return _adapter.Connect(x => x.AddChannel(channel));
		}

		public void Handle(Message<AddSubscriptioXn> message)
		{
			_adapter.Send(message);
		}

		public void Handle(Message<RemoveSubscriptioXn> message)
		{
			_adapter.Send(message);
		}
	}
}