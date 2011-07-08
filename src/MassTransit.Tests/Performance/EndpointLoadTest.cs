// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.Tests.Performance
{
	using System.Diagnostics;
	using System.Threading;
	using Magnum.Extensions;
	using NUnit.Framework;

	public class EndpointLoadTest
	{
		private readonly IServiceBus _bus;
		private readonly int _messageCount;
		private readonly ManualResetEvent _requestEvent = new ManualResetEvent(false);
		private readonly ManualResetEvent _responseEvent = new ManualResetEvent(false);

		private int _requestCounter;
		private int _responseCounter;

		public EndpointLoadTest(IServiceBus bus, int messageCount)
		{
			_bus = bus;
			_messageCount = messageCount;
		}

		public void Run()
		{
			_bus.SubscribeHandler<LoadedRequest>(HandleLoadedRequest);
			_bus.SubscribeHandler<LoadedResponse>(HandleLoadedResponse);

			Stopwatch stopwatch = Stopwatch.StartNew();

			var endpoint = _bus.Endpoint;

			var request = new LoadedRequest(64);
			for (int index = 0; index < _messageCount; index++)
			{
				endpoint.Send(request);
			}

			bool requestComplete = _requestEvent.WaitOne(60.Seconds(), true);
			bool responseComplete = _responseEvent.WaitOne(60.Seconds(), true);

			stopwatch.Stop();

			Assert.IsTrue(requestComplete, "The requests did not complete");
			Assert.IsTrue(responseComplete, "The responses did not complete");

			Trace.WriteLine("Time to process " + _messageCount + " messages = " + stopwatch.ElapsedMilliseconds + "ms");
			Trace.WriteLine("Messages per second: " + _messageCount*1000/stopwatch.ElapsedMilliseconds);
		}

		private void HandleLoadedResponse(LoadedResponse obj)
		{
			Interlocked.Increment(ref _responseCounter);
			if (_responseCounter == _messageCount)
				_responseEvent.Set();
		}

		private void HandleLoadedRequest(LoadedRequest obj)
		{
			_bus.Publish(new LoadedResponse {Values = obj.Values});

			Interlocked.Increment(ref _requestCounter);
			if (_requestCounter == _messageCount)
				_requestEvent.Set();
		}
	}
}