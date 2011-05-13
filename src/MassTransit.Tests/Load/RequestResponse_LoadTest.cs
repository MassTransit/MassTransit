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
namespace MassTransit.Tests.Load
{
	using System.Diagnostics;
	using System.Threading;
	using Magnum.Extensions;
	using NUnit.Framework;
	using Tests.Messages;
	using TextFixtures;

	[TestFixture]
	public class RequestResponse_LoadTest :
		LoopbackTestFixture
	{
		[Test, Explicit]
		public void Perform_a_large_request_response_pool()
		{
			const int repeatCount = 20000;

			int responsesReceived = 0;

			ManualResetEvent completed = new ManualResetEvent(false);

			LocalBus.SubscribeHandler<PingMessage>(x => LocalBus.Context().Respond(new PongMessage(x.CorrelationId)));
			LocalBus.SubscribeHandler<PongMessage>(x =>
				{
					if (Interlocked.Increment(ref responsesReceived) == repeatCount)
					{
						completed.Set();
					}
				});

			Thread.Sleep(3.Seconds());

			Stopwatch stopwatch = Stopwatch.StartNew();

			for (int index = 0; index < repeatCount; index++)
			{
				LocalBus.Publish(new PingMessage());
			}

			bool success = completed.WaitOne(60.Seconds(), true);

			stopwatch.Stop();

			Trace.WriteLine(string.Format("Elapsed Time for {0} messages = {1}", repeatCount * 2, stopwatch.Elapsed));
			Trace.WriteLine(string.Format("Messages Per Second = {0}", repeatCount * 2000 / stopwatch.ElapsedMilliseconds));
		}
	}
}