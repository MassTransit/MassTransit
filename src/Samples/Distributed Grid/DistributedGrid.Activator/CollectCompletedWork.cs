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
namespace DistributedGrid.Activator
{
	using System;
	using System.Threading;
	using MassTransit;
	using Shared;
	using Shared.Messages;

	public class CollectCompletedWork :
		Consumes<CompletedSimpleWorkItem>.All,
		IServiceInterface
	{
		private readonly IServiceBus _bus;
		private UnsubscribeAction _unsubscribeAction;

		public CollectCompletedWork(IServiceBus bus)
		{
			_bus = bus;

			Thread.Sleep(1000);

			for (int i = 0; i < 100; i++)
			{
				var g = Guid.NewGuid();
				Console.WriteLine("Publishing: " + g);
				_bus.Publish(new DoSimpleWorkItem(g));
			}
		}

		public void Consume(CompletedSimpleWorkItem message)
		{
			Console.WriteLine("Got Item");
		}

		public void Start()
		{
			_unsubscribeAction = _bus.Subscribe(this);
		}

		public void Stop()
		{
			var action = _unsubscribeAction;
			if (action != null)
			{
				action();
			}
		}
	}
}