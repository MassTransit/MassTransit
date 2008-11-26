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
namespace MassTransit.Host.Tests
{
	using System;

	public class SimpleMessageServer :
		IHostedService
	{
		private readonly IServiceBus _serviceBus;
		private int _hitCount;

		public SimpleMessageServer(IServiceBus serviceBus)
		{
			_serviceBus = serviceBus;
		}

		public void Start()
		{
			_serviceBus.Subscribe<MySimpleMessage>(HandleMySimpleMessage);
		}

		public void Stop()
		{
			_serviceBus.Unsubscribe<MySimpleMessage>(HandleMySimpleMessage);
		}

		public void Dispose()
		{
			_serviceBus.Dispose();
		}

		private void HandleMySimpleMessage(MySimpleMessage message)
		{
			_hitCount++;
		}
	}

	[Serializable]
	public class MySimpleMessage
	{
	}
}